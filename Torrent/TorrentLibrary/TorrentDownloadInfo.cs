using MonoTorrent.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace TorrentLibrary
{
    public class TorrentDownloadInfo
    {
        public TorrentState State { get; set; }
        public string Name { get; set; }
        public double Progress { get; set; }
        public double DownloadSpeed { get; set; }
        public double UploadSpeed { get; set; }
        public double DataBytesDownloaded { get; set; }
        public double DataBytesUploaded { get; set; }

    }
}
