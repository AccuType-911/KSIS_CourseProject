using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TorrentLibrary;

namespace Torrent
{
    /// <summary>
    /// Логика взаимодействия для SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public GlobalSettings GlobalSettings { get; private set; }

        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ConfirmSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                GlobalSettings = new GlobalSettings();
                GlobalSettings.DownloadSpeedLimit = int.Parse(DownloadSpeedLimitTextBox.Text);
                GlobalSettings.UploadSpeedLimit = int.Parse(UploadSpeedLimitTextBox.Text);
                if (GlobalSettings.DownloadSpeedLimit > -1 && GlobalSettings.UploadSpeedLimit > -1)
                {
                    Close();
                }
                else
                {
                    MessageBox.Show("Значения должны быть положительные!");
                }
            }
            catch
            {
                MessageBox.Show("Введите корректно все значения!");
            }
        }

        private void DownloadSpeedLimitTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
            }
        }

        private void UploadSpeedLimitTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
            }
        }
    }
}
