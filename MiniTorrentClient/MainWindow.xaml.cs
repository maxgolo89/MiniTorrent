using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.XPath;
using MiniTorrentClient.ResponseData;
using Newtonsoft.Json;
using MessageBox = System.Windows.MessageBox;

namespace MiniTorrentClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Constants
        private readonly string ConfigFileName = "MyConfig.xml";
        private readonly string NodeMiniTorrentParameters = "MiniTorrentParameters";
        private readonly string NodeServerIpAddress = "ServerIpAddress";
        private readonly string NodeIncomingPort = "IncomingPort";
        private readonly string NodeOutgoingPort = "OutgoingPort";
        private readonly string NodeUsername = "Username";
        private readonly string NodePassword = "Password";
        private readonly string NodeSharedFolder = "SharedFolder";
        private readonly string NodeDestinationFolder = "DestinationFolder";
        #endregion

        private Configuration CurrentConfiguration = null;
        private JavaScriptSerializer serializer = new JavaScriptSerializer();

        public MainWindow()
        {
            InitializeComponent();
            LoadConfiguration();
            LoadConfigurationToGui();
        }

        /// <summary>
        /// Load all configurable properties to GUI
        /// </summary>
        private void LoadConfigurationToGui()
        {
            if (CurrentConfiguration == null)
                return;

            tb_server_ip.Text = CurrentConfiguration.ServerAddress;
            tb_server_username.Text = CurrentConfiguration.Username;
            tb_server_password.Text = CurrentConfiguration.Password;
            tb_client_port_in.Text = CurrentConfiguration.InPort.ToString();
            tb_client_port_out.Text = CurrentConfiguration.OutPort.ToString();
            tb_client_shared_folder.Text = CurrentConfiguration.SharedFolder;
            tb_client_dest_folder.Text = CurrentConfiguration.DestinationFolder;
        }

        /// <summary>
        /// Load confiuration from MyConfig.xml
        /// </summary>
        private void LoadConfiguration()
        {
            if (CurrentConfiguration == null)
                CurrentConfiguration = new Configuration();

            // Resolve host ip address
            CurrentConfiguration.HostIp = HostIpAddressHelper();

            // Load configuration from xml
            XmlDocument xmlConfFile = new XmlDocument();
            xmlConfFile.Load(ConfigFileName);
            try
            {
                XmlNode parentNode = xmlConfFile.SelectSingleNode("/" + NodeMiniTorrentParameters);
                CurrentConfiguration.ServerAddress = parentNode.SelectSingleNode(NodeServerIpAddress).InnerText;
                CurrentConfiguration.Username = parentNode.SelectSingleNode(NodeUsername).InnerText;
                CurrentConfiguration.Password = parentNode.SelectSingleNode(NodePassword).InnerText;
                CurrentConfiguration.InPort = Convert.ToInt32(parentNode.SelectSingleNode(NodeIncomingPort).InnerText);
                CurrentConfiguration.OutPort = Convert.ToInt32(parentNode.SelectSingleNode(NodeOutgoingPort).InnerText);
                CurrentConfiguration.SharedFolder = parentNode.SelectSingleNode(NodeSharedFolder).InnerText;
                CurrentConfiguration.DestinationFolder = parentNode.SelectSingleNode(NodeDestinationFolder).InnerText;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        /// <summary>
        /// Host IP address resolver
        /// </summary>
        /// <returns></returns>
        private string HostIpAddressHelper()
        {
            var host = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (var ip in host)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return "localhost";
        }

        /// <summary>
        /// Event handler for selection of the folder to share browse button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_client_shared_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.ShowDialog();
                tb_client_shared_folder.Text = dialog.SelectedPath;
            }
        }

        /// <summary>
        /// Event handler for the selection of the destination folder browse button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_client_dest_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.ShowDialog();
                tb_client_dest_folder.Text = dialog.SelectedPath;
            }
        }

        /// <summary>
        /// Event handler for connect button click.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_connect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Update configuration from text fields
                UpdateConfiguration();

                // Setup needed login information
                Uri loginUri = new Uri(CurrentConfiguration.ServerAddress + "/login");
                List<KeyValuePair<string, int>> filesToShare = KeyValuePairFilesFactory();

                object toSend = new
                {
                    username = CurrentConfiguration.Username,
                    password = CurrentConfiguration.Password,
                    ip = CurrentConfiguration.HostIp,
                    port = CurrentConfiguration.InPort,
                    files = filesToShare
                };

                string jsonLoginInfo = serializer.Serialize(new
                {
                    req = toSend
                });


                // Setup connection
                WebClient request = new WebClient();
                request.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                var response = request.UploadString(loginUri, jsonLoginInfo);
                if (response == null)
                    MessageBox.Show("Something went wrong");
                else
                {
                    // Parse result string
                    dynamic result = JsonConvert.DeserializeObject(response);
                    string innerJson = result.LoginResult.ToString();
                    InnerResult resultData = JsonConvert.DeserializeObject<InnerResult>(innerJson);

                    // If username returned, login successful, open MiniTorrentProgram window.
                    if (resultData.response.username.Equals(CurrentConfiguration.Username))
                    {
                        MiniTorrentProgram torrent = new MiniTorrentProgram(CurrentConfiguration);
                        torrent.Show();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Unseccessful login");
                    }
                }                   

            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        /// <summary>
        /// Create list of KeyValuePair of the shared and downloaded folder files.
        /// </summary>
        /// <returns></returns>
        private List<KeyValuePair<string, int>> KeyValuePairFilesFactory()
        {
            List<KeyValuePair<string, int>> files = new List<KeyValuePair<string, int>>();
            if (!Directory.Exists(CurrentConfiguration.SharedFolder))
                return files;

            foreach (var file in Directory.GetFiles(CurrentConfiguration.SharedFolder))
            {
                var fileInstance = new FileInfo(file);
                string fileName = fileInstance.Name;
                int fileSize = (int)fileInstance.Length;
                files.Add(new KeyValuePair<string, int>(fileName, fileSize));
            }

            foreach (var file in Directory.GetFiles(CurrentConfiguration.DestinationFolder))
            {
                var fileInstance = new FileInfo(file);
                string fileName = fileInstance.Name;
                int fileSize = (int)fileInstance.Length;
                files.Add(new KeyValuePair<string, int>(fileName, fileSize));
            }

            return files;
        }

        /// <summary>
        /// Update configuration file and object from text boxes.
        /// </summary>
        private void UpdateConfiguration()
        {
            XmlDocument xmlConfFile = new XmlDocument();
            xmlConfFile.Load(ConfigFileName);
            XmlNode parentNode = xmlConfFile.SelectSingleNode("/" + NodeMiniTorrentParameters);

            try
            {
                // Server ip changed
                if (tb_server_ip.Text != CurrentConfiguration.ServerAddress)
                {
                    CurrentConfiguration.ServerAddress = tb_server_ip.Text;
                    parentNode.SelectSingleNode(NodeServerIpAddress).InnerText = tb_server_ip.Text;
                }

                // Username changed
                if (tb_server_username.Text != CurrentConfiguration.Username)
                {
                    CurrentConfiguration.Username = tb_server_username.Text;
                    parentNode.SelectSingleNode(NodeUsername).InnerText = tb_server_username.Text;
                }
                
                // Password changed
                if (tb_server_password.Text != CurrentConfiguration.Password)
                {
                    CurrentConfiguration.Password = tb_server_password.Text;
                    parentNode.SelectSingleNode(NodePassword).InnerText = tb_server_password.Text;
                }

                // In port changed
                if (tb_client_port_in.Text != CurrentConfiguration.InPort.ToString())
                {
                    CurrentConfiguration.InPort = Convert.ToInt32(tb_client_port_in.Text);
                    parentNode.SelectSingleNode(NodeIncomingPort).InnerText = tb_client_port_in.Text;
                }

                // Out port changed
                if (tb_client_port_out.Text != CurrentConfiguration.OutPort.ToString())
                {
                    CurrentConfiguration.OutPort = Convert.ToInt32(tb_client_port_out.Text);
                    parentNode.SelectSingleNode(NodeOutgoingPort).InnerText = tb_client_port_out.Text;
                }

                // Shared folder changed
                if (tb_client_shared_folder.Text != CurrentConfiguration.SharedFolder)
                {
                    CurrentConfiguration.SharedFolder = tb_client_shared_folder.Text;
                    parentNode.SelectSingleNode(NodeSharedFolder).InnerText = tb_client_shared_folder.Text;
                }

                // Destination folder changed
                if (tb_client_dest_folder.Text != CurrentConfiguration.DestinationFolder)
                {
                    CurrentConfiguration.DestinationFolder = tb_client_dest_folder.Text;
                    parentNode.SelectSingleNode(NodeDestinationFolder).InnerText = tb_client_dest_folder.Text;
                }

                xmlConfFile.Save(ConfigFileName);
            }
            catch (Exception)
            {
                throw new Exception("Something is wrong with the configuration.");
            }
        }
    }
}
