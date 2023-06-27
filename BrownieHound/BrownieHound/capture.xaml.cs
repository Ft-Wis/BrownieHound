
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
using System.IO;

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
        List<DispatcherTimer> detectTimerList = new List<DispatcherTimer>();
        DispatcherTimer clockTimer;
        int clock = 0;
        //１秒単位の経過時間
        List<int> countRows = new List<int>();
        //秒数毎のCDataのカウント
        List<List<List<int>>> detectionNumbers = new List<List<List<int>>>();
        //検出したキャプチャデータのナンバーをルールに対応付けて格納
        //これを基に検知画面に表示したい
        Window dWindow;
        string path = @"conf";
        List<string> ruleGroupNames = new List<string>();
        List<ruleGroupData> detectionRuleGroups = new List<ruleGroupData>();


        public capture(string tsINumber)
        {
            InitializeComponent();
            CaputureData.ItemsSource = CData;
            CData = new ObservableCollection<packetData>();
            this.tsInterfaceNumber = tsINumber;

            dWindow = new detectWindow();
            dWindow.Show();
        }
        public capture(string tsINumber,List<ruleGroupData> detectionRuleGroups)
        {
            InitializeComponent();
            CaputureData.ItemsSource = CData;
            CData = new ObservableCollection<packetData>();
            this.tsInterfaceNumber = tsINumber;
            this.detectionRuleGroups = detectionRuleGroups;
            dWindow = new detectWindow();
            dWindow.Show();
        }

        private void inactivate_Click(object sender, RoutedEventArgs e)
        {
            closing();
            
        }
        bool stopflag = false;
        private void stop_Click(object sende, RoutedEventArgs e) 
        {
            processTscap.Kill();
            stopflag = true;
            foreach (var detectTimer in detectTimerList)
            {
                detectTimer.Stop();
            }

        }
       
        private void tsStart(string tsDirectory, string args)
        {
            processTscap = new Process();
            ProcessStartInfo processSinfo = new ProcessStartInfo($@"{tsDirectory}\tshark.exe", args);
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

            string tsDirectory = "";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);

            }
            if (!File.Exists(@$"{path}\path.conf"))
            {
                using (StreamWriter sw = new StreamWriter(@$"{path}\path.conf", false, Encoding.GetEncoding("UTF-8")))
                {
                    sw.WriteLine(@"C:\Program Files\Wireshark");
                }
            }
            using (StreamReader sr = new StreamReader(@$"{path}\path.conf", Encoding.GetEncoding("UTF-8")))
            {
                tsDirectory = sr.ReadLine();
            }

            string args = $"-i {tsInterfaceNumber} -T ek";

            countRows.Add(0);
            foreach (var detectionRuleGroup in detectionRuleGroups.Select((Value, Index) => new {Value,Index }))
            {
                ruleGroupNames.Add(detectionRuleGroup.Value.Name);
                detectionNumbers.Add(new List<List<int>>());
                //一番上位の要素を格納　ルールグループの数
                ruleGroupDataSplit(detectionRuleGroup.Value, detectionRuleGroup.Index);
            }


            tsStart(tsDirectory, args);

            clockTimer = new DispatcherTimer();
            clockTimer.Interval = new TimeSpan(0, 0, 1);
            clockTimer.Tick += new EventHandler(recordTime);
            clockTimer.Start();
            for(int i = 0;i<detectTimerList.Count;i++)
            {
                detectTimerList[i].Start();
            }

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
        private void detectLogic(int start,int end,ruleData rule,int detectionNumber)
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
                    if (!detectionNumbers[detectionNumber][rule.ruleNo].Contains(targets[i]))
                    {
                        detectionNumbers[detectionNumber][rule.ruleNo].Add(targets[i]);

                        //以下テスト用
                        Debug.WriteLine(ruleGroupNames[detectionNumber] +"::" + rule.ruleNo);
                        //Debug.WriteLine(detectionRuleGroups[detectionNumber].ruleDatas[rule.ruleNo].Source +"::" + detectionRuleGroups[detectionNumber].ruleDatas[rule.ruleNo].Protocol);
                        Debug.WriteLine(CData[targets[i]].Source + "::" + CData[targets[i]].Protocol);
                    }
                }
            }
        }
        private void ruleGroupDataSplit(ruleGroupData ruleGroup,int detectionNumber)
        {
            foreach(ruleData rule in ruleGroup.ruleDatas)
            {
                detectionNumbers[detectionNumber].Add(new List<int>());
                //2次要素を格納　ルールグループの中のルールの数
                detectionSet(rule,detectionNumber);
            }
        }

        private void detectionSet(ruleData rule,int detectionNumber)
        {
            DispatcherTimer detectTimer = new DispatcherTimer();
            detectTimer.Interval = new TimeSpan(0, 0, 1);
            detectTimer.Tick += new EventHandler(detection);
            detectTimerList.Add(detectTimer);
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
                    detectLogic(start, end, rule,detectionNumber);

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
                
                //CData.Add(new packetData(msg));
                //errなどはそのまま出力する
                if (stopflag==true)
                {
                    stopstatus.Content = "中断中";
                }
                else
                {
                    CData.Add(new packetData(msg));
                }
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
            foreach(var detectTimer in detectTimerList)
            {
                detectTimer.Stop();
            }
            Application.Current.Shutdown();

        }
        private void Grid_Unloaded(object sender, RoutedEventArgs e)
        {
            closing();
        }
    }
}
