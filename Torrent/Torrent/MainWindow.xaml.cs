using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TorrentLibrary;

namespace Torrent
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TorrentClient torrentClient;

        public MainWindow()
        {
            InitializeComponent();
            torrentClient = new TorrentClient(LogTextBox, TorrentsGrid);
        }

        private void AddTorrentButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Торрент - файл | *.torrent";
            if ((bool)openFileDialog.ShowDialog())
            {
                var torrentPath = openFileDialog.FileName;
                torrentClient.AddTorrent(torrentPath);
            }
        }

        private async void ResumeDownloadButton_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(()=> torrentClient.StartEngine());
        }
    }
}
