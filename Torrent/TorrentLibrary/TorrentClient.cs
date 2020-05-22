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
using System.Windows.Controls;

namespace TorrentLibrary
{
    public class TorrentClient
    {
        private const int Port = 1317;
        private const int ShowInfoTimeInterval = 3000;

        private TextBox commonInfoTextBox;
        private DataGrid torrentsDataGrid;

        private string basePath;
        private string downloadsPath;
        private string fastResumeFilePath;
        private string torrentsPath;
        private string dhtNodeFile;
        private ClientEngine engine;
        private List<TorrentManager> torrentsManagers;

        public TorrentClient(TextBox textBox, DataGrid dataGrid)
        {
            commonInfoTextBox = textBox;
            torrentsDataGrid = dataGrid;
            basePath = Environment.CurrentDirectory;
            downloadsPath = Path.Combine(basePath, "Downloads");
            torrentsPath = Path.Combine(basePath, "Torrents");
            fastResumeFilePath = Path.Combine(torrentsPath, "fastresume.data");
            dhtNodeFile = Path.Combine(basePath, "DhtNodes");
            torrentsManagers = new List<TorrentManager>();

            AppDomain.CurrentDomain.ProcessExit += delegate { Shutdown().Wait(); };
            AppDomain.CurrentDomain.UnhandledException += delegate (object sender, UnhandledExceptionEventArgs e) { TextBoxWriteLine(e.ExceptionObject.ToString()); Shutdown().Wait(); };
            Thread.GetDomain().UnhandledException += delegate (object sender, UnhandledExceptionEventArgs e) { TextBoxWriteLine(e.ExceptionObject.ToString()); Shutdown().Wait(); };
            Setup();
        }

        private bool CheckActiveTorrents()
        {
            foreach (var manager in torrentsManagers) {
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

        private void TextBoxClear()
        {
            commonInfoTextBox.Dispatcher.Invoke(() =>
            {
                commonInfoTextBox.Clear();
            });
        }

        private void TextBoxWriteLine(string data)
        {
            commonInfoTextBox.Dispatcher.Invoke(() =>
            {
                commonInfoTextBox.Text += data;
            });
        }

        private void TorrentsDataGridUpdate()
        {
            torrentsDataGrid.Dispatcher.Invoke(() =>
            {
                var torrentsDownloadInfo = GetTorrentsDownloadInfo();
                var selectedIndex = torrentsDataGrid.SelectedIndex;
                torrentsDataGrid.ItemsSource = torrentsDownloadInfo;
                torrentsDataGrid.SelectedIndex = selectedIndex;
            });
        }

        private BEncodedDictionary TryLoadFastResumeFile()
        {
            try
            {
                if (File.Exists(fastResumeFilePath))
                    return BEncodedValue.Decode<BEncodedDictionary>(File.ReadAllBytes(fastResumeFilePath));
                return null;
            }
            catch
            {
                return null;
            }
        }

        public async void Setup()
        {
            int port = Port;
            var engineSettings = new EngineSettings();
            engineSettings.SavePath = downloadsPath;
            engineSettings.ListenPort = port;
            //engineSettings.GlobalMaxUploadSpeed = 30 * 1024;
            //engineSettings.GlobalMaxDownloadSpeed = 100 * 1024;
            //engineSettings.MaxReadRate = 1 * 1024 * 1024;

            engine = new ClientEngine(engineSettings);

            var nodes = Array.Empty<byte>();
            try
            {
                if (File.Exists(dhtNodeFile))
                    nodes = File.ReadAllBytes(dhtNodeFile);
            }
            catch
            {
                TextBoxWriteLine("No existing dht nodes could be loaded");
            }

            var dhtEngine = new DhtEngine(new IPEndPoint(IPAddress.Any, port));
            await engine.RegisterDhtAsync(dhtEngine);
            await engine.DhtEngine.StartAsync(nodes);

            if (!Directory.Exists(engine.Settings.SavePath))
                Directory.CreateDirectory(engine.Settings.SavePath);

            if (!Directory.Exists(torrentsPath))
                Directory.CreateDirectory(torrentsPath);
        }

        public async void AddTorrent(string torrentPath)
        {
            var fastResume = TryLoadFastResumeFile();
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
                    TextBoxWriteLine("Couldn't decode: " + torrentPath + " ");
                    TextBoxWriteLine(exception.Message);
                }

                var manager = new TorrentManager(torrent, downloadsPath, torrentDefaults);
                if (fastResume != null && fastResume.ContainsKey(torrent.InfoHash.ToHex()))
                    manager.LoadFastResume(new FastResume((BEncodedDictionary)fastResume[torrent.InfoHash.ToHex()]));
                await engine.Register(manager);

                torrentsManagers.Add(manager);

                TorrentsDataGridUpdate();
            }
        }

        public async Task StartEngine()
        {
            if (torrentsManagers.Count == 0)
            {
                TextBoxWriteLine("No torrents");
                TextBoxWriteLine("Exiting...");
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
                TextBoxWriteLine("No torrents");
                TextBoxWriteLine("Exiting...");
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

                TextBoxClear();
                TextBoxWriteLine(GetCommonInfo());
                TorrentsDataGridUpdate();

                Thread.Sleep(ShowInfoTimeInterval);
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
            File.WriteAllBytes(dhtNodeFile, nodes);
            File.WriteAllBytes(fastResumeFilePath, fastResume.Encode());
            engine.Dispose();

            Thread.Sleep(2000);
        }
    }
}

