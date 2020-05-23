using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TorrentLibrary
{
    public class PathsManager
    {
        public string BasePath { get; private set; }
        public string DownloadsPath { get; private set; }
        public string FastResumeFilePath { get; private set; }
        public string TorrentsPath { get; private set; }
        public string DhtNodeFilePath { get; private set; }

        public PathsManager()
        {
            BasePath = Environment.CurrentDirectory;
            DownloadsPath = Path.Combine(BasePath, "Downloads");
            TorrentsPath = Path.Combine(BasePath, "Torrents");
            FastResumeFilePath = Path.Combine(TorrentsPath, "fastresume.data");
            DhtNodeFilePath = Path.Combine(BasePath, "DhtNodes");
        }
    }
}
