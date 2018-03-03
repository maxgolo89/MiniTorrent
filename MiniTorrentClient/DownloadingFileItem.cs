using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniTorrentClient
{
    public class DownloadingFileItem : INotifyPropertyChanged
    {
        private double mPercentage;
        private DateTime mEndedTime;

        public string Filename { get; set; }
        public float Size { get; set; }
        public DateTime StartedTime { get; set; }
        public DateTime EndedTime 
        {
            get { return mEndedTime; }
            set
            {
                mEndedTime = value;
                OnPropertyChanged("EndedTime");
            }
        }
        public double Percentage 
        {
            get { return mPercentage; }
            set
            {
                mPercentage = value;
                OnPropertyChanged("Percentage");
            }
        }

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
