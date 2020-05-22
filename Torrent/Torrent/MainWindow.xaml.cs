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
        private int selectedIndex;

        public MainWindow()
        {
            InitializeComponent();
            torrentClient = new TorrentClient(CommonInfoTextBox, TorrentsDataGrid);
            torrentClient.CheckTorrentsFolder();
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
            selectedIndex = TorrentsDataGrid.SelectedIndex;
            if (selectedIndex > -1 && selectedIndex < torrentClient.GetCurrentTorrentsCount())
            {
                await Task.Run(() => torrentClient.StartEngine(selectedIndex));
            }
            else
            {
                await Task.Run(() => torrentClient.StartEngine());
            }
        }

        private async void StopDownloadButton_Click(object sender, RoutedEventArgs e)
        {
            selectedIndex = TorrentsDataGrid.SelectedIndex;
            if (selectedIndex > -1 && selectedIndex < torrentClient.GetCurrentTorrentsCount())
            {
                await Task.Run(() => torrentClient.Pause(selectedIndex));
            }
            else
            {
                await Task.Run(() => torrentClient.Pause());
            }
        }
    }
}
