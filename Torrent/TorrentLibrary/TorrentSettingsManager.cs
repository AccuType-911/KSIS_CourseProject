using MonoTorrent.Client;

namespace TorrentLibrary
{
    public class TorrentSettingsManager
    {
        public GlobalSettings GetCurrentSettings(ClientEngine engine)
        {
            var engineSettings = engine.Settings;
            var globalSettings = new GlobalSettings();
            globalSettings.Port = engineSettings.ListenPort;
            globalSettings.DownloadSpeedLimit = engineSettings.MaximumDownloadSpeed / 1024;
            globalSettings.UploadSpeedLimit = engineSettings.MaximumUploadSpeed / 1024;
            globalSettings.MaxDiskReadSpeed = engineSettings.MaximumDiskReadRate / 1024;
            globalSettings.MaxDiskWriteSpeed = engineSettings.MaximumDiskWriteRate / 1024;
            globalSettings.MaxOpenConnectionsCount = engineSettings.MaximumConnections;
            return globalSettings;
        }

        public void SetSettings(ClientEngine engine, GlobalSettings globalSettings)
        {
            var engineSettings = engine.Settings;
            engineSettings.ListenPort = globalSettings.Port;
            engineSettings.MaximumDownloadSpeed = globalSettings.DownloadSpeedLimit * 1024;
            engineSettings.MaximumUploadSpeed = globalSettings.UploadSpeedLimit * 1024;
            engineSettings.MaximumDiskReadRate = globalSettings.MaxDiskReadSpeed * 1024;
            engineSettings.MaximumDiskWriteRate = globalSettings.MaxDiskWriteSpeed * 1024;
            engineSettings.MaximumConnections = globalSettings.MaxOpenConnectionsCount;
        }
    }
}
