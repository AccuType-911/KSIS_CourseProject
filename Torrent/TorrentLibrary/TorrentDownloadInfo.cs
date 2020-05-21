using MonoTorrent.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace TorrentLibrary
{
    public class TorrentDownloadInfo
    {
        public int Number { get; set; }
        public TorrentState State { get; set; }
        public string Name { get; set; }
        public string Progress { get; set; }
        public string DownloadSpeed { get; set; }
        public string UploadSpeed { get; set; }
        public string DownloadedData { get; set; }
        public string UploadedData { get; set; }

    }
}
