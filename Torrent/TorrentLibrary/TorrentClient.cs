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

        private TextBox commonInfoTextBox;
        private DataGrid torrentsDataGrid;

        private string basePath;
        private string downloadsPath;
        //private string fastResumeFilePath;
        private string dhtNodeFile;
        private ClientEngine engine;
        private List<TorrentManager> torrents;
        private Top10Listener listener;

        public TorrentClient(TextBox textBox, DataGrid dataGrid)
        {
            commonInfoTextBox = textBox;
            torrentsDataGrid = dataGrid;
            basePath = Environment.CurrentDirectory;
            downloadsPath = Path.Combine(basePath, "Downloads");
            //fastResumeFilePath = Path.Combine(torrentsPath, "fastresume.data");
            dhtNodeFile = Path.Combine(basePath, "DhtNodes");
            torrents = new List<TorrentManager>();
            listener = new Top10Listener(10);

            AppDomain.CurrentDomain.ProcessExit += delegate { Shutdown().Wait(); };
            AppDomain.CurrentDomain.UnhandledException += delegate (object sender, UnhandledExceptionEventArgs e) { Console.WriteLine(e.ExceptionObject); Shutdown().Wait(); };
            Thread.GetDomain().UnhandledException += delegate (object sender, UnhandledExceptionEventArgs e) { Console.WriteLine(e.ExceptionObject); Shutdown().Wait(); };
            Setup();
        }

        private void ClearText()
        {
            commonInfoTextBox.Clear();
        }

        private void WriteLine(string data)
        {
            commonInfoTextBox.Text += data;
        }

        private void SetTorrentsDataGridContent(List<TorrentDownloadInfo> torrentsDownloadInfo)
        {
            torrentsDataGrid.ItemsSource = torrentsDownloadInfo;
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

            // Create an instance of the engine.
            engine = new ClientEngine(engineSettings);

            var nodes = Array.Empty<byte>();
            try
            {
                if (File.Exists(dhtNodeFile))
                    nodes = File.ReadAllBytes(dhtNodeFile);
            }
            catch
            {
                WriteLine("No existing dht nodes could be loaded");
            }

            var dhtEngine = new DhtEngine(new IPEndPoint(IPAddress.Any, port));
            await engine.RegisterDhtAsync(dhtEngine);
            await engine.DhtEngine.StartAsync(nodes);

            if (!Directory.Exists(engine.Settings.SavePath))
                Directory.CreateDirectory(engine.Settings.SavePath);

            /*BEncodedDictionary fastResume = new BEncodedDictionary();
            try
            {
                if (File.Exists(fastResumeFilePath))
                    fastResume = BEncodedValue.Decode<BEncodedDictionary>(File.ReadAllBytes(fastResumeFilePath));
            }
            catch                   ПОКА БЕЗ ВОЗМОЖНОСТИ ПАУЗЫ И ВОЗОБНОВЛЕНИЯ
            {
            }*/
        }

        public async void AddTorrent(string torrentPath)
        {
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
                    WriteLine("Couldn't decode: " + torrentPath + " ");
                    WriteLine(exception.Message);
                }

                var manager = new TorrentManager(torrent, downloadsPath, torrentDefaults);
                await engine.Register(manager);

                torrents.Add(manager);
                manager.PeersFound += Manager_PeersFound;
            }
        }

        public async Task StartEngine()
        {
            if (torrents.Count == 0)
            {
                WriteLine("No torrents found in the Torrents directory");
                WriteLine("Exiting...");
                engine.Dispose();
                return;
            }

            foreach (var manager in torrents)  // TODO: ПЕРЕДЕЛАТЬ ПОД 1 ТОРРЕНТ, А НЕ СПИСОК
            {
                manager.PeerConnected += (o, e) =>
                {
                    lock (listener)
                        listener.WriteLine($"Connection succeeded: {e.Peer.Uri}");
                };
                manager.ConnectionAttemptFailed += (o, e) =>
                {
                    lock (listener)
                        listener.WriteLine($"Connection failed: {e.Peer.ConnectionUri} - {e.Reason} - {e.Peer.AllowedEncryption}");
                };
                manager.PieceHashed += delegate (object o, PieceHashedEventArgs e)
                {
                    lock (listener)
                        listener.WriteLine($"Piece Hashed: {e.PieceIndex} - {(e.HashPassed ? "Pass" : "Fail")}");
                };
                manager.TorrentStateChanged += delegate (object o, TorrentStateChangedEventArgs e)
                {
                    lock (listener)
                        listener.WriteLine($"OldState: {e.OldState} NewState: {e.NewState}");
                };
                manager.TrackerManager.AnnounceComplete += (sender, e) =>
                {
                    listener.WriteLine($"{e.Successful}: {e.Tracker}");
                };
                await manager.StartAsync();
            }

            await engine.EnablePortForwardingAsync(CancellationToken.None);

            ShowDownloadInfo(torrents);

            await engine.DisablePortForwardingAsync(CancellationToken.None);
        }

        private string GetCommonInfo()
        {
            var commonInfoStringBuilder = new StringBuilder(1024);
            AppendFormat(commonInfoStringBuilder, "Total Download Rate: {0:0.00}kB/sec", engine.TotalDownloadSpeed / 1024.0);
            AppendFormat(commonInfoStringBuilder, "Total Upload Rate:   {0:0.00}kB/sec", engine.TotalUploadSpeed / 1024.0);
            AppendFormat(commonInfoStringBuilder, "Disk Read Rate:      {0:0.00} kB/s", engine.DiskManager.ReadRate / 1024.0);
            AppendFormat(commonInfoStringBuilder, "Disk Write Rate:     {0:0.00} kB/s", engine.DiskManager.WriteRate / 1024.0);
            AppendFormat(commonInfoStringBuilder, "Total Read:         {0:0.00} kB", engine.DiskManager.TotalRead / 1024.0);
            AppendFormat(commonInfoStringBuilder, "Total Written:      {0:0.00} kB", engine.DiskManager.TotalWritten / 1024.0);
            AppendFormat(commonInfoStringBuilder, "Open Connections:    {0}", engine.ConnectionManager.OpenConnections);
            return commonInfoStringBuilder.ToString();
        }

        private TorrentDownloadInfo GetTorrentDownloadInfo(TorrentManager manager)
        {
            var downloadInfo = new TorrentDownloadInfo();
            downloadInfo.State = manager.State;
            downloadInfo.Name = manager.Torrent == null ? "MetaDataMode" : manager.Torrent.Name;
            downloadInfo.Progress = manager.Progress;
            downloadInfo.DownloadSpeed = manager.Monitor.DownloadSpeed / 1024.0;
            downloadInfo.UploadSpeed = manager.Monitor.UploadSpeed / 1024.0;
            downloadInfo.DataBytesDownloaded = manager.Monitor.DataBytesDownloaded / (1024.0 * 1024.0);
            downloadInfo.DataBytesUploaded = manager.Monitor.DataBytesUploaded / (1024.0 * 1024.0);
            return downloadInfo;
        }

        private void ShowDownloadInfo(List<TorrentManager> torrents)
        {
            bool isRunning = true;
            while (isRunning)
            {
                isRunning = torrents.Exists(manager => manager.State != TorrentState.Stopped);

                var commonInfo = GetCommonInfo();
                var torrentsDownloadInfo = new List<TorrentDownloadInfo>();
                foreach (var manager in torrents)
                {
                    var torrentDownloadInfo = GetTorrentDownloadInfo(manager);
                    torrentsDownloadInfo.Add(torrentDownloadInfo);
                }


                commonInfoTextBox.Dispatcher.Invoke(new Action(ClearText));
                commonInfoTextBox.Dispatcher.Invoke(new Action<string>(WriteLine), commonInfo);
                torrentsDataGrid.Dispatcher.Invoke(new Action<List<TorrentDownloadInfo>>(SetTorrentsDataGridContent), torrentsDownloadInfo);
                //listener.ExportTo(Console.Out);

                Thread.Sleep(5000);
            }
        }

        private void Manager_PeersFound(object sender, PeersAddedEventArgs e)
        {
            lock (listener)
                listener.WriteLine($"Found {e.NewPeers} new peers and {e.ExistingPeers} existing peers");//throw new Exception("The method or operation is not implemented.");
        }

        private void AppendSeparator(StringBuilder stringBuilder)
        {
            AppendFormat(stringBuilder, "", null);
            AppendFormat(stringBuilder, "- - - - - - - - - - - - - - - - - - - - - - - - - - - - - -", null);
            AppendFormat(stringBuilder, "", null);
        }

        private void AppendFormat(StringBuilder stringBuilder, string str, params object[] formatting)
        {
            if (formatting != null)
                stringBuilder.AppendFormat(str, formatting);
            else
                stringBuilder.Append(str);
            stringBuilder.AppendLine();
        }

        private async Task Shutdown()
        {
            var fastResume = new BEncodedDictionary();
            for (var i = 0; i < torrents.Count; i++)
            {
                var stoppingTask = torrents[i].StopAsync();
                while (torrents[i].State != TorrentState.Stopped)
                {
                    WriteLine(torrents[i].Torrent.Name + " is " + torrents[i].State);
                    Thread.Sleep(250);
                }
                await stoppingTask;

                if (torrents[i].HashChecked)
                    fastResume.Add(torrents[i].Torrent.InfoHash.ToHex(), torrents[i].SaveFastResume().Encode());
            }

            var nodes = await engine.DhtEngine.SaveNodesAsync();
            File.WriteAllBytes(dhtNodeFile, nodes);
            //File.WriteAllBytes(fastResumeFilePath, fastResume.Encode());
            engine.Dispose();

            Thread.Sleep(2000);
        }
    }
}

