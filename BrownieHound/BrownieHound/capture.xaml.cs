
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
using MimeKit;
using MailKit.Net.Smtp;
using MailKit;

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
                Protocol = Protocol.ToUpper();

                frameLength = Int32.Parse((string)layersObject[protocols[0]][$"{protocols[0]}_{protocols[0]}_len"]);
                Info += $" {protocols.Last()}";
                //Debug.WriteLine($"{Number} : {time.TimeOfDay} : {Source} : {Destination} : {Protocol} : {Length} :: {Info}");

            }

        }

        Process processTscap = null;
        string tsInterfaceNumber = "";
        List<DispatcherTimer> detectTimerList = new List<DispatcherTimer>();
        DispatcherTimer clockTimer;
        DispatcherTimer mailTimer;
        int clock = 0;
        //１秒単位の経過時間
        List<int> countRows = new List<int>();
        //秒数毎のCDataのカウント
        List<List<List<int>>> detectionNumbers = new List<List<List<int>>>();
        //検出したキャプチャデータのナンバーをルールに対応付けて格納
        //これを基に検知画面に表示したい
        detectWindow dWindow;
        string path = @"conf";
        List<ruleGroupData> detectionRuleGroups = new List<ruleGroupData>();
        string mailAddress = null;
        List<List<int>> streamStart = new List<List<int>>();


        public capture(string tsINumber)
        {
            InitializeComponent();
            this.tsInterfaceNumber = tsINumber;

        }
        public capture(string tsINumber,List<ruleGroupData> detectionRuleGroups)
        {
            InitializeComponent();
            this.tsInterfaceNumber = tsINumber;
            this.detectionRuleGroups = detectionRuleGroups;
            dWindow = new detectWindow(detectionRuleGroups);
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
            if (mailTimer != null)
            {
                mailTimer.Stop();
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
            if (dWindow != null && File.Exists(@$"{path}\mail.conf"))
            {
                Mail_Validation mailValidation = new Mail_Validation();
                using (StreamReader sr = new StreamReader(@$"{path}\mail.conf", Encoding.GetEncoding("UTF-8")))
                {

                    if (bool.TryParse(sr.ReadLine().Split(":")[1], out var isEnabled))
                    {
                        mailValidation.isEnabled.Value = isEnabled;
                    }
                    if (int.TryParse(sr.ReadLine().Split(":")[1], out var span))
                    {
                        mailValidation.span.Value = span.ToString();
                    }
                    mailValidation.mailAddress.Value = sr.ReadLine().Split(":")[1];
                    string authorized = sr.ReadLine().Split(":")[1];
                    if (mailValidation.isEnabled.Value && authorized.Equals("Authorized"))
                    {
                        if ((mailValidation.span.Value != "" && !mailValidation.span.HasErrors) && (mailValidation.mailAddress.Value != "" && !mailValidation.mailAddress.HasErrors))
                        {
                            mailAddress = mailValidation.mailAddress.Value;
                            mailTimer = new DispatcherTimer();
                            mailTimer.Interval = new TimeSpan(0, int.Parse(mailValidation.span.Value), 0);
                            mailTimer.Tick += new EventHandler(mailSend);
                            mailTimer.Start();
                            
                        }

                    }
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
                streamStart.Add(new List<int>());
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

        private void mailSend(object sender, EventArgs e)
        {
            _ = SendEmailNew();
        }
        private async Task SendEmailNew()
        {
            var email = new MimeMessage();
            int addCount;
            email.From.Add(new MailboxAddress("browniehound", "browniehound2024@gmail.com"));
            email.To.Add(new MailboxAddress("", mailAddress));
            packetData pd = (packetData)CaptureData.Items[CaptureData.Items.Count - 1];
            email.Subject = "userの定期検知メール";
            var body = new BodyBuilder();
            body.HtmlBody = $"<html><body><h1>userの定期検知メール</h1><br>";
            body.HtmlBody += $"<p><b>総キャプチャ数：{pd.Number}</b></p>";
            

            for(int i = 0;i < detectionRuleGroups.Count;i++)
            {
                
                body.HtmlBody += $"<h2>{detectionRuleGroups[i].Name}</h2>";
                for(int j = 0; j < detectionRuleGroups[i].ruleDatas.Count;j++)
                {
                    addCount = 0;
                    body.HtmlBody += $"<table border='1' style='margin-left:1%;border-collapse: collapse;border-color: thistle;width:98%;'><thead style='background-color:rgb(255, 179, 0);color:rgb(226, 247, 250);'><tr><th style='min-width:3em;'>No</th><th style='min-width:8em;'>Time</th><th style='min-width:4em;'>間隔(s)</th><th style='min-width:2em;'>頻度</th><th style='min-width:18em;'>Source</th><th style='min-width:18em;'>Destination</th><th style='min-width:5em;'>Protocol</th><th style='min-width:6em;'>sourcePort</th><th style='min-width:5em;'>destPort</th><th style='min-width:4em;'>Length</th></tr></thead>";
                    body.HtmlBody += $"<thead style='background-color:rgb(255, 179, 0);color:rgb(226, 247, 250);'><tr><th>{detectionRuleGroups[i].ruleDatas[j].ruleNo}</th><th>0</th><th>{detectionRuleGroups[i].ruleDatas[j].detectionInterval}</th><th>{detectionRuleGroups[i].ruleDatas[j].detectionCount}</th><th>{detectionRuleGroups[i].ruleDatas[j].Source}</th><th>{detectionRuleGroups[i].ruleDatas[j].Destination}</th><th>{detectionRuleGroups[i].ruleDatas[j].Protocol}</th><th>{detectionRuleGroups[i].ruleDatas[j].sourcePort}</th><th>{detectionRuleGroups[i].ruleDatas[j].destinationPort}</th><th>{detectionRuleGroups[i].ruleDatas[j].frameLength}</th></tr></thead>";
                    while(streamStart[i][j] < dWindow.detectionDatas[i].children[j].children.Count)
                    {
                        addCount++;
                        int k = streamStart[i][j];
                        packetData detectionPacketData = dWindow.detectionDatas[i].children[j].children[k].packet;
                        body.HtmlBody += $"<tbody style='background-color: blanchedalmond;'><tr><td>{detectionPacketData.Number}</td><td>{detectionPacketData.Time.TimeOfDay}</td><td></td><td></td><td>{detectionPacketData.Source}</td><td>{detectionPacketData.Destination}</td><td>{detectionPacketData.Protocol}</td><td>{detectionPacketData.sourcePort}</td><td>{detectionPacketData.destinationPort}</td><td>{detectionPacketData.frameLength}</td></tr></tbody>";
                        streamStart[i][j]++;
                    }
                    body.HtmlBody += "</table><br>";
                    body.HtmlBody += $"<p><b>検知増分：{addCount}</b></p>";
                }
                

            }
            body.HtmlBody += "</body></html>";

            email.Body = body.ToMessageBody();

            using (var smtp = new SmtpClient())
            {
                await smtp.ConnectAsync("smtp.gmail.com", 587, false);
                await smtp.AuthenticateAsync("browniehound2024", "eszyyyyhrwarlsns");
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
            }
        }

        private void recordTime(object sender,EventArgs e)
        {
            int countNumber = CaptureData.Items.Count;
            if(countNumber > 0)
            {
                countNumber -= 1;
            }
            clock++;
            countRows.Add(countNumber);
        }
        private void detectLogic(int detectionNumber)
        {
            List<int> targets = new List<int>();
            foreach (ruleData detectionRule in detectionRuleGroups[detectionNumber].ruleDatas)
            {
                List<int> temp = new List<int>();
                if (detectionRule.detectionInterval <= clock)
                {
                    int start = countRows[clock - detectionRule.detectionInterval];
                    int end = countRows[clock] - 1;
                    //if (countRows[clock] == countRows[clock - 1])
                    //{
                    //    end++;
                    //}
                    if (clock > detectionRule.detectionInterval && start == countRows[clock - detectionRule.detectionInterval - 1] && start < end)
                    {
                        //検知する範囲内で出現したパケットのみを対象とする処理
                        //0,0,2,2,3...等の時に２回目の試行には0を入れたくない
                        start++;
                    }
                    for (int i = start; i <= end; i++)
                    {
                        packetData packet = (packetData)CaptureData.Items[i];
                        int flg = 0;
                        if (detectionRule.Source.Equals("all") || detectionRule.Source.Equals(packet.Source))
                        {
                            flg++;
                        }
                        if (detectionRule.Destination.Equals("all") || detectionRule.Destination.Equals(packet.Destination))
                        {
                            flg++;
                        }
                        if (detectionRule.Protocol.Equals("all") || detectionRule.Protocol.Equals(packet.Protocol))
                        {
                            flg++;
                        }
                        if (detectionRule.sourcePort.Equals("all") || detectionRule.sourcePort.Equals(packet.sourcePort))
                        {
                            flg++;
                        }
                        if (detectionRule.destinationPort.Equals("all") || detectionRule.destinationPort.Equals(packet.destinationPort))
                        {
                            flg++;
                        }
                        if (packet.frameLength > detectionRule.frameLength)
                        {
                            flg++;
                        }
                        if (flg == 6)
                        {
                            temp.Add(i);
                        }
                    }
                    if(temp.Count >= detectionRule.detectionCount)
                    {
                        for (int i = 0; i < temp.Count; i++)
                        {
                            if (!detectionNumbers[detectionNumber][detectionRule.ruleNo].Contains(temp[i]))
                            {
                                detectionNumbers[detectionNumber][detectionRule.ruleNo].Add(temp[i]);
                                //dWindow.show_detection((packetData)CaptureData.Items[temp[i]], detectionNumber, detectionRule.ruleNo);
                                targets.Add(temp[i]);
                            }
                        }
                    }
                }
            }
            targets.Sort();
            foreach(int target  in targets)
            {
                dWindow.show_detection((packetData)CaptureData.Items[target], detectionNumber,0);
            }
        }
        private void ruleGroupDataSplit(ruleGroupData ruleGroup,int detectionNumber)
        {
            foreach(ruleData rule in ruleGroup.ruleDatas)
            {
                streamStart[detectionNumber].Add(0);
                detectionNumbers[detectionNumber].Add(new List<int>());
                //2次要素を格納　ルールグループの中のルールの数
                
            }
            detectionSet(detectionNumber);
        }

        private void detectionSet(int detectionNumber)
        {
            DispatcherTimer detectTimer = new DispatcherTimer();
            detectTimer.Interval = new TimeSpan(0, 0, 1);
            detectTimer.Tick += new EventHandler(detection);
            detectTimerList.Add(detectTimer);
            void detection(object sender, EventArgs e)
            {
                    detectLogic(detectionNumber);
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
                    CaptureData.Items.Add(pd);
                }
            }
            catch
            {
                
                //errなどはそのまま出力する
                if (stopflag==true)
                {
                    stopstatus.Content = "中断中";
                }
                else
                {
                    CaptureData.Items.Add(new packetData(msg));
                }
            }
            bool isRowSelected = CaptureData.SelectedItems.Count > 0;

            if (!isRowSelected)
            {
                CaptureData.ScrollIntoView(CaptureData.Items.GetItemAt(CaptureData.Items.Count - 1));
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
            if(mailTimer != null)
            {
                mailTimer.Stop();
            }
            Application.Current.Shutdown();

        }
        private void Grid_Unloaded(object sender, RoutedEventArgs e)
        {
            closing();
        }

        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            packetData packet = (packetData)CaptureData.SelectedItem;
            if (packet != null)
            {
                packet_detail_Window packet_detail = new packet_detail_Window(packet.Data);
                packet_detail.Show();
            }
        }
    }
}
