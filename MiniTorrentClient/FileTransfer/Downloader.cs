using System;
using System.Net.Sockets;
using System.Text;
using MiniTorrent_MediationServerContract;
using System.Threading;
using Newtonsoft.Json;

namespace MiniTorrent_GUI
{
    public class Downloader
    {
        public Object locker = new Object();

        private TcpClient socket;
        private int bytesStart;
        private int bytesEnd;
        private int receivedBytes;
        private int totalBytes;
        private FileDetails fileInfo;
        private FileDataContract dataContract;
        private ClientTask clientTask;
        private byte[] generalBuffer;
        private DownloadingFileItem downloadingFileItem;

        public Downloader(TcpClient socket, int bytesStart, int bytesEnd, FileDetails fileInfo, ClientTask clientTask, DownloadingFileItem downloadingFileItem)
        {
            this.socket = socket;
            this.bytesStart = bytesStart;
            this.bytesEnd = bytesEnd;
            this.fileInfo = fileInfo;
            this.downloadingFileItem = downloadingFileItem;
            receivedBytes = 0;
            totalBytes = bytesEnd - bytesStart;
            generalBuffer = new byte[totalBytes];

            this.clientTask = clientTask;
            dataContract = new FileDataContract 
            { 
                Filename = fileInfo.Name,
                StartByte = bytesStart,
                EndByte = bytesEnd,
                TotalFileSizeInBytes = (int)(fileInfo.Size*FilesHelper.ONE_MB)
            };

            Thread thread = new Thread(startNewDownlaod);
            thread.Start();
        }

        private void startNewDownlaod()
        {
            NetworkStream stream = socket.GetStream();

            string jsonString = JsonConvert.SerializeObject(dataContract);
            byte[] buffer = Encoding.ASCII.GetBytes(jsonString);
            stream.Write(buffer, 0, buffer.Length);

            beginReceive(stream);
        }

        private void beginReceive(NetworkStream stream)
        {
            byte[] buffer = new byte[TorrentWindow.BufferSize];
            int count;

            while (receivedBytes < totalBytes)
            {
                count = stream.Read(buffer, 0, buffer.Length);

                Array.Copy(buffer, 0, generalBuffer, receivedBytes, count);
                receivedBytes += count;
                updatePersentage(count);
            }
            submitFinalBuffer();
            stream.Close();
            socket.Close();
        }

        private void updatePersentage(int count)
        {
            lock (locker)
            {
                downloadingFileItem.Percentage += (double)((double)count / (double)dataContract.TotalFileSizeInBytes)*100;
            }
        }

        private void submitFinalBuffer()
        {
            clientTask.UpdateFinalFileBuffer(generalBuffer, bytesStart, totalBytes);

        }
    }
}
