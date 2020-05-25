using System;
using System.Windows;
using System.Windows.Input;
using TorrentLibrary;

namespace Torrent
{
    /// <summary>
    /// Логика взаимодействия для SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public GlobalSettings GlobalSettings { get; private set; }
        public bool? IsNoChangingExit { get; private set; }

        public SettingsWindow(GlobalSettings currentGlobalSettings)
        {
            InitializeComponent();
            GlobalSettings = currentGlobalSettings;
            ShowGlobalSettings();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            IsNoChangingExit = true;
            Close();
        }

        private void ShowGlobalSettings()
        {
            PortTextBox.Text = GlobalSettings.Port.ToString();
            DiskReadLimitTextBox.Text = GlobalSettings.MaxDiskReadSpeed.ToString();
            DiskWriteLimitTextBox.Text = GlobalSettings.MaxDiskWriteSpeed.ToString();
            DownloadSpeedLimitTextBox.Text = GlobalSettings.DownloadSpeedLimit.ToString();
            UploadSpeedLimitTextBox.Text = GlobalSettings.UploadSpeedLimit.ToString();
            MaxOpenConnectionsCountTextBox.Text = GlobalSettings.MaxOpenConnectionsCount.ToString();
        }

        private void SetGlobalSettingsObject()
        {
            GlobalSettings.Port = int.Parse(PortTextBox.Text);
            GlobalSettings.DownloadSpeedLimit = int.Parse(DownloadSpeedLimitTextBox.Text);
            GlobalSettings.UploadSpeedLimit = int.Parse(UploadSpeedLimitTextBox.Text);
            GlobalSettings.MaxDiskReadSpeed = int.Parse(DiskReadLimitTextBox.Text);
            GlobalSettings.MaxDiskWriteSpeed = int.Parse(DiskWriteLimitTextBox.Text);
            GlobalSettings.MaxOpenConnectionsCount = int.Parse(MaxOpenConnectionsCountTextBox.Text);
        }

        private bool CheckGlobalSettings()
        {
            return GlobalSettings.DownloadSpeedLimit > -1 &&
            GlobalSettings.UploadSpeedLimit > -1 &&
            GlobalSettings.MaxDiskReadSpeed > -1 &&
            GlobalSettings.MaxDiskWriteSpeed > -1 &&
            GlobalSettings.Port > 0 &&
            GlobalSettings.MaxOpenConnectionsCount > 0;
        }

        private void ConfirmSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SetGlobalSettingsObject();
                if (CheckGlobalSettings())
                {
                    IsNoChangingExit = false;
                    Close();
                }
                else
                {
                    MessageBox.Show("Значения должны быть положительные или 0!");
                }
            }
            catch
            {
                MessageBox.Show("Введите корректно все значения!");
            }
        }

        private void TextBoxesDigitsHandler(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
            }
        }
    }
}
