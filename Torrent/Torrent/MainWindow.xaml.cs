using Microsoft.Win32;
using System.Threading.Tasks;
using System.Windows;
using TorrentLibrary;

namespace Torrent
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string OpenFileDialogFilter = "Торрент - файл | *.torrent";

        private TorrentClient torrentClient;

        public MainWindow()
        {
            InitializeComponent();
            torrentClient = new TorrentClient(CommonInfoTextBox, TorrentsDataGrid);
        }

        private void AddTorrentButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = OpenFileDialogFilter;
            if (openFileDialog.ShowDialog().Value)
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
