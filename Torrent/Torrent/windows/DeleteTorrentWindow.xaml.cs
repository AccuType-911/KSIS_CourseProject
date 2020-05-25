using MonoTorrent.Client;
using System.IO;
using System.Windows;

namespace Torrent
{
    /// <summary>
    /// Логика взаимодействия для DeleteTorrentWindow.xaml
    /// </summary>
    public partial class DeleteTorrentWindow : Window
    {
        public TorrentManager DeletedTorrentManager { get; set; }
        public string TorrentsFolderPath { get; set; }
        public string DownloadFolderPath { get; set; }
        public bool? IsNoChangingExit { get; private set; }

        public DeleteTorrentWindow()
        {
            InitializeComponent();
        }

        private void DeleteFromDownloadsFolder()
        {
            var torrentName = DeletedTorrentManager.Torrent.Name;
            var deletedPath = Path.Combine(DownloadFolderPath, torrentName);
            if (Directory.Exists(deletedPath))
            {
                Directory.Delete(deletedPath);
            }
            if (File.Exists(deletedPath))
            {
                File.Delete(deletedPath);
            }
        }

        private void DeleteFromTorrentsFolder()
        {
            var torrentFileName = Path.GetFileName(DeletedTorrentManager.Torrent.TorrentPath);
            var deletedPath = Path.Combine(TorrentsFolderPath, torrentFileName);
            if (File.Exists(deletedPath))
            {
                File.Delete(deletedPath);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (DeleteFromTorrentsFolderCheckBox.IsChecked.Value)
            {
                DeleteFromTorrentsFolder();
            }
            if (DeleteFromDownloadsFolderCheckBox.IsChecked.Value)
            {
                DeleteFromDownloadsFolder();
            }
            IsNoChangingExit = false;
            Close();
        }

        private void DeleteButton_Copy_Click(object sender, RoutedEventArgs e)
        {
            IsNoChangingExit = true;
            Close();
        }
    }
}
