using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;

namespace MiniTorrentClient.FileTransfer
{
    class DownloadTask
    {
        private AvailableFile file;
        private Configuration configuration;
        private Queue<DownloadStream> downloadQueue;
        private LinkedList<Thread> threads;
        private byte[] recievedFile;
        public int TotalBytesReceived = 0;
        private static object theLock = new object();
        private static object theFileLock = new object();


        public DownloadTask(AvailableFile file, Configuration configuration)
        {
            this.file = file;
            this.configuration = configuration;
            downloadQueue = new Queue<DownloadStream>();
            threads = new LinkedList<Thread>();
            recievedFile = new byte[file.Size];
            ConnectionInitializer();
        }

        /// <summary>
        /// *** Setup connection to all peers and make initial request ***
        /// </summary>
        private async void ConnectionInitializer()
        {
            try
            {
                if (file.Owners.Count == 0)
                    return;

                int bytePerPeer = file.Size / file.Owners.Count;

                for (int i = 0; i < file.Owners.Count; i++)
                {
                    FileRequest fileRequest = new FileRequest();
                    TcpClient tcpClient = new TcpClient();
                    await tcpClient.ConnectAsync(IPAddress.Parse(file.Owners[i].Ip), file.Owners[i].Port);

                    fileRequest.Name = file.Name;
                    fileRequest.Size = file.Size;
                    fileRequest.StartByte = bytePerPeer * i;
                    fileRequest.EndByte = (i == file.Owners.Count - 1) ? file.Size : bytePerPeer * (i + 1);

                    downloadQueue.Enqueue(new DownloadStream(fileRequest, tcpClient));
                    Thread thread = new Thread(DownloadFile);
                    threads.AddLast(thread);
                    thread.Start();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            
        }

        /// <summary>
        /// *** File download initiator ***
        /// </summary>
        private async void DownloadFile()
        {
            DownloadStream dStream = null;
            NetworkStream stream = null;
            TcpClient tcpClient = null;
            try
            {
                lock (theLock)
                {
                    dStream = downloadQueue.Dequeue();
                    if (dStream != null)
                    {
                        tcpClient = dStream.DownloadClient;
                        stream = tcpClient.GetStream();
                    }
                        
                }

                if (stream == null)
                    return;
                
                string json = JsonConvert.SerializeObject(dStream.DownloadFile);
                byte[] writeBuffer = Encoding.ASCII.GetBytes(json);

                // Send file request to the seed
                await stream.WriteAsync(writeBuffer, 0, writeBuffer.Length);
                
                await Reciever(stream, dStream);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                stream?.Close();
                dStream?.DownloadClient?.Close();
            }
            
        }

        /// <summary>
        /// *** File receiver ***
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="dStream"></param>
        private async Task Reciever(NetworkStream stream, DownloadStream dStream)
        {
            byte[] recBuffer = new byte[SizeConstants.KILOBYTE * 32];
            int byteToReceive = dStream.DownloadFile.EndByte - dStream.DownloadFile.StartByte;
            int byteReceived = 0;

            try
            {
                while (byteReceived < byteToReceive)
                {
                    int count = await stream.ReadAsync(recBuffer, 0, recBuffer.Length);
                    lock (theFileLock)
                    {
                        Array.Copy(recBuffer, 0, recievedFile, dStream.DownloadFile.StartByte + byteReceived, count);
                        byteReceived += count;
                        TotalBytesReceived += count;
                    }
                }

                DownloadFinished();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void DownloadFinished()
        {
            Console.WriteLine(Thread.CurrentThread.Name + "Is finishing...");
            lock (theFileLock)
            {
                if (TotalBytesReceived != file.Size)
                    return;
            }

            if (!Directory.Exists(configuration.DestinationFolder))
            {
                Directory.CreateDirectory(configuration.DestinationFolder);
            }

            Console.WriteLine(configuration.DestinationFolder + @"\" + file.Name);
            File.WriteAllBytes(configuration.DestinationFolder + @"\" + file.Name, recievedFile);
        }
    }

    internal class DownloadStream
    {
        public FileRequest DownloadFile;
        public TcpClient DownloadClient;

        public DownloadStream(FileRequest fileRequest, TcpClient tcpClient)
        {
            this.DownloadFile = fileRequest;
            this.DownloadClient = tcpClient;
        }
    }
}
