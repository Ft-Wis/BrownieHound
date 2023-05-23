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
using System.Printing;

namespace BrownieHound
{
    /// <summary>
    /// capture.xaml の相互作用ロジック
    /// </summary>
    public partial class capture : Page
    {
        public class packetData
        {
            public int Number { get; set; }
            public string time { get; set; }
            public string Source { get; set; }
            public string Destination { get; set; }
            public string Protocol { get; set; }
            public int Length { get; set; }
            public string Info { get; set; }

            public void packetSplit(string msg)
            {
                string[] data = msg.Trim().Split(' ');
                int i = 0;
                if (Int32.TryParse(data[i], out int num)) {
                    Number = num;
                    time = data[++i];
                    while (data[++i] == "");
                    Source = data[i];
                    Destination = data[i += 2];
                    while (data[++i] == "");
                    Protocol = data[i++];
                    if (Int32.TryParse(data[i],out int length))
                    {
                        Length = length;
                        i++;
                    }
                }

                for(; i < data.Length; i++)
                {
                    Info += $" {data[i]}";
                }

            }
            public packetData(string msg)
            {
                packetSplit(msg);
            }
        }
        Process processTscap = null;
        string tsInterfaceNumber = "";
        private ObservableCollection<packetData> CData;
        public capture(string tsINumber)
        {
            InitializeComponent();
            CaputureData.ItemsSource = CData;
            CData = new ObservableCollection<packetData> { };
            this.tsInterfaceNumber = tsINumber;
        }
        
        private void inactivate_Click(object sender, RoutedEventArgs e)
        {
            closing();
        }
        private void tsStart(string Command, string args)
        {
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
        private void Page_loaded(object sender, RoutedEventArgs e)
        {
            string Command = "C:\\Program Files\\Wireshark\\tshark.exe";

            string args = $"-i {tsInterfaceNumber} -t a";
            //オプションとしてテスト用に固定値を指定
            tsStart(Command, args);

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
            packetData pd = new packetData(msg);
            CData.Add(pd);
            CaputureData.ItemsSource = CData;
            CaputureData.ScrollIntoView(CaputureData.Items.GetItemAt(CaputureData.Items.Count - 1));
            
        }
        private void PrintTextBoxByThread(string msg)
        {
            Dispatcher.Invoke(new Action(() => PrintText(msg)));
        }
        private void closing()
        {
            if (processTscap != null && !processTscap.HasExited)
            {
                processTscap.Kill();
            }
            Application.Current.Shutdown();

        }
        private void Grid_Unloaded(object sender, RoutedEventArgs e)
        {
            closing();
        }
    }
}
