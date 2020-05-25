namespace TorrentLibrary
{
    public class GlobalSettings
    {
        public int Port { get; set; }
        public int DownloadSpeedLimit { get; set; }
        public int UploadSpeedLimit { get; set; }
        public int MaxDiskReadSpeed { get; set; }
        public int MaxDiskWriteSpeed { get; set; }
        public int MaxOpenConnectionsCount { get; set; }
    }
}
