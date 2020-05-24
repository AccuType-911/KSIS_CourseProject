using MonoTorrent;
using MonoTorrent.BEncoding;
using MonoTorrent.Client;
using MonoTorrent.Dht;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TorrentLibrary
{
    public class TorrentClient
    {
        private List<TorrentManager> torrentsManagers;
        private UiManager uiManager;

        public ClientEngine engine;
        public TorrentSettingsManager settingsManager;
        public PathsManager pathsManager;

        public TorrentClient(UiManager uiManager, PathsManager pathsManager, TorrentSettingsManager settingsManager)
        {
            torrentsManagers = new List<TorrentManager>();
            this.uiManager = uiManager;
            this.pathsManager = pathsManager;
            this.settingsManager = settingsManager;

            AppDomain.CurrentDomain.ProcessExit += delegate { Shutdown().Wait(); };
            AppDomain.CurrentDomain.UnhandledException += delegate (object sender, UnhandledExceptionEventArgs e) { uiManager.TextBoxWriteLine(e.ExceptionObject.ToString()); Shutdown().Wait(); };
            Setup();
        }

        public void DeleteTorrentManager(int selectedIndex)
        {
            torrentsManagers.RemoveAt(selectedIndex);
            var torrentsDownloadInfo = GetTorrentsDownloadInfo();
            uiManager.TorrentsDataGridUpdate(torrentsDownloadInfo);
        }

        private bool CheckActiveTorrents()
        {
            foreach (var manager in torrentsManagers)
            {
                if (manager.State == TorrentState.Downloading)
                {
                    return true;
                }
            }
            return false;
        }

        public int GetCurrentTorrentsCount()
        {
            return torrentsManagers.Count;
        }

        public TorrentManager GetTorrentManager(int selectedTorrentIndex)
        {
            return torrentsManagers[selectedTorrentIndex];
        }

        private BEncodedDictionary LoadFastResumeFile()
        {
            try
            {
                if (File.Exists(pathsManager.FastResumeFilePath))
                    return BEncodedValue.Decode<BEncodedDictionary>(File.ReadAllBytes(pathsManager.FastResumeFilePath));
                return null;
            }
            catch
            {
                return null;
            }
        }

        public async void Setup()
        {
            var port = settingsManager.Port;
            var engineSettings = new EngineSettings();
            engineSettings.SavePath = pathsManager.DownloadsPath;
            engineSettings.ListenPort = port;

            engine = new ClientEngine(engineSettings);

            var nodes = Array.Empty<byte>();
            try
            {
                if (File.Exists(pathsManager.DhtNodeFilePath))
                    nodes = File.ReadAllBytes(pathsManager.DhtNodeFilePath);
            }
            catch
            {
                uiManager.TextBoxWriteLine("No existing dht nodes could be loaded");
            }

            var dhtEngine = new DhtEngine(new IPEndPoint(IPAddress.Any, port));
            await engine.RegisterDhtAsync(dhtEngine);
            await engine.DhtEngine.StartAsync(nodes);
        }

        private bool CheckTorrentInTorrentsPath(string torrentName)
        {
            if (File.Exists(Path.Combine(pathsManager.TorrentsPath, torrentName)))
            {
                return true;
            }
            return false;
        }

        public void CheckTorrentsFolder()
        {
            foreach (var torrentPath in Directory.GetFiles(pathsManager.TorrentsPath))
            {
                AddTorrent(torrentPath);
            }
            var torrentsDownloadInfo = GetTorrentsDownloadInfo();
            uiManager.TorrentsDataGridUpdate(torrentsDownloadInfo);
        }

        private void CopyTorrentToTorrentsFolder(string torrentPath)
        {
            var torrentName = Path.GetFileName(torrentPath);
            byte[] torrentBytes;
            using (var fileStream = new FileStream(torrentPath, FileMode.Open))
            {
                torrentBytes = new byte[fileStream.Length];
                fileStream.Read(torrentBytes, 0, torrentBytes.Length);
            }
            using (var fileStream = new FileStream(Path.Combine(pathsManager.TorrentsPath, torrentName), FileMode.Create))
            {
                fileStream.Write(torrentBytes, 0, torrentBytes.Length);
            }
        }

        public async void AddTorrent(string torrentPath)
        {
            var torrentName = Path.GetFileName(torrentPath);
            if (!CheckTorrentInTorrentsPath(torrentName))
            {
                CopyTorrentToTorrentsFolder(torrentPath);
            }
            var fastResume = LoadFastResumeFile();
            Torrent torrent = null;
            var torrentDefaults = new TorrentSettings();
            if (torrentPath.EndsWith(".torrent", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    torrent = await Torrent.LoadAsync(torrentPath);

                }
                catch (Exception exception)
                {
                    uiManager.TextBoxWriteLine("Couldn't decode: " + torrentPath + " ");
                    uiManager.TextBoxWriteLine(exception.Message);
                }

                var manager = new TorrentManager(torrent, pathsManager.DownloadsPath, torrentDefaults);
                if (fastResume != null && fastResume.ContainsKey(torrent.InfoHash.ToHex()))
                    manager.LoadFastResume(new FastResume((BEncodedDictionary)fastResume[torrent.InfoHash.ToHex()]));
                await engine.Register(manager);

                torrentsManagers.Add(manager);

                var torrentsDownloadInfo = GetTorrentsDownloadInfo();
                uiManager.TorrentsDataGridUpdate(torrentsDownloadInfo);
            }
        }

        public async Task StartEngine()
        {
            if (torrentsManagers.Count == 0)
            {
                uiManager.TextBoxWriteLine("No torrents");
                uiManager.TextBoxWriteLine("Exiting...");
                engine.Dispose();
                return;
            }

            foreach (var manager in torrentsManagers)
            {
                await manager.StartAsync();
            }

            await engine.EnablePortForwardingAsync(CancellationToken.None);

            ShowDownloadInfo(torrentsManagers);

            await engine.DisablePortForwardingAsync(CancellationToken.None);
        }

        public async Task StartEngine(int selectedIndex)
        {
            if (torrentsManagers.Count == 0)
            {
                uiManager.TextBoxWriteLine("No torrents");
                uiManager.TextBoxWriteLine("Exiting...");
                engine.Dispose();
                return;
            }

            await torrentsManagers[selectedIndex].StartAsync();

            await engine.EnablePortForwardingAsync(CancellationToken.None);

            ShowDownloadInfo(torrentsManagers);

            if (!CheckActiveTorrents())
            {
                await engine.DisablePortForwardingAsync(CancellationToken.None);
            }
        }

        private string GetCommonInfo()
        {
            var commonInfoStringBuilder = new StringBuilder(1024);
            AppendFormat(commonInfoStringBuilder, "Total Download Rate: {0:0.00} kB/s", engine.TotalDownloadSpeed / 1024.0);
            AppendFormat(commonInfoStringBuilder, "Total Upload Rate:   {0:0.00} kB/s", engine.TotalUploadSpeed / 1024.0);
            AppendFormat(commonInfoStringBuilder, "Disk Read Rate:      {0:0.00} kB/s", engine.DiskManager.ReadRate / 1024.0);
            AppendFormat(commonInfoStringBuilder, "Disk Write Rate:     {0:0.00} kB/s", engine.DiskManager.WriteRate / 1024.0);
            AppendFormat(commonInfoStringBuilder, "Total Read:          {0:0.00} kB", engine.DiskManager.TotalRead / 1024.0);
            AppendFormat(commonInfoStringBuilder, "Total Written:       {0:0.00} kB", engine.DiskManager.TotalWritten / 1024.0);
            AppendFormat(commonInfoStringBuilder, "Open Connections:    {0}", engine.ConnectionManager.OpenConnections);
            return commonInfoStringBuilder.ToString();
        }

        private TorrentDownloadInfo GetTorrentDownloadInfo(TorrentManager manager, int number)
        {
            var downloadInfo = new TorrentDownloadInfo();
            downloadInfo.Number = number;
            downloadInfo.State = manager.State;
            downloadInfo.Name = manager.Torrent == null ? "MetaDataMode" : manager.Torrent.Name;
            downloadInfo.Progress = manager.Progress != 100.0 ? string.Format("{0:0.00} %", manager.Progress) : "Загружено";
            downloadInfo.DownloadSpeed = string.Format("{0:0.00} kB/s", manager.Monitor.DownloadSpeed / 1024.0);
            downloadInfo.UploadSpeed = string.Format("{0:0.00} kB/s", manager.Monitor.UploadSpeed / 1024.0);
            downloadInfo.DownloadedData = string.Format("{0:0.00} MB", manager.Monitor.DataBytesDownloaded / (1024.0 * 1024.0));
            downloadInfo.UploadedData = string.Format("{0:0.00} MB", manager.Monitor.DataBytesUploaded / (1024.0 * 1024.0));
            return downloadInfo;
        }

        private List<TorrentDownloadInfo> GetTorrentsDownloadInfo()
        {
            var torrentsDownloadInfo = new List<TorrentDownloadInfo>();
            var number = 1;
            foreach (var manager in torrentsManagers)
            {
                var torrentDownloadInfo = GetTorrentDownloadInfo(manager, number);
                torrentsDownloadInfo.Add(torrentDownloadInfo);
                number++;
            }
            return torrentsDownloadInfo;
        }

        private void ShowDownloadInfo(List<TorrentManager> torrents)
        {
            bool isRunning = true;
            while (isRunning)
            {
                isRunning = torrents.Exists(manager => manager.State != TorrentState.Stopped);

                uiManager.TextBoxClear();
                uiManager.TextBoxWriteLine(GetCommonInfo());
                var torrentsDownloadInfo = GetTorrentsDownloadInfo();
                uiManager.TorrentsDataGridUpdate(torrentsDownloadInfo);

                Thread.Sleep(settingsManager.ShowingInfoTimeInterval);
            }
        }

        private void AppendFormat(StringBuilder stringBuilder, string data, params object[] formatting)
        {
            if (formatting != null)
                stringBuilder.AppendFormat(data, formatting);
            else
                stringBuilder.Append(data);
            stringBuilder.AppendLine();
        }

        public async Task Pause()
        {
            for (var i = 0; i < torrentsManagers.Count; i++)
            {
                var stoppingTask = torrentsManagers[i].StopAsync();
                while (torrentsManagers[i].State != TorrentState.Stopped)
                {
                    Thread.Sleep(250);
                }
                await stoppingTask;
            }
        }

        public async Task Pause(int selectedIndex)
        {
            var stoppingTask = torrentsManagers[selectedIndex].StopAsync();
            while (torrentsManagers[selectedIndex].State != TorrentState.Stopped)
            {
                Thread.Sleep(250);
            }
            await stoppingTask;

        }

        public async Task Shutdown()
        {
            var fastResume = new BEncodedDictionary();
            for (var i = 0; i < torrentsManagers.Count; i++)
            {
                var stoppingTask = torrentsManagers[i].StopAsync();
                while (torrentsManagers[i].State != TorrentState.Stopped)
                {
                    Thread.Sleep(250);
                }
                await stoppingTask;

                if (torrentsManagers[i].HashChecked)
                    fastResume.Add(torrentsManagers[i].Torrent.InfoHash.ToHex(), torrentsManagers[i].SaveFastResume().Encode());
            }

            var nodes = await engine.DhtEngine.SaveNodesAsync();
            File.WriteAllBytes(pathsManager.DhtNodeFilePath, nodes);
            File.WriteAllBytes(pathsManager.FastResumeFilePath, fastResume.Encode());
            engine.Dispose();

            Thread.Sleep(2000);
        }
    }
}

