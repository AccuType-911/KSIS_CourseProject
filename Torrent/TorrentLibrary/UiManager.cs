using System.Collections.Generic;
using System.Windows.Controls;

namespace TorrentLibrary
{
    public class UiManager
    {
        public TextBox commonInfoTextBox { get; private set; }
        public DataGrid torrentsDataGrid { get; private set; }

        public UiManager(TextBox textBox, DataGrid dataGrid)
        {
            commonInfoTextBox = textBox;
            torrentsDataGrid = dataGrid;
        }

        public void TextBoxClear()
        {
            commonInfoTextBox.Dispatcher.Invoke(() =>
            {
                commonInfoTextBox.Clear();
            });
        }

        public void TextBoxWriteLine(string data)
        {
            commonInfoTextBox.Dispatcher.Invoke(() =>
            {
                commonInfoTextBox.Text += data;
            });
        }

        public void TorrentsDataGridUpdate(List<TorrentDownloadInfo> torrentsDownloadInfo)
        {
            torrentsDataGrid.Dispatcher.Invoke(() =>
            {
                var selectedIndex = torrentsDataGrid.SelectedIndex;
                torrentsDataGrid.ItemsSource = torrentsDownloadInfo;
                torrentsDataGrid.SelectedIndex = selectedIndex;
            });
        }
    }
}
