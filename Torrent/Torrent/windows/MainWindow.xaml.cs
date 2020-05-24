using Microsoft.Win32;
using MonoTorrent.Client;
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
        private const int Port = 1317;
        private const int ShowingDownloadInfoTimeInterval = 2500;

        private TorrentClient torrentClient;
        private int selectedIndex;

        public MainWindow()
        {
            InitializeComponent();
            torrentClient = new TorrentClient(new UiManager(CommonInfoTextBox, TorrentsDataGrid), new PathsManager(), new TorrentSettingsManager(Port, ShowingDownloadInfoTimeInterval));
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

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            selectedIndex = TorrentsDataGrid.SelectedIndex;
            if (selectedIndex > -1 && selectedIndex < torrentClient.GetCurrentTorrentsCount())
            {
                await torrentClient.Pause(selectedIndex);
                var deleteTorrentWindow = new DeleteTorrentWindow();
                deleteTorrentWindow.Owner = this;
                deleteTorrentWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                deleteTorrentWindow.TorrentsFolderPath = torrentClient.pathsManager.TorrentsPath;
                deleteTorrentWindow.DownloadFolderPath = torrentClient.pathsManager.DownloadsPath;
                deleteTorrentWindow.DeletedTorrentManager = torrentClient.GetTorrentManager(selectedIndex);
                deleteTorrentWindow.ShowDialog();
                
                if (deleteTorrentWindow.IsCancelButtonPressed == false)
                {
                    torrentClient.DeleteTorrentManager(selectedIndex);
                }
            }
        }

        private async void SettingsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            await torrentClient.Pause();
            var settingsWindow = new SettingsWindow(); 
            settingsWindow.ShowDialog();
            var globalSettings = settingsWindow.GlobalSettings;
            if (globalSettings != null)
            {
                torrentClient.settingsManager.SetSettings(torrentClient.engine, globalSettings);
            }
        }
    }
}
