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
        private int selectedTorrentIndex;

        public MainWindow()
        {
            InitializeComponent();
            torrentClient = new TorrentClient(new UiManager(CommonInfoTextBox, TorrentsDataGrid), new PathsManager(), new TorrentSettingsManager());
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
            selectedTorrentIndex = TorrentsDataGrid.SelectedIndex;
            if (selectedTorrentIndex > -1 && selectedTorrentIndex < torrentClient.GetCurrentTorrentsCount())
            {
                await Task.Run(() => torrentClient.StartEngine(selectedTorrentIndex));
            }
            else
            {
                await Task.Run(() => torrentClient.StartEngine());
            }
        }

        private async void StopDownloadButton_Click(object sender, RoutedEventArgs e)
        {
            selectedTorrentIndex = TorrentsDataGrid.SelectedIndex;
            if (selectedTorrentIndex > -1 && selectedTorrentIndex < torrentClient.GetCurrentTorrentsCount())
            {
                await Task.Run(() => torrentClient.Pause(selectedTorrentIndex));
            }
            else
            {
                await Task.Run(() => torrentClient.Pause());
            }
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            selectedTorrentIndex = TorrentsDataGrid.SelectedIndex;
            if (selectedTorrentIndex > -1 && selectedTorrentIndex < torrentClient.GetCurrentTorrentsCount())
            {
                await torrentClient.Pause(selectedTorrentIndex);
                var deleteTorrentWindow = new DeleteTorrentWindow();
                deleteTorrentWindow.Owner = this;
                deleteTorrentWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                deleteTorrentWindow.TorrentsFolderPath = torrentClient.pathsManager.TorrentsPath;
                deleteTorrentWindow.DownloadFolderPath = torrentClient.pathsManager.DownloadsPath;
                deleteTorrentWindow.DeletedTorrentManager = torrentClient.GetTorrentManager(selectedTorrentIndex);
                deleteTorrentWindow.ShowDialog();

                if (deleteTorrentWindow.IsNoChangingExit == false)
                {
                    torrentClient.DeleteTorrentManager(selectedTorrentIndex);
                }
            }
        }

        private async void SettingsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            await torrentClient.Pause();
            var currentGlobalSettings = torrentClient.settingsManager.GetCurrentSettings(torrentClient.engine);
            var settingsWindow = new SettingsWindow(currentGlobalSettings);
            settingsWindow.ShowDialog();
            if (settingsWindow.IsNoChangingExit == false)
            {
                var globalSettings = settingsWindow.GlobalSettings;
                torrentClient.settingsManager.SetSettings(torrentClient.engine, globalSettings);
            }
        }
    }
}
