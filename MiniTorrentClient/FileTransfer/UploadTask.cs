using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;

namespace MiniTorrentClient.FileTransfer
{
    class UploadTask
    {
        public delegate void NewUpload(string name, int size, string ip);
        public delegate void UpdateFileUploadProgress(string name, double percentage, string ip);
        public delegate void UpdateFileUploadFinished(string name, double percentage, string ip, DateTime endTime);

        public event NewUpload NewUploadEventHandler;
        public event UpdateFileUploadProgress UploadProgressEventHandler;
        public event UpdateFileUploadFinished UploadFinishedEventHandler;
        private Configuration configuration;
        private TcpListener connListener;
        private Queue<TcpClient> connectionQueue;
        private static object lockObject = new object();

        public UploadTask(Configuration configuration)
        {
            this.configuration = configuration;
            connectionQueue = new Queue<TcpClient>();
            ConnectionListener();
        }

        /// <summary>
        /// *** Connection listener ***
        /// 1. Setup TcpLiestener to listen to the configured ip and port.
        /// 3. Wait for connection.
        /// 2. On connection request, create a new thread for that connection.
        /// </summary>
        private async void ConnectionListener()
        {
            try
            {
                connListener = new TcpListener(IPAddress.Parse(configuration.HostIp), configuration.InPort);
                connListener.Start();
                while (true)
                {
                    TcpClient tcpClient = await connListener.AcceptTcpClientAsync();
                    connectionQueue.Enqueue(tcpClient);
                    Thread uploadThread = new Thread(ConnectionInitializer);
                    uploadThread.Start();
                }
            }
            catch (Exception e)
            {
                connListener.Stop();
                MessageBox.Show(e.Message);
            }
            finally
            {
                connListener?.Stop();
            }
        }

        /// <summary>
        /// *** Connection initializer, upload task caller ***
        /// 1. Obtain the network stream.
        /// 2. Read request from the network stream.
        /// 3. Invoke UploadFie method, which sends the file to the peer.
        /// </summary>
        private async void ConnectionInitializer()
        {
            // Set buffer size
            byte[] readBuffer = new byte[SizeConstants.KILOBYTE*32];
            NetworkStream stream = null;
            TcpClient tcpClient = null;

            try
            {
                // Obtain network stream from queued connection
                lock (lockObject)
                {
                    tcpClient = connectionQueue.Dequeue();
                    if (tcpClient != null)
                    {
                        stream = tcpClient.GetStream();
                    }
                        
                }

                if (stream == null)
                    return;

                // Read from network stream
                await stream.ReadAsync(readBuffer, 0, readBuffer.Length);
                
                // Translate buffer content to string and then to FileRequest object.
                string requestString = Encoding.ASCII.GetString(readBuffer);
                FileRequest file = JsonConvert.DeserializeObject<FileRequest>(requestString);
                NewUploadEventHandler?.Invoke(file.Name, file.Size, tcpClient.Client.RemoteEndPoint.ToString());
                // Upload the file to the requester
                await UploadFile(stream, file, tcpClient.Client.RemoteEndPoint.ToString());
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                stream?.Close();
                tcpClient?.Close();
            }
            
        }

        /// <summary>
        /// *** Upload file async method ***
        /// 1. Open file stream for local file.
        /// 2. Send the file in chunks.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="fileRequest"></param>
        private async Task UploadFile(NetworkStream stream, FileRequest fileRequest, string ip)
        {
            // Set buffer size
            byte[] readBuffer = new byte[SizeConstants.KILOBYTE * 32];
            int byteSent= 0;

            // Open the uploaded file, and create a file stream.
            FileInfo localFile = new FileInfo(configuration.SharedFolder + @"\" + fileRequest.Name);
            using (FileStream fileStream = new FileStream(localFile.FullName, FileMode.Open, FileAccess.Read))
            {
                try
                {
                    // Set position in the file to the first asked byte.
                    fileStream.Seek(fileRequest.StartByte, 0);
                    // While asked bytes not completely sent.
                    while (fileRequest.StartByte + byteSent < fileRequest.EndByte)
                    {
                        int countOfBytesLeft = fileRequest.EndByte - (fileRequest.StartByte + byteSent);
                        if (countOfBytesLeft >= readBuffer.Length)
                        {
                            /* If bytes left to send, are more then the buffer size, send buffer size bytes. */
                            fileStream.Read(readBuffer, 0, readBuffer.Length);
                            await stream.WriteAsync(readBuffer, 0, readBuffer.Length);
                            byteSent += readBuffer.Length;
                        }
                        else
                        {
                            /* If bytes left to send are less then the buffer size, send the number of bytes left. */
                            fileStream.Read(readBuffer, 0, countOfBytesLeft);
                            await stream.WriteAsync(readBuffer, 0, countOfBytesLeft);
                            byteSent += countOfBytesLeft;
                        }
                        UploadProgressEventHandler?.Invoke(fileRequest.Name, ((double)byteSent/(fileRequest.EndByte - fileRequest.StartByte)) * 100, ip);
                    }
                    UploadFinishedEventHandler?.Invoke(fileRequest.Name, 100.0, ip, DateTime.Now);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
        }
    }
}
