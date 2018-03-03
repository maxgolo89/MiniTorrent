using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

namespace MiniTorrentClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
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
    }
}
