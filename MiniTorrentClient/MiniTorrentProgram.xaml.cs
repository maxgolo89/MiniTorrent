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
        public static object theLock = new object();
        public static object theUploadLock = new object();
        public Configuration CurrentConfiguration { get; set; }

        private List<AvailableFile> AvailableFiles;
        private CollectionViewSource AvailableFileSource;

        private List<FileUploadDownloadProgress> DownloadingFiles;
        private CollectionViewSource DownloadingFileSource;

        private List<FileUploadDownloadProgress> uploadingFiles;
        private CollectionViewSource uploadingFileSource;

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

            // Initialize available file list
            AvailableFileSource = (CollectionViewSource) (FindResource("AvailableFileSource"));
            AvailableFileSource.Source = AvailableFiles;

            // Initialize downloading file list
            DownloadingFileSource = (CollectionViewSource) (FindResource("DownloadingFileSource"));
            DownloadingFileSource.Source = DownloadingFiles;
            DownloadingFiles = new List<FileUploadDownloadProgress>();

            // Initialize uploading file list
            uploadingFileSource = (CollectionViewSource) (FindResource("UploadingFileSource"));
            uploadingFileSource.Source = uploadingFiles;
            uploadingFiles = new List<FileUploadDownloadProgress>();

            UpdateOwnedFile();
            UpdateAvailableFiles();

            // Connection listener for upload
            server = new UploadTask(configuration);
            server.NewUploadEventHandler += NewUpload;
            server.UploadProgressEventHandler += UpdateUploadProgress;
            server.UploadFinishedEventHandler += UpdateUploadFinished;
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

            foreach (var file in Directory.GetFiles(CurrentConfiguration.DestinationFolder))
            {
                FileInfo f = new FileInfo(file);
                OwnedFilesList.Add(new MyFile() { Name = f.Name, Size = (int)f.Length });
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
        /// *** New upload view initialization ***
        /// Creates a new FileUploadDownloadProgress object.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="size"></param>
        /// <param name="ip"></param>
        private void NewUpload(string name, int size, string ip)
        {
            FileUploadDownloadProgress uploadFile = new FileUploadDownloadProgress() {Filename = name, StartedTime = DateTime.Now, Size = size, Ip = ip, Percentage = 0};
            uploadingFiles.Add(uploadFile);
            /* As CollectionViewSource cannot be accessed from different threads other than the UI thread,
             * The UI refresh delegated to the dispatcher. */
            App.Current.Dispatcher.Invoke((Action) delegate
            {
                uploadingFileSource.Source = null;
                uploadingFileSource.Source = uploadingFiles;
            });
        }

        /// <summary>
        /// *** Update progress ***
        /// Takes uploading file name, percentage of uploaded bytes, and destination ip address,
        /// and updates the appropriate progress object.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="percentage"></param>
        /// <param name="ip"></param>
        private void UpdateUploadProgress(string name, double percentage, string ip)
        {
            // Get the FileUploadDownloadProgress for the updated file upload progress
            FileUploadDownloadProgress uploadFile = GetFileUploadingByNameAndDestination(name, ip);

            // Check if progress object found
            if (uploadFile == null)
                return;

            // Update the progress bar
            uploadFile.Percentage = percentage;
        }

        /// <summary>
        /// *** Update upload is finished ***
        /// Puts an end time stamp to the appropriate FileUploadDownload object.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="percentage"></param>
        /// <param name="ip"></param>
        /// <param name="endTime"></param>
        private void UpdateUploadFinished(string name, double percentage, string ip, DateTime endTime)
        {
            // Update progress
            UpdateUploadProgress(name, percentage, ip);

            // Get the FileUploadDownloadProgress for the updated file upload progress
            FileUploadDownloadProgress uploadFile = GetFileUploadingByNameAndDestination(name, ip);

            // Check if progress object found
            if (uploadFile == null)
                return;

            uploadFile.EndedTime = endTime;
        }


        /// <summary>
        /// *** Fetch appropriate upload progress object ***
        /// </summary>
        /// <param name="name"></param>
        /// <param name="ip"></param>
        /// <returns></returns>
        private FileUploadDownloadProgress GetFileUploadingByNameAndDestination(string name, string ip)
        {
            // iterate the list and compare file name and ip.
            foreach(var upload in uploadingFiles)
            {
                if (upload.Filename == name && upload.Ip == ip)
                {
                    return upload;
                }
            }

            return null;
        }

        /// <summary>
        /// *** Update download progress ***
        /// Invoked on update event from download task.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="percentage"></param>
        private void UpdateDownloadProgress(string name, double percentage)
        {
            FileUploadDownloadProgress downloadFile = GetFileDownloadingByName(name);
            if (downloadFile == null)
                return;

            lock (theLock)
            {
                downloadFile.Percentage = percentage;
            }
        }

        /// <summary>
        /// *** Update download finished ***
        /// Invoked on update event from download task.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="percentage"></param>
        /// <param name="endTime"></param>
        private void UpdateDownloadFinished(string name, double percentage, DateTime endTime)
        {
            FileUploadDownloadProgress downloadFile = GetFileDownloadingByName(name);
            if (downloadFile == null)
                return;

            lock (theLock)
            {
                downloadFile.Percentage = percentage;
                downloadFile.EndedTime = endTime;
            }

            UpdateOwnedFile();
        }

        /// <summary>
        /// *** Search file in own files ***
        /// *** HELPER METHOD ***
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private FileUploadDownloadProgress GetFileDownloadingByName(string name)
        {
            FileUploadDownloadProgress downloadFile = null;
            foreach (var download in DownloadingFiles)
            {
                if (download.Filename == name)
                {
                    downloadFile = download;
                    break;
                }
            }

            if (downloadFile == null)
                return null;

            return downloadFile;
        }

        /// <summary>
        /// *** Check if user have a file ***
        /// true - file not found.
        /// false - file was found.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private bool ValidateFile(string name)
        {
            foreach (var file in OwnedFilesList)
            {
                if (file.Name == name)
                    return false;
            }

            return true;
        }


        /// <summary>
        /// *** Handle Close event ***
        /// Logs out the user.
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


        /// <summary>
        /// *** Update own and available files ***
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateFilesButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateOwnedFile();
            UpdateAvailableFiles();
        }

        /// <summary>
        /// *** Make a download request ***
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RequestAFileButton_Click(object sender, RoutedEventArgs e)
        {
            AvailableFile fileToDownload = (AvailableFile)AvailableFileDataGrid.SelectedItem;
            if (!ValidateFile(fileToDownload.Name))
            {
                MessageBox.Show("You already own this file");
                return;
            }

            DownloadTask download = new DownloadTask(fileToDownload, CurrentConfiguration);
            FileUploadDownloadProgress progressView = new FileUploadDownloadProgress() {Filename = fileToDownload.Name, Percentage = 0, Size = fileToDownload.Size, StartedTime = DateTime.Now};
            DownloadingFiles.Add(progressView);
            DownloadingFileSource.Source = null;
            DownloadingFileSource.Source = DownloadingFiles;
            download.DownloadProgressEventHandler += UpdateDownloadProgress;
            download.DownloadFinishedEventHandler += UpdateDownloadFinished;
            download.ConnectionInitializer();
        }

        
        /// <summary>
        /// TO BE IMPLEMENTED
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReflectAFile_Click(object sender, RoutedEventArgs e)
        {
            
        }

        /// <summary>
        /// *** Handle search ***
        /// On each character search within available files.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            string fileName = SearchTextBox.Text;

            if (fileName == string.Empty)
                ClearButton.IsEnabled = false;
            else
                ClearButton.IsEnabled = true;

            var resultlist = from aFile in AvailableFiles where aFile.Name.Contains(fileName) select aFile;
            AvailableFileSource.Source = resultlist.ToList();
        }

        /// <summary>
        /// *** Handle clear button click ***
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearButton_OnClick(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = "";
        }
    }
}
