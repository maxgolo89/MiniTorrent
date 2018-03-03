using System;
using System.Collections.Generic;
using System.Windows;
using System.IO;
using System.Xml;
using MiniTorrent_MediationServerContract;
using Newtonsoft.Json;
using System.Net;
using System.Net.Sockets;
using System.ServiceModel;

namespace MiniTorrent_GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Constants
        public const string ConfigFileName = "MyConfig.xml";
        public const string MiniTorrentParams = "MiniTorrentParams";
        public const string ServerIpAddress = "ServerIpAddress";
        public const string IncomingPort = "IncomingTcpPort";
        public const string OutgoingPort = "OutgoingTcpPort";
        public const string Username = "Username";
        public const string Password = "Password";
        public const string SourceDir = "PublishedFilesSource";
        public const string DestDir = "DownloadedFilesDestination";
        #endregion

        private MediationReference.MediationServerContractClient client;
        private ConnectionDetails connectionDetails;
        private string localIp;

        public MainWindow()
        {
            InitializeComponent();
            
        }

        private bool validateConfigFile()
        {
            localIp = GetLocalIPAddress();

            if (connectionDetails == null)
            {
                connectionDetails = new ConnectionDetails();
            }
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(ConfigFileName);

            try
            {
                XmlNode paramsNode = xDoc.SelectSingleNode("/"+ MiniTorrentParams);
                connectionDetails.ServerIpAddress = paramsNode.SelectSingleNode(ServerIpAddress).InnerText;
                connectionDetails.IncomingTcpPort = Convert.ToInt32(paramsNode.SelectSingleNode(IncomingPort).InnerText);
                connectionDetails.OutgoingTcpPort = Convert.ToInt32(paramsNode.SelectSingleNode(OutgoingPort).InnerText);
                connectionDetails.Username = paramsNode.SelectSingleNode(Username).InnerText;
                connectionDetails.Password = paramsNode.SelectSingleNode(Password).InnerText;
                connectionDetails.PublishedFilesSource = paramsNode.SelectSingleNode(SourceDir).InnerText;
                connectionDetails.DownloadedFilesDestination = paramsNode.SelectSingleNode(DestDir).InnerText;
            }
            catch
            {
                MessageBox.Show("Please check your settings & MyConfig.xml file for valid format.\ni.e.:<MiniTorrentParams>\n" +
                        "< ServerIpAddress > 192.168.0.109 </ ServerIpAddress >\n" +
                        "< IncomingTcpPort > 8005 </ IncomingTcpPort >\n" +
                        "< OutgoingTcpPort > 8006 </ OutgoingTcpPort >\n" +
                        "< Username > Username1 </ Username >\n" +
                        "< Password > password1 </ Password >\n" +
                        "< PublishedFilesSource > D:\\UTorrent </ PublishedFilesSource >\n" +
                        "< DownloadedFilesDestination > D:\\MiniTorrent_Downloaded </ DownloadedFilesDestination >\n" +
                        "</ MiniTorrentParams >", "Invalid MyConfig.xml File",MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            if (!PreDefinedConfigCheckBox.IsChecked.Value)
                updateConfigFile();

            if (!validateConfigFile())
                return;

            EndpointAddress remoteAddress = new EndpointAddress("http://" + connectionDetails.ServerIpAddress +
                ":8089/MediationService");
            client = new MediationReference.MediationServerContractClient("BasicHttpBinding_IMediationServerContract", remoteAddress);

            try
            {
                if (!ConnectToServer())
                {
                    MessageBox.Show("Failed to connect to Torrent server", "Failure", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            catch (System.ServiceModel.EndpointNotFoundException)
            {
                MessageBox.Show("Failed to communicate with the server,\nPlease validate that the service is up.","Failure", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            TorrentWindow torrentWin = new TorrentWindow(client, connectionDetails, localIp);
            torrentWin.Show();
            this.Close();
        }

        private bool ConnectToServer()
        {
            List<FileDetails> filesList = FilesHelper.getFilesList(connectionDetails.PublishedFilesSource);
            JsonItems jsonItems = new JsonItems
            {
                Username = connectionDetails.Username,
                Password = connectionDetails.Password,
                Ip = localIp,
                Port = connectionDetails.IncomingTcpPort.ToString(),
                AllFiles = filesList
            };
            string toSend = JsonConvert.SerializeObject(jsonItems);

            return client.SingIn(toSend);
        }
       
        private void updateConfigFile()
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(ConfigFileName);
            XmlNode paramsNode = xDoc.SelectSingleNode("/" + MiniTorrentParams);

            paramsNode.SelectSingleNode(ServerIpAddress).InnerText = ServerIpAddressTextbox.Text;
            paramsNode.SelectSingleNode(IncomingPort).InnerText = IncomingTcpPortTextbox.Text;
            paramsNode.SelectSingleNode(OutgoingPort).InnerText = OutgoingTcpPortTextbox.Text;
            paramsNode.SelectSingleNode(Username).InnerText = UsernameTextbox.Text;
            paramsNode.SelectSingleNode(Password).InnerText = PasswordTextbox.Password;
            paramsNode.SelectSingleNode(SourceDir).InnerText = PublishedFilesSourceTextbox.Text;
            paramsNode.SelectSingleNode(DestDir).InnerText = DownloadedFilesDestTextbox.Text;

            xDoc.Save(ConfigFileName);
        }

        private void checkBox_Changed(object sender, RoutedEventArgs e)
        {
            bool check = PreDefinedConfigCheckBox.IsChecked.Value;

            ServerIpAddressTextbox.IsEnabled = !check;
            IncomingTcpPortTextbox.IsEnabled = !check;
            OutgoingTcpPortTextbox.IsEnabled = !check;
            UsernameTextbox.IsEnabled = !check;
            PasswordTextbox.IsEnabled = !check;
            PublishedFilesSourceTextbox.IsEnabled = !check;
            DownloadedFilesDestTextbox.IsEnabled = !check;
        }

        public string GetLocalIPAddress()
        {
            if (IpCheckBox.IsChecked.Value)
            {
                return IPTextBox.Text;
            }

            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("Local IP Address Not Found!");
        }

        private void SourceButton_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                dialog.ShowDialog();
                PublishedFilesSourceTextbox.Text = dialog.SelectedPath;
            }

        }

        private void DestinationButton_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                dialog.ShowDialog();
                DownloadedFilesDestTextbox.Text = dialog.SelectedPath;
            }
        }

        private void IpCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (IpCheckBox.IsChecked.Value)
                IPTextBox.Visibility = Visibility.Visible;
            else
                IPTextBox.Visibility = Visibility.Hidden;
        }
    }
}
