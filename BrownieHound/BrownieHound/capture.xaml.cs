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
using System.Windows.Interop;
using System.Windows.Threading;
using System.Globalization;
using System.Text.Json.Serialization;
using System.Xml.Linq;
using System.Xml;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using static BrownieHound.App;

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
            public DateTime Time { get; set; }
            public string Source { get; set; }
            public string Destination { get; set; }
            public string sourcePort { get; set; }
            public string destinationPort { get; set; }
            public string Protocol { get; set; }
            public int frameLength { get; set; }
            public string Info { get; set; }
            public string Data { get; set; }
            List<string> protocols = new List<string>();

            public packetData(String err)
            {
                Info = err;
                
            }
            public packetData(JObject layersObject)
            {
                Data = JsonConvert.SerializeObject(layersObject, Newtonsoft.Json.Formatting.None);
                foreach (var layer in layersObject)
                {
                    protocols.Add(layer.Key.ToString());
                    //Debug.WriteLine(layer.Key);
                }
                Number = Int32.Parse((string)layersObject[protocols[0]][$"{protocols[0]}_{protocols[0]}_number"]);
                //frame_frame_number

                string caputureTime = (string)layersObject[protocols[0]][$"{protocols[0]}_{protocols[0]}_time"];
                caputureTime = caputureTime.Substring(0, 27);
                //精度が高すぎるので落とす

                Time = DateTime.ParseExact(caputureTime, "yyyy-MM-dd'T'HH:mm:ss.FFFFFFF", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                Time = Time.AddHours(9);


                string eSource = (string)layersObject[protocols[1]][$"{protocols[1]}_{protocols[1]}_src"];
                string eDestination = (string)layersObject[protocols[1]][$"{protocols[1]}_{protocols[1]}_dst"];
                //ethレベルのアドレス（MACアドレス）

                Source = (string)layersObject[protocols[2]][$"{protocols[2]}_{protocols[2]}_src"];
                Destination = (string)layersObject[protocols[2]][$"{protocols[2]}_{protocols[2]}_dst"];

                if (Source == null)
                {
                    Source = eSource;
                    Destination = eDestination;
                }
                if (protocols.Contains("tcp") || protocols.Contains("udp"))
                {
                    sourcePort = (string)layersObject[protocols[3]][$"{protocols[3]}_{protocols[3]}_srcport"];
                    destinationPort = (string)layersObject[protocols[3]][$"{protocols[3]}_{protocols[3]}_dstport"];
                    Info += $"{sourcePort} → {destinationPort}";
                }

                if (protocols.Last().Equals("data"))
                {
                    Protocol = protocols[protocols.Count - 2];
                }
                else
                {
                    Protocol = protocols.Last();
                }

                frameLength = Int32.Parse((string)layersObject[protocols[0]][$"{protocols[0]}_{protocols[0]}_len"]);
                Info += $" {protocols.Last()}";
                //Debug.WriteLine($"{Number} : {time.TimeOfDay} : {Source} : {Destination} : {Protocol} : {Length} :: {Info}");

            }

        }

        Process processTscap = null;
        string tsInterfaceNumber = "";
        private ObservableCollection<packetData> CData;
        DispatcherTimer detectTimer;
        DispatcherTimer clockTimer;
        int clock = 0;
        //１秒単位の経過時間
        List<int> countRows = new List<int>();
        //秒数毎のCDataのカウント
        List<List<List<int>>> detectionNumber = new List<List<List<int>>>();
        //検出したキャプチャデータのナンバーをルールに対応付けて格納
        //これを基に検知画面に表示したい
        Window dwindow;


        public capture(string tsINumber)
        {
            InitializeComponent();
            CaputureData.ItemsSource = CData;
            CData = new ObservableCollection<packetData> { };
            this.tsInterfaceNumber = tsINumber;

            Window dwindow = new detectWindow();
            dwindow.Show();
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

            string args = $"-i {tsInterfaceNumber} -T ek";

            countRows.Add(0);
            for(int i = 0;i < 10; i++)
            {
                detectionNumber.Add(new List<List<int>>());
            }
            for(int i = 0;i < 10; i++)
            {
                detectionNumber[i].Add(new List<int>());
            }

            tsStart(Command, args);
            ruleData rule = new ruleData("0,0,60,1,8.8.8.8,,,,,0");
            //固定値のルールセットを渡す

            clockTimer = new DispatcherTimer();
            clockTimer.Interval = new TimeSpan(0, 0, 1);
            clockTimer.Tick += new EventHandler(recordTime);
            clockTimer.Start();
            dtStart(rule);

        }

        private void recordTime(object sender,EventArgs e)
        {
            int countNumber = CData.Count;
            if(countNumber > 0)
            {
                countNumber -= 1;
            }
            clock++;
            countRows.Add(countNumber);
        }
        private void detectLogic(int start,int end,ruleData rule)
        {
            List<int> targets = new List<int>();
            for (int i = start; i <= end; i++)
            {
                int flg = 0;
                if (rule.Source == "" || rule.Source.Equals(CData[i].Source))
                {
                    flg++;
                }
                if (rule.Destination == "" || rule.Destination.Equals(CData[i].Destination))
                {
                    flg++;
                }
                if (rule.Protocol == "" || rule.Protocol.Equals(CData[i].Protocol))
                {
                    flg++;
                }
                if(rule.sourcePort == "" || rule.sourcePort.Equals(CData[i].sourcePort))
                {
                    flg++;
                }
                if(rule.destinationPort == "" || rule.destinationPort.Equals(CData[i].destinationPort))
                {
                    flg++;
                }
                if (CData[i].frameLength > rule.frameLength)
                {
                    flg++;
                }
                if (flg == 6)
                {
                    targets.Add(i);
                }
            }
            if (targets.Count >= rule.detectionCount)
            {
                for (int i = 0; i < targets.Count; i++)
                {
                    if (!detectionNumber[rule.ruleGroupNo][rule.ruleNo].Contains(targets[i]))
                    {
                        detectionNumber[rule.ruleGroupNo][rule.ruleNo].Add(targets[i]);
                    }
                }
            }
        }
        private void dtStart(ruleData rule)
        {
            detectTimer = new DispatcherTimer();
            detectTimer.Interval = new TimeSpan(0, 0, 1);
            detectTimer.Tick += new EventHandler(detection);
            detectTimer.Start();
            void detection(object sender, EventArgs e)
            {
                if (clock >= rule.detectionInterval)
                {
                    int start = countRows[clock - rule.detectionInterval];
                    int end = countRows[clock] - 1;
                    //想定としてインターバルは秒指定
                    if (countRows[clock] == countRows[clock - 1])
                    {
                        end++;
                    }
                    if (clock > rule.detectionInterval && start == countRows[clock - rule.detectionInterval - 1] && start < end)
                    {
                        //検知する範囲内で出現したパケットのみを対象とする処理
                        //0,0,2,2,3...等の時に２回目の試行には0を入れたくない
                        start++;
                    }
                    detectLogic(start, end, rule);

                }
            }
        }



        private void errReceived(object sender, DataReceivedEventArgs e)
        {
            string packetText = e.Data;
            if (packetText != null && packetText.Length > 0)
            {
                PrintpacketByThread("ERR:" + packetText);
            }
        }

        private void dataReceived(object sender, DataReceivedEventArgs e)
        {
            string packetText = e.Data;
            if (packetText != null && packetText.Length > 0)
            {
                PrintpacketByThread(packetText);
            }
        }
        private void Printpacket(string msg)
        {
            try
            {
                JObject packetObject = JObject.Parse(msg);
                if (packetObject["layers"] != null)
                {
                    packetData pd = new packetData((JObject)packetObject["layers"]);
                    CData.Add(pd);
                }
            }
            catch
            {
                CData.Add(new packetData(msg));
                //errなどはそのまま出力する
            }
            bool isRowSelected = CaputureData.SelectedItems.Count > 0;
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
        private void PrintpacketByThread(string msg)
        {
            Dispatcher.Invoke(new Action(() => Printpacket(msg)));
        }
        private void closing()
        {
            if (processTscap != null && !processTscap.HasExited)
            {
                processTscap.Kill();
            }
            clockTimer.Stop();
            detectTimer.Stop();
            Application.Current.Shutdown();

        }
        private void Grid_Unloaded(object sender, RoutedEventArgs e)
        {
            closing();
        }
    }
}
