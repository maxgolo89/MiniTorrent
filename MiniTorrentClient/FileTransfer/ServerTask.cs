using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace MiniTorrent_GUI
{
    class ServerTask
    {
        private TorrentWindow torrentWindow;
        private ConnectionDetails connDetails;
        private string localIp;

        private TcpListener clientListener;

        public ServerTask(ConnectionDetails connDetails, string localIp, TorrentWindow torrentWindow)
        {
            this.torrentWindow = torrentWindow;
            this.localIp = localIp;
            this.connDetails = connDetails;

            WaitForConnections();
        }
    

        public async void WaitForConnections()
        {
            try
            {
                clientListener = new TcpListener(IPAddress.Parse(localIp), connDetails.IncomingTcpPort);
                clientListener.Start();
                while (true)
                {
                    TcpClient clientSocket = await clientListener.AcceptTcpClientAsync();
                    Uploader uploader = new Uploader(clientSocket, connDetails, torrentWindow); 
                }
            }catch
            {
                
            }
        }

        public void CloseConnection()
        {
            clientListener.Stop();
        }


    }

}
