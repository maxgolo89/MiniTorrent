using System;
using System.ComponentModel;

namespace MiniTorrentClient
{
    public class FileUploadDownloadProgress : INotifyPropertyChanged
    {
        private double mPercentage;
        private DateTime mEndedTime;
        public string Filename { get; set; }
        public float Size { get; set; }
        public DateTime StartedTime { get; set; }
        
        // Use for uploading only
        public string Ip { get; set; }

        /// <summary>
        /// *** Getter and Setter ***
        /// Notify ended time changed.
        /// </summary>
        public DateTime EndedTime
        {
            get { return mEndedTime; }
            set
            {
                mEndedTime = value;
                OnPropertyChanged("EndedTime");
            }
        }

        /// <summary>
        /// *** Getter and Setter ***
        /// Notify on Percentage changed.
        /// </summary>
        public double Percentage
        {
            get { return mPercentage; }
            set
            {
                mPercentage = value;
                OnPropertyChanged("Percentage");
            }
        }

        /// <summary>
        /// *** Update view on property changes ***
        /// </summary>
        /// <param name="propertyName"></param>
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}