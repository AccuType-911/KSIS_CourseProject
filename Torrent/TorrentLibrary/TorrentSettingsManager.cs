using MonoTorrent.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace TorrentLibrary
{
    public class TorrentSettingsManager
    {
        public int Port { get; private set; }
        public int ShowingInfoTimeInterval { get; private set; }

        public TorrentSettingsManager(int port, int showingInfoTimeInterval)
        {
            Port = port;
            ShowingInfoTimeInterval = showingInfoTimeInterval;
        }

        public void SetSettings(ClientEngine engine, GlobalSettings globalSettings)
        {
            engine.Settings.MaximumDownloadSpeed = globalSettings.DownloadSpeedLimit * 1024;
            engine.Settings.MaximumUploadSpeed = globalSettings.UploadSpeedLimit * 1024;
        }
    }
}
