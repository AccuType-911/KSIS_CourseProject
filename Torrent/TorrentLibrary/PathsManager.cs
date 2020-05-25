using System;
using System.IO;

namespace TorrentLibrary
{
    public class PathsManager
    {
        private const string DefaultDownloadsFolderName = "Downloads";
        private const string DefaultTorrentsFolderName = "Torrents";
        private const string DefaultFastResumeFileName = "fastresume.data";
        private const string DefaultDhtNodeFileName = "DhtNodes";

        public string BasePath { get; private set; }
        public string DownloadsPath { get; private set; }
        public string FastResumeFilePath { get; private set; }
        public string TorrentsPath { get; private set; }
        public string DhtNodeFilePath { get; private set; }

        public PathsManager()
        {
            BasePath = Environment.CurrentDirectory;
            DownloadsPath = Path.Combine(BasePath, DefaultDownloadsFolderName);
            TorrentsPath = Path.Combine(BasePath, DefaultTorrentsFolderName);
            FastResumeFilePath = Path.Combine(TorrentsPath, DefaultFastResumeFileName);
            DhtNodeFilePath = Path.Combine(BasePath, DefaultDhtNodeFileName);

            if (!Directory.Exists(DownloadsPath))
                Directory.CreateDirectory(DownloadsPath);

            if (!Directory.Exists(TorrentsPath))
                Directory.CreateDirectory(TorrentsPath);
        }
    }
}
