using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Collections.ObjectModel;

namespace BrownieHound
{
    /// <summary>
    /// capture.xaml の相互作用ロジック
    /// </summary>
    public partial class capture : Page
    {
        public class packetData
        {
            public string Data { get; set; }
            public packetData(string msg)
            {
                Data = msg;
            }
        }
        Process processTscap = null;
        private ObservableCollection<packetData> CData;
        public capture()
        {
            InitializeComponent();
            CaputureData.ItemsSource = CData;
            CData = new ObservableCollection<packetData> { };
        }
        
        private void inactivate_Click(object sender, RoutedEventArgs e)
        {
            if (processTscap != null && !processTscap.HasExited)
            {
                processTscap.Kill();
            }
            Application.Current.Shutdown();

        }

        private void Page_loaded(object sender, RoutedEventArgs e)
        {
            string Command = "C:\\Program Files\\Wireshark\\tshark.exe";

            string args = "-i 5";
            //オプションとしてテスト用に固定値を指定
            processTscap = new Process();
            ProcessStartInfo processSinfo = new ProcessStartInfo(Command, args);
            processSinfo.CreateNoWindow = true;
            processSinfo.UseShellExecute = false;
            processSinfo.RedirectStandardOutput = true;
            processSinfo.RedirectStandardError = true;

            processSinfo.StandardErrorEncoding = Encoding.UTF8;
            processSinfo.StandardOutputEncoding = Encoding.UTF8;

            processTscap = Process.Start(processSinfo);

            processTscap.OutputDataReceived += dataReceived;
            processTscap.ErrorDataReceived += errReceived;

            processTscap.BeginErrorReadLine();
            processTscap.BeginOutputReadLine();
        }

        private void errReceived(object sender, DataReceivedEventArgs e)
        {
            string packetText = e.Data;
            if (packetText != null && packetText.Length > 0)
            {
                PrintTextBoxByThread("ERR:" + packetText);
            }
        }

        private void dataReceived(object sender, DataReceivedEventArgs e)
        {
            string packetText = e.Data;
            if (packetText != null && packetText.Length > 0)
            {
                PrintTextBoxByThread(packetText);
            }
        }
        private void PrintText(string msg)
        {
            //CData.Add(new packetData(msg));
            //CaputureData.ItemsSource = CData;
            //CaputureData.ScrollIntoView(CaputureData.Items.GetItemAt(CaputureData.Items.Count - 1));
            bool isRowSelected = CaputureData.SelectedItems.Count > 0;

            CData.Add(new packetData(msg));
            CaputureData.ItemsSource = CData;

            if (!isRowSelected)
            {
                CaputureData.ScrollIntoView(CaputureData.Items.GetItemAt(CaputureData.Items.Count - 1));
            }


        }
        private void chaptureDataGrid_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)sender;
            ScrollViewer scrollViewer = GetScrollViewer(dataGrid);

            if (scrollViewer.VerticalOffset + scrollViewer.ViewportHeight >= scrollViewer.ExtentHeight)
            {
                dataGrid.UnselectAll(); // DataGrid自体から選択を解除する場合
            }
        }

        private ScrollViewer GetScrollViewer(DependencyObject depObj)
        {
            if (depObj is ScrollViewer viewer)
                return viewer;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);
                var result = GetScrollViewer(child);
                if (result != null)
                    return result;
            }

            return null;
        }
        private void PrintTextBoxByThread(string msg)
        {
            Dispatcher.Invoke(new Action(() => PrintText(msg)));
        }
    }
}
