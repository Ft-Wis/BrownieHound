
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
using Reactive.Bindings.Extensions;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Bcpg;
using static BrownieHound.RuleData;

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
        DispatcherTimer clockTimer = null;
        DispatcherTimer mailTimer = null;
        int clock = 0;
        //１秒単位の経過時間
        List<List<int>> detectionNumbers = new List<List<int>>();
        //検出したキャプチャデータのナンバーをルールに対応付けて格納
        //これを基に検知画面に表示したい
        detectWindow dWindow;
        string path = @"conf";
        List<ruleGroupData> detectionRuleGroups = new List<ruleGroupData>();
        string mailAddress = null;

        List<string> viewPacketString = new List<string>();
        List<int> recordPacketNo = new List<int>();
        int mostDitectionCount = 1;
        int packetCount = 0;
        int viewDistrictCount = 100;
        private ObservableCollection<packetData> viewPacketDatas;
        private ObservableCollection<packetData> memoryPackets = new ObservableCollection<packetData> { };
        int dataCount = 0;

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
            clockTimer.Stop();
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
            if (!Directory.Exists("temps"))
            {
                Directory.CreateDirectory("temps");
            }
            using (File.Create("temps\\temp.tmp")) { }
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
                            using (File.Create("temps\\maildata.tmp")) { }
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

            foreach (var detectionRuleGroup in detectionRuleGroups.Select((Value, Index) => new {Value,Index }))
            {
                detectionNumbers.Add(new List<int>());
                int detectionCount = detectionRuleGroup.Value.ruleDatas.Max(x => x.detectionInterval);
                if(detectionCount > mostDitectionCount)
                {
                    mostDitectionCount = detectionCount;
                }
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
            email.Subject = "userの定期検知メール";
            var body = new BodyBuilder();
            body.HtmlBody = $"<html><body><h1>userの定期検知メール</h1><br>";
            body.HtmlBody += $"<p><b>総キャプチャ数：{packetCount}</b></p>";
            string[] origindata;
            List<List<string>> sendList = new List<List<string>>(detectionRuleGroups.Count);

            for(int i = 0;i < detectionRuleGroups.Count; i++)
            {
                sendList.Add(new List<string>());
            }
            using (StreamReader sr = new StreamReader("temps\\maildata.tmp"))
            {
                origindata = sr.ReadToEnd().Split('\n');
            }
            using (File.Create("temps\\maildata.tmp")) { }
            for(int i = 0;i < origindata.Count() - 1; i++)
            {
                int number = Int32.Parse(origindata[i].Split("\\")[0]);
                sendList[number].Add(origindata[i].Split("\\")[1]);
            }
            for (int i = 0;i < detectionRuleGroups.Count;i++)
            {
                addCount = 0;
                body.HtmlBody += $"<h2>{detectionRuleGroups[i].Name}</h2>";
                for(int j = 0; j < detectionRuleGroups[i].ruleDatas.Count;j++)
                {
                    string category;
                    if (detectionRuleGroups[i].ruleDatas[j].ruleCategory == 0)
                    {
                        category = "black";
                    }
                    else
                    {
                        category = "white";
                    }
                    body.HtmlBody += $"<table border='1' style='margin-left:1%;border-collapse: collapse;border-color: thistle;width:98%;'><thead style='background-color:rgb(255, 179, 0);color:rgb(226, 247, 250);'><tr><th style='min-width:3em;'>No</th><th style='min-width:3em'>Category</th><th style='min-width:8em;'>Time</th><th style='min-width:4em;'>間隔(s)</th><th style='min-width:2em;'>頻度</th><th style='min-width:18em;'>Source</th><th style='min-width:18em;'>Destination</th><th style='min-width:5em;'>Protocol</th><th style='min-width:6em;'>sourcePort</th><th style='min-width:5em;'>destPort</th><th style='min-width:4em;'>Length</th></tr></thead>";
                    body.HtmlBody += $"<thead style='background-color:rgb(255, 179, 0);color:rgb(226, 247, 250);'><tr><th>{detectionRuleGroups[i].ruleDatas[j].ruleNo}</th><th>{category}</th><th>0</th><th>{detectionRuleGroups[i].ruleDatas[j].detectionInterval}</th><th>{detectionRuleGroups[i].ruleDatas[j].detectionCount}</th><th>{detectionRuleGroups[i].ruleDatas[j].Source}</th><th>{detectionRuleGroups[i].ruleDatas[j].Destination}</th><th>{detectionRuleGroups[i].ruleDatas[j].Protocol}</th><th>{detectionRuleGroups[i].ruleDatas[j].sourcePort}</th><th>{detectionRuleGroups[i].ruleDatas[j].destinationPort}</th><th>{detectionRuleGroups[i].ruleDatas[j].frameLength}</th></tr></thead>";
                }
                for(int j = 0;j < sendList[i].Count;j++)
                {
                    addCount++;
                    body.HtmlBody += sendList[i][j];
                }
                body.HtmlBody += "</table><br>";
                body.HtmlBody += $"<p><b>検知増分：{addCount}</b></p>";

            }
            body.HtmlBody += "</body></html>";

            email.Body = body.ToMessageBody();
            //どこかでformatExceptionが起きている
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
            recordPacketNo.Add(packetCount);

            if (detectionRuleGroups.Count == 0)
            {
                while (recordPacketNo.Count > mostDitectionCount + 10 && memoryPackets.Count > 100)
                {
                    while (memoryPackets[0].Number < recordPacketNo[1])
                    {
                        memoryPackets.RemoveAt(0);
                    }
                    recordPacketNo.RemoveAt(0);

                }
            }

            int countNumber = dataCount;
            int endPoint = viewPacketString.Count;
            int readPoint = 0;
            writeFile(endPoint);
            if (countNumber > 0)
            {
                countNumber -= 1;
            }
            //viewPacketDatas = new ObservableCollection<packetData>();
            //if(countNumber >= viewDistrictCount)
            //{
            //    readPoint = countNumber - viewDistrictCount;
            //}
            //readFile(readPoint, countNumber);
            CaptureData.ItemsSource = memoryPackets;
            bool isRowSelected = CaptureData.SelectedItems.Count > 0;

            if (!isRowSelected && CaptureData.Items.Count > 0)
            {
                CaptureData.ScrollIntoView(CaptureData.Items.GetItemAt(CaptureData.Items.Count - 1));
            }
            clock++;

        }
        private packetData transfar(string msg)
        {
            packetData pd = null;
            try
            {
                JObject packetObject = JObject.Parse(msg);
                if (packetObject["layers"] != null)
                {
                    pd = new packetData((JObject)packetObject["layers"]);
                }
                }
            catch
            {

                //errなどはそのまま出力する
                if (stopflag == true)
                {
                    stopstatus.Content = "中断中";
                }
                else
                {
                    pd = (new packetData(msg));
                }
            }
            return pd;
        }
        private void writeFile(int endPoint)
        {
            using (StreamWriter sw = new StreamWriter("temps\\temp.tmp", true, Encoding.GetEncoding("UTF-8")))
            {
                for(int i = 0;i < endPoint; i++)
                {
                    sw.WriteLine(viewPacketString[i]);
                }

            }
            viewPacketString.RemoveRange(0, endPoint);
        }
        private void readFile(int start,int end)
        {
            
            using(StreamReader sr = new StreamReader("temps\\temp.tmp"))
            {
                //List<string> temps = new List<string>(sr.ReadToEnd().Split("\n"));
                //Debug.WriteLine(temps[start]);
                for(int i = 0;i<start;i++)
                {
                    sr.ReadLine();
                }
                for(int i = start;i< end; i++)
                {
                    //viewPacketDatas.Add(transfar(temps[i]));
                    viewPacketDatas.Add(transfar(sr.ReadLine()));
                }

            }
        }
        private void detectLogic(int detectionNumber)
        {
            int recordEnd = recordPacketNo.Count - 1;
            int end = recordPacketNo[recordEnd] - 1;
            List<packetData> packetList = new List<packetData>();
            if (detectionRuleGroups[detectionNumber].blackListRules.Count > 0)
            {
                List<int> temp = new List<int>();
                foreach (var detectionRule in detectionRuleGroups[detectionNumber].blackListRules.Select((Value, Index) => new { Value, Index }))
                {
                    temp.Add(0);

                    if (detectionRule.Value.detectionInterval <= recordEnd)
                    {
                        int start = recordPacketNo[recordEnd - detectionRule.Value.detectionInterval];
                        //if (recordEnd > detectionRule.Value.detectionInterval && start == recordPacketNo[recordEnd - detectionRule.Value.detectionInterval - 1] && start < end)
                        //{
                        //    //検知する範囲内で出現したパケットのみを対象とする処理
                        //    //0,0,2,2,3...等の時に２回目の試行には0を入れたくない
                        //    start++;
                        //}
                        int detectIndex = 0;
                        while(detectIndex < memoryPackets.Count)
                        {
                            if (memoryPackets[detectIndex].Number == start)
                            {
                                break;
                            }
                            detectIndex++;
                        }

                        for (int i = detectIndex; memoryPackets[i].Number <= end; i++)
                        {
                            int flg = 0;
                            if (detectionRule.Value.Source.Equals("all") || detectionRule.Value.Source.Equals(memoryPackets[i].Source))
                            {
                                flg++;
                            }
                            if (detectionRule.Value.Destination.Equals("all") || detectionRule.Value.Destination.Equals(memoryPackets[i].Destination))
                            {
                                flg++;
                            }
                            if (detectionRule.Value.Protocol.Equals("all") || detectionRule.Value.Protocol.Equals(memoryPackets[i].Protocol))
                            {
                                flg++;
                            }
                            if (detectionRule.Value.sourcePort.Equals("all") || detectionRule.Value.sourcePort.Equals(memoryPackets[i].sourcePort))
                            {
                                flg++;
                            }
                            if (detectionRule.Value.destinationPort.Equals("all") || detectionRule.Value.destinationPort.Equals(memoryPackets[i].destinationPort))
                            {
                                flg++;
                            }
                            if (memoryPackets[i].frameLength >= detectionRule.Value.frameLength)
                            {
                                flg++;
                            }
                            if (flg == 6)
                            {
                                foreach (ruleData whiteListRule in detectionRuleGroups[detectionNumber].whiteListRules)
                                {
                                    int wflg = 0;
                                    if (whiteListRule.Source.Equals("all") || whiteListRule.Source.Equals(memoryPackets[i].Source))
                                    {
                                        wflg++;
                                    }
                                    if (whiteListRule.Destination.Equals("all") || whiteListRule.Destination.Equals(memoryPackets[i].Destination))
                                    {
                                        wflg++;
                                    }
                                    if (whiteListRule.Protocol.Equals("all") || whiteListRule.Protocol.Equals(memoryPackets[i].Protocol))
                                    {
                                        wflg++;
                                    }
                                    if (whiteListRule.sourcePort.Equals("all") || whiteListRule.sourcePort.Equals(memoryPackets[i].sourcePort))
                                    {
                                        wflg++;
                                    }
                                    if (whiteListRule.destinationPort.Equals("all") || whiteListRule.destinationPort.Equals(memoryPackets[i].destinationPort))
                                    {
                                        wflg++;
                                    }
                                    if (memoryPackets[i].frameLength <= whiteListRule.frameLength)
                                    {
                                        wflg++;
                                    }
                                    if (wflg == 6)
                                    {
                                        flg--;
                                    }
                                }
                                if (flg == 6)
                                {
                                    if (!packetList.Contains(memoryPackets[i]))
                                    {
                                        packetList.Add(memoryPackets[i]);
                                        temp[detectionRule.Index]++;
                                    }
                                }

                            }
                        }

                        if (temp[detectionRule.Index] < detectionRule.Value.detectionCount)
                        {
                            int startIndex = 0;
                            int i = 0;
                            for (; i < temp.Count - 1 - 1; i++)
                            {
                                startIndex += temp[i];
                            }
                            packetList.RemoveRange(startIndex, temp[i]);
                            temp[i] = 0;
                        }
                    }
                }
            }
            else
            {
                if (1 <= recordEnd)
                {
                    int start = recordPacketNo[recordEnd - 1];
                    //if (recordEnd > 1 && start == recordPacketNo[recordEnd - 1 - 1] && start < end)
                    //{
                    //    start++;
                    //}
                    int detectIndex = 0;
                    while (detectIndex < memoryPackets.Count)
                    {
                        if (memoryPackets[detectIndex].Number == start)
                        {
                            break;
                        }
                        detectIndex++;
                    }
                    for(int i = detectIndex; memoryPackets[i].Number <= end; i++)
                    {
                        if (memoryPackets[i].Number == 0)
                        {
                            continue;
                        }
                        packetList.Add(memoryPackets[i]);
                    }
                    foreach (ruleData whiteListRule in detectionRuleGroups[detectionNumber].whiteListRules)
                    {

                        for (int i = 0; i < packetList.Count; i++)
                        {
                            int wflg = 0;
                            if (whiteListRule.Source.Equals("all") || whiteListRule.Source.Equals(packetList[i].Source))
                            {
                                wflg++;
                            }
                            if (whiteListRule.Destination.Equals("all") || whiteListRule.Destination.Equals(packetList[i].Destination))
                            {
                                wflg++;
                            }
                            if (whiteListRule.Protocol.Equals("all") || whiteListRule.Protocol.Equals(packetList[i].Protocol))
                            {
                                wflg++;
                            }
                            if (whiteListRule.sourcePort.Equals("all") || whiteListRule.sourcePort.Equals(packetList[i].sourcePort))
                            {
                                wflg++;
                            }
                            if (whiteListRule.destinationPort.Equals("all") || whiteListRule.destinationPort.Equals(packetList[i].destinationPort))
                            {
                                wflg++;
                            }
                            if (packetList[i].frameLength <= whiteListRule.frameLength)
                            {
                                wflg++;
                            }
                            if (wflg == 6)
                            {
                                packetList.RemoveAt(i--);

                            }

                        }


                    }
                }
            }
            packetList.Sort((a,b)=>a.Number - b.Number);
            foreach(var packet in packetList)
            {
                if (!detectionNumbers[detectionNumber].Contains(packet.Number))
                {
                    dWindow.show_detection(packet, detectionNumber);
                    detectionNumbers[detectionNumber].Add(packet.Number);
                }

            }
            while (recordPacketNo.Count > mostDitectionCount + 10 && memoryPackets.Count > 100)
            {
                while(memoryPackets[0].Number < recordPacketNo[1])
                {
                    memoryPackets.RemoveAt(0);
                }
                recordPacketNo.RemoveAt(0);

            }
        }
        private void ruleGroupDataSplit(ruleGroupData ruleGroup,int detectionNumber)
        {
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
                    dataCount++;
                    packetCount++;
                    viewPacketString.Add(msg);
                    memoryPackets.Add(new packetData((JObject)packetObject["layers"]));
                }
            }
            catch
            {

                //errなどはそのまま出力する
                if (stopflag == true)
                {
                    stopstatus.Content = "中断中";
                }
                else
                {
                    dataCount++;
                    viewPacketString.Add(msg);
                    memoryPackets.Add(new packetData(msg));
                }
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
            if (File.Exists("temps\\temp.tmp"))
            {
                File.Delete("temps\\temp.tmp");
            }
            if (File.Exists("temps\\maildata.tmp"))
            {
                File.Delete("temps\\maildata.tmp");
            }
            if (Directory.Exists("tempdetectionData"))
            {
                Directory.Delete("tempdetectionData",true);
            }

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

        private void up_Click(object sender, RoutedEventArgs e)
        {
            CaptureData.ScrollIntoView(CaptureData.Items.GetItemAt(0));
            CaptureData.SelectedIndex = 0;
        }

        private void doun_Click(object sender, RoutedEventArgs e)
        {
            CaptureData.ScrollIntoView(CaptureData.Items.GetItemAt(CaptureData.Items.Count - 1));
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
