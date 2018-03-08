using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MiniTorrentClient.FileTransfer;
using Newtonsoft.Json;
using FileInfo = System.IO.FileInfo;

namespace MiniTorrentClient
{
    /// <summary>
    /// Interaction logic for MiniTorrentProgram.xaml
    /// </summary>
    public partial class MiniTorrentProgram : Window
    {
        public Configuration CurrentConfiguration { get; set; }

        private List<AvailableFile> AvailableFiles;
        private CollectionViewSource AvailableFileSource;

        private List<DownloadingFile> DownloadingFiles;
        private CollectionViewSource DownloadingFileSource;
        private List<MyFile> OwnedFilesList;
        private UploadTask server;

        /// <summary>
        /// *** Constractor ***
        /// * Initialize GUI components.
        /// * Update the server and locally the owned file list.
        /// * Update available files for download.
        /// </summary>
        /// <param name="configuration"></param>
        public MiniTorrentProgram(Configuration configuration)
        {
            CurrentConfiguration = configuration;
            InitializeComponent();
            AvailableFileSource = (CollectionViewSource) (FindResource("AvailableFileSource"));
            AvailableFileSource.Source = AvailableFiles;
            UpdateOwnedFile();
            UpdateAvailableFiles();
            server = new UploadTask(configuration);
        }
        
        /// <summary>
        /// *** Update the available files list, and update the view of it ***
        /// 1. Send Json REST request to the server with credentials, to /filelist method uri.
        /// 2. Parse the result into AvailableFile objects list.
        /// 3. Update view.
        /// * Blocking method.
        /// * Need to refactor it to support cuncorrency.
        /// </summary>
        private void UpdateAvailableFiles()
        {
            AvailableFiles = new List<AvailableFile>();

            Uri getFilesUri = new Uri(CurrentConfiguration.ServerAddress + "/filelist");
            WebClient request = new WebClient();

            string cred = JsonConvert.SerializeObject(new
            {
                req = new
                {
                    username = CurrentConfiguration.Username,
                    password = CurrentConfiguration.Password
                } 
            });

            //request.UploadStringCompleted += UpdateAvailableFilesView;
            request.Headers.Add(HttpRequestHeader.ContentType, "application/json");
            string res = request.UploadString(getFilesUri, "POST", cred);
            dynamic resJson = JsonConvert.DeserializeObject(res);
            dynamic results = JsonConvert.DeserializeObject(resJson.GetFileListResult.ToString());
            foreach (var file in results.response.files)
            {
                AvailableFile newFile = new AvailableFile();
                newFile.Name = file.Name;
                newFile.Size = file.Size;
                newFile.Owners = new List<TorrentUser>();
                foreach (var owner in file.Resources)
                {
                    newFile.Owners.Add(new TorrentUser() {Username = owner.Username, Ip = owner.Ip, Port = owner.Port});
                }
                AvailableFiles.Add(newFile);
            }

            AvailableFileSource.Source = null;
            AvailableFileSource.Source = AvailableFiles;
        }

        /// <summary>
        /// *** Update local files list, and update the server ***
        /// 1. Traverse the files in the shared directory, and instantiate MyFile with each file.
        /// 2. Add each MyFile object to OwnedFilesList.
        /// 3. Send owned files list to the server.
        /// </summary>
        private void UpdateOwnedFile()
        {
            OwnedFilesList = new List<MyFile>();

            foreach (var file in Directory.GetFiles(CurrentConfiguration.SharedFolder))
            {
                FileInfo f = new FileInfo(file);
                OwnedFilesList.Add(new MyFile() {Name = f.Name, Size = (int)f.Length});
            }

            SendOwnedFilesUpdate();
        }

        /// <summary>
        /// *** Update the server with currently owned file list ***
        /// 1. Parse files into list of KeyValuePair of Name and Size.
        /// 2. Pack list with username and password to a json string.
        /// 3. Send update request to /updatefilelistbyuser method uri.
        /// </summary>
        private void SendOwnedFilesUpdate()
        {
            try
            {
                Uri loginUri = new Uri(CurrentConfiguration.ServerAddress + "/updatefilelistbyuser");
                List<KeyValuePair<string, int>> filesToShare = KeyValuePairFilesFactory(OwnedFilesList);

                object toSend = new
                {
                    username = CurrentConfiguration.Username,
                    password = CurrentConfiguration.Password,
                    files = filesToShare
                };

                string jsonLoginInfo = JsonConvert.SerializeObject(new
                {
                    req = toSend
                });

                WebClient request = new WebClient();
                request.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                request.UploadString(loginUri, jsonLoginInfo);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
            
        }

        /// <summary>
        /// *** Takes list of MyFile objects and repacks it to list of KeyValuePair's of Name and Size ***
        /// </summary>
        /// <returns></returns>
        private List<KeyValuePair<string, int>> KeyValuePairFilesFactory(List<MyFile> filesToRepack)
        {
            List<KeyValuePair<string, int>> files = new List<KeyValuePair<string, int>>();
            if (OwnedFilesList.Count == 0)
                return files;

            foreach (var file in filesToRepack)
            {
                string fileName = file.Name;
                int fileSize = (int)file.Size;
                files.Add(new KeyValuePair<string, int>(fileName, fileSize));
            }

            return files;
        }

        /// <summary>
        /// *** Handle Close event, logout user ***
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                Uri logout = new Uri(CurrentConfiguration.ServerAddress + "/logout");
                WebClient request = new WebClient();
                string logoutJson = JsonConvert.SerializeObject(new
                {
                    req = new
                    {
                        username = CurrentConfiguration.Username,
                        password = CurrentConfiguration.Password
                    }
                });
                request.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                request.UploadString(logout, "POST", logoutJson);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }

            
        }




        private void UpdateFilesButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateOwnedFile();
            UpdateAvailableFiles();
        }

        private void RequestAFileButton_Click(object sender, RoutedEventArgs e)
        {
            AvailableFile fileToDownload = (AvailableFile)AvailableFileDataGrid.SelectedItem;
            DownloadTask download = new DownloadTask(fileToDownload, CurrentConfiguration);
        }

        private void ReflectAFile_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
