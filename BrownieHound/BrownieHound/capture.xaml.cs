
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
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;
using Application = System.Windows.Application;
using static BrownieHound.ReadPacketData;

namespace BrownieHound
{
    /// <summary>
    /// capture.xaml の相互作用ロジック
    /// </summary>
    public partial class capture : Page
    {
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
        string mailAddress = "";
        string userName = "";
        int safeLine = 0;

        List<int> recordPacketNo = new List<int>();
        int mostDitectionCount = 1;
        int packetCount = 0;
        int viewDistrictCount = 100;
        private ObservableCollection<packetData> memoryPackets = new ObservableCollection<packetData> { };
        int dataCount = 0;
        int capturePacketsValue = 0;
        int writePlace = 1;
        int viewPlace = 0;
        int mailDetectionCount = 0;
        bool viewUpdateFlg = true;
        bool processFlg = false;
        bool beforeFlg = false;
        bool nextFlg = false;
        bool scrollFlg = true;

        public capture(string tsINumber)
        {
            InitializeComponent();
            this.tsInterfaceNumber = tsINumber;

        }
        public capture(string tsINumber, List<ruleGroupData> detectionRuleGroups)
        {
            InitializeComponent();
            this.tsInterfaceNumber = tsINumber;
            this.detectionRuleGroups = detectionRuleGroups;
            dWindow = new detectWindow(detectionRuleGroups);
            dWindow.Show();
        }

        private void inactivate_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
            "終了してルールグループ選択画面にもどります。\nよろしいですか？",
            "確認",
            MessageBoxButton.OKCancel,
            MessageBoxImage.Question,
            MessageBoxResult.Cancel);
            if (result == MessageBoxResult.OK)
            {
                closing();
                NavigationService.GoBack();
                if (dWindow != null)
                {
                    dWindow.winClose();
                    dWindow.Close();

                }
                List<Window> windows = App.Current.Windows.Cast<Window>().ToList();
                Window mainWindow = windows.FirstOrDefault(window => window is MainWindow);
                foreach (Window window in windows)
                {
                    if (window != mainWindow)
                    {
                        window.Close();
                    }
                }

            }


        }
        bool stopflag = false;
        private void stop_Click(object sende, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
                "検知とキャプチャを停止します。",
                "確認",
                MessageBoxButton.OKCancel,
                MessageBoxImage.Question,
                MessageBoxResult.Cancel);
            if (result == MessageBoxResult.OK)
            {
                stopstatus.Content = "停止";
                stop.IsEnabled = false;
                viewable.IsEnabled = false;
                stopflag = true;
                if (viewUpdateFlg)
                {

                    down_Scroll();
                    if (CaptureData.Items.Count > 500)
                    {
                        nextFlg = true;
                    }
                    viewUpdateFlg = false;
                }

                processTscap.Kill();

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

        }

        private void viewable_Click(object sender, RoutedEventArgs e)
        {

            if (!stopflag)
            {
                stopflag = true;
                if (viewUpdateFlg)
                {

                    down_Scroll();
                    if (CaptureData.Items.Count > 500)
                    {
                        nextFlg = true;
                    }
                    viewUpdateFlg = false;
                }
                stopstatus.Content = "閲覧中";
            }
            else
            {
                stopflag = false;
                nextFlg = false;
                beforeFlg = false;
                viewUpdateFlg = true;
                stopstatus.Content = "更新中";
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
            using (File.Create("temps\\temp1.tmp")) { }
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
                    mailValidation.userName.Value = sr.ReadLine().Split(":")[1];
                    if (bool.TryParse(sr.ReadLine().Split(":")[1], out var isEnabled))
                    {
                        mailValidation.isEnabled.Value = isEnabled;
                    }
                    if (int.TryParse(sr.ReadLine().Split(":")[1], out var span))
                    {
                        mailValidation.span.Value = span.ToString();
                    }
                    if (int.TryParse(sr.ReadLine().Split(":")[1], out var mailLimit))
                    {
                        mailValidation.mailLimit.Value = mailLimit.ToString();
                    }
                    mailValidation.mailAddress.Value = sr.ReadLine().Split(":")[1];

                    //MessageBox.Show(mailValidation.userName.Value +" "+ mailValidation.isEnabled.Value + " "+ mailValidation.span.Value + " "+ mailValidation.mailLimit.Value + " "+ mailValidation.mailAddress.Value + " ");

                    if (mailValidation.isEnabled.Value)
                    {
                        if ((mailValidation.span.Value != "" && !mailValidation.span.HasErrors)
                            && (mailValidation.mailAddress.Value != "" && !mailValidation.mailAddress.HasErrors)
                            && (mailValidation.userName.Value != "" && !mailValidation.userName.HasErrors)
                            && (mailValidation.mailLimit.Value != "" && !mailValidation.mailLimit.HasErrors))
                        {
                            using (File.Create(@"temps\maildata0.tmp")) { };
                            mailAddress = mailValidation.mailAddress.Value;
                            safeLine = int.Parse(mailValidation.mailLimit.Value);
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

            foreach (var detectionRuleGroup in detectionRuleGroups.Select((Value, Index) => new { Value, Index }))
            {
                detectionNumbers.Add(new List<int>());
                int detectionCount = detectionRuleGroup.Value.RuleDatas.Max(x => x.DetectionInterval);
                if (detectionCount > mostDitectionCount)
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
            for (int i = 0; i < detectTimerList.Count; i++)
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
            int captureCount = capturePacketsValue;
            DateTime sendTime = DateTime.Now;
            //ここに閾値のIF分岐を挿入
            if (mailDetectionCount > safeLine)
            {
                Debug.WriteLine(mailDetectionCount);
                mailDetectionCount = 0;
                for (int mailFileNo = 0; File.Exists($"temps\\maildata{mailFileNo}.tmp"); mailFileNo++)
                {
                    var email = new MimeMessage();
                    int addCount;
                    email.From.Add(new MailboxAddress("browniehound", "browniehound2024@gmail.com"));
                    email.To.Add(new MailboxAddress("", mailAddress));
                    email.Subject = userName + "の定期検知メール";
                    var body = new BodyBuilder();
                    body.HtmlBody = $"<html><body><h1>{userName}の定期検知メール-{mailFileNo}</h1><br>";


                    string[] origindata;
                    List<List<string>> sendList = new List<List<string>>();
                    for (int i = 0; i < detectionRuleGroups.Count; i++)
                    {
                        sendList.Add(new List<string>());
                    }
                    using (StreamReader sr = new StreamReader($"temps\\maildata{mailFileNo}.tmp"))
                    {
                        origindata = sr.ReadToEnd().Split('\n');
                    }

                    if (mailFileNo == 0)
                    {
                        using (File.Create("temps\\maildata0.tmp")) { }
                        dWindow.maildataCount = 0;
                        dWindow.mailFileCount = 0;
                        sendTime = DateTime.Now;
                    }
                    else
                    {
                        File.Delete($"temps\\maildata{mailFileNo}.tmp");
                    }
                    body.HtmlBody += $"<p><b>時間：{sendTime}</b></p>";
                    body.HtmlBody += $"<p><b>総キャプチャ数：{captureCount}</b></p>";
                    for (int i = 0; i < origindata.Count() - 1; i++)
                    {
                        int number = Int32.Parse(origindata[i].Split("\\")[0]);
                        sendList[number].Add(origindata[i].Split("\\")[1]);
                    }
                    origindata = null;
                    for (int i = 0; i < detectionRuleGroups.Count; i++)
                    {
                        addCount = 0;
                        if (detectionRuleGroups[i].ExtendFlg)
                        {
                            body.HtmlBody += $"<h3 style='color:rgb(255, 179, 0)'>Link Rule</h3>";
                        }
                        body.HtmlBody += $"<h2>{detectionRuleGroups[i].Name}</h2>";
                        body.HtmlBody += $"<table border='1' style='margin-left:1%;border-collapse: collapse;border-color: thistle;width:98%;'><thead style='background-color:rgb(255, 179, 0);color:rgb(226, 247, 250);'><tr><th style='min-width:3em;'>No</th><th style='min-width:3em'>Category</th><th style='min-width:8em;'>Time</th><th style='min-width:4em;'>間隔(s)</th><th style='min-width:2em;'>頻度</th><th style='min-width:18em;'>Source</th><th style='min-width:18em;'>Destination</th><th style='min-width:5em;'>Protocol</th><th style='min-width:6em;'>sourcePort</th><th style='min-width:5em;'>destPort</th><th style='min-width:4em;'>Length</th></tr></thead>";
                        for (int j = 0; j < detectionRuleGroups[i].RuleDatas.Count; j++)
                        {
                            string category;
                            if (detectionRuleGroups[i].RuleDatas[j].RuleCategory == 0)
                            {
                                category = "black";
                            }
                            else
                            {
                                category = "white";
                            }
                            body.HtmlBody += $"<thead style='background-color:rgb(255, 179, 0);color:rgb(226, 247, 250);'><tr><th>{detectionRuleGroups[i].RuleDatas[j].RuleNo}</th><th>{category}</th><th>0</th><th>{detectionRuleGroups[i].RuleDatas[j].DetectionInterval}</th><th>{detectionRuleGroups[i].RuleDatas[j].DetectionCount}</th><th>{detectionRuleGroups[i].RuleDatas[j].Source}</th><th>{detectionRuleGroups[i].RuleDatas[j].Destination}</th><th>{detectionRuleGroups[i].RuleDatas[j].Protocol}</th><th>{detectionRuleGroups[i].RuleDatas[j].SourcePort}</th><th>{detectionRuleGroups[i].RuleDatas[j].DestinationPort}</th><th>{detectionRuleGroups[i].RuleDatas[j].FrameLength}</th></tr></thead>";
                        }
                        while (0 < sendList[i].Count)
                        {
                            addCount++;
                            body.HtmlBody += sendList[i][0];
                            sendList[i].RemoveAt(0);
                        }
                        body.HtmlBody += "</table><br>";
                        body.HtmlBody += $"<p><b>検知増分：{addCount}</b></p>";
                                       
                    }
                    sendList = null;
                    body.HtmlBody += "</body></html>";
                    email.Body = body.ToMessageBody();
                    body = null;
                    using (var smtp = new SmtpClient())
                    {
                        await smtp.ConnectAsync("smtp.gmail.com", 587, false);
                        await smtp.AuthenticateAsync("browniehound2024", "eszyyyyhrwarlsns");
                        await smtp.SendAsync(email);
                        await smtp.DisconnectAsync(true);
                    }
                }
            }
            else
            {
                //閾値を超えてないとき。
                MessageBox.Show("閾値を超えていません");
                //ファイルを削除する処理
                string pathToTemps = @"temps\";
                string[] files = Directory.GetFiles(pathToTemps);
                foreach (string file in files)
                {
                    if (file.ToString().Contains("mail"))
                    {
                        File.Delete(file);
                    }
                }
                using (File.Create(@"temps\maildata0.tmp")) { };
            }
        }

        private void recordTime(object sender, EventArgs e)
        {
            recordPacketNo.Add(packetCount);

            if (detectionRuleGroups.Count == 0)
            {
                while (recordPacketNo.Count > mostDitectionCount + 10 && memoryPackets.Count > 200)
                {
                    while (memoryPackets[0].Number < recordPacketNo[1])
                    {
                        memoryPackets.RemoveAt(0);
                    }
                    recordPacketNo.RemoveAt(0);

                }
            }
            if (viewUpdateFlg)
            {
                CaptureData.ItemsSource = null;
                CaptureData.Items.Clear();
                CaptureData.ItemsSource = memoryPackets;
                bool isRowSelected = CaptureData.SelectedItems.Count > 0;

                if (scrollFlg && CaptureData.Items.Count > 0)
                {
                    CaptureData.ScrollIntoView(CaptureData.Items.GetItemAt(CaptureData.Items.Count - 1));
                }

            }

            clock++;
            GC.Collect();
        }

        private void detectLogic(int detectionNumber)
        {
            bool detectFlg = true;
            int recordEnd = recordPacketNo.Count - 1;
            int end = recordPacketNo[recordEnd] - 1;
            capturePacketsValue = end;
            List<packetData> packetList = new List<packetData>();
            if (detectionRuleGroups[detectionNumber].BlackListRules.Count > 0)
            {
                List<int> detectionCount = new List<int>();
                List<int> temp = new List<int>();
                foreach (var detectionRule in detectionRuleGroups[detectionNumber].BlackListRules.Select((Value, Index) => new { Value, Index }))
                {
                    temp.Add(0);
                    detectionCount.Add(0);
                    if (detectionRule.Value.DetectionInterval <= recordEnd)
                    {
                        int start = recordPacketNo[recordEnd - detectionRule.Value.DetectionInterval];
                        int detectIndex = 0;
                        while (detectIndex < memoryPackets.Count)
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
                            if (memoryPackets[i].Number == 0)
                            {
                                continue;
                            }
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
                            if (detectionRule.Value.SourcePort.Equals("all") || detectionRule.Value.SourcePort.Equals(memoryPackets[i].SourcePort))
                            {
                                flg++;
                            }
                            if (detectionRule.Value.DestinationPort.Equals("all") || detectionRule.Value.DestinationPort.Equals(memoryPackets[i].DestinationPort))
                            {
                                flg++;
                            }
                            if (detectionRule.Value.FrameLength.Equals("none") || Int32.Parse(memoryPackets[i].FrameLength) >= Int32.Parse(detectionRule.Value.FrameLength))
                            {
                                flg++;
                            }
                            if (flg == 6)
                            {
                                foreach (RuleData.ruleData whiteListRule in detectionRuleGroups[detectionNumber].WhiteListRules)
                                {
                                    int wFlg = 0;
                                    if (whiteListRule.Source.Equals("all") || whiteListRule.Source.Equals(memoryPackets[i].Source))
                                    {
                                        wFlg++;
                                    }
                                    if (whiteListRule.Destination.Equals("all") || whiteListRule.Destination.Equals(memoryPackets[i].Destination))
                                    {
                                        wFlg++;
                                    }
                                    if (whiteListRule.Protocol.Equals("all") || whiteListRule.Protocol.Equals(memoryPackets[i].Protocol))
                                    {
                                        wFlg++;
                                    }
                                    if (whiteListRule.SourcePort.Equals("all") || whiteListRule.SourcePort.Equals(memoryPackets[i].SourcePort))
                                    {
                                        wFlg++;
                                    }
                                    if (whiteListRule.DestinationPort.Equals("all") || whiteListRule.DestinationPort.Equals(memoryPackets[i].DestinationPort))
                                    {
                                        wFlg++;
                                    }
                                    if (whiteListRule.FrameLength.Equals("none") || Int32.Parse(memoryPackets[i].FrameLength) <= Int32.Parse(whiteListRule.FrameLength))
                                    {
                                        wFlg++;
                                    }
                                    if (wFlg == 6)
                                    {
                                        flg--;
                                    }
                                }
                                if (flg == 6)
                                {
                                    detectionCount[detectionRule.Index]++;
                                    if (!packetList.Contains(memoryPackets[i]))
                                    {
                                        packetList.Add(memoryPackets[i]);
                                        temp[detectionRule.Index]++;
                                    }
                                }

                            }
                        }

                        if (detectionCount[detectionRule.Index] < detectionRule.Value.DetectionCount)
                        {
                            if (detectionRuleGroups[detectionNumber].ExtendFlg)
                            {
                                detectFlg = false;
                                break;
                            }
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
                    int detectIndex = 0;
                    while (detectIndex < memoryPackets.Count)
                    {
                        if (memoryPackets[detectIndex].Number == start)
                        {
                            break;
                        }
                        detectIndex++;
                    }
                    for (int i = detectIndex; memoryPackets[i].Number <= end; i++)
                    {
                        if (memoryPackets[i].Number == 0)
                        {
                            continue;
                        }
                        packetList.Add(memoryPackets[i]);
                    }
                    foreach (RuleData.ruleData whiteListRule in detectionRuleGroups[detectionNumber].WhiteListRules)
                    {

                        for (int i = 0; i < packetList.Count; i++)
                        {
                            int wFlg = 0;
                            if (whiteListRule.Source.Equals("all") || whiteListRule.Source.Equals(packetList[i].Source))
                            {
                                wFlg++;
                            }
                            if (whiteListRule.Destination.Equals("all") || whiteListRule.Destination.Equals(packetList[i].Destination))
                            {
                                wFlg++;
                            }
                            if (whiteListRule.Protocol.Equals("all") || whiteListRule.Protocol.Equals(packetList[i].Protocol))
                            {
                                wFlg++;
                            }
                            if (whiteListRule.SourcePort.Equals("all") || whiteListRule.SourcePort.Equals(packetList[i].SourcePort))
                            {
                                wFlg++;
                            }
                            if (whiteListRule.DestinationPort.Equals("all") || whiteListRule.DestinationPort.Equals(packetList[i].DestinationPort))
                            {
                                wFlg++;
                            }
                            if (whiteListRule.FrameLength.Equals("none") || Int32.Parse(packetList[i].FrameLength) <= Int32.Parse(whiteListRule.FrameLength))
                            {
                                wFlg++;
                            }
                            if (wFlg == 6)
                            {
                                packetList.RemoveAt(i--);

                            }

                        }


                    }
                }
            }
            packetList.Sort((a, b) => a.Number - b.Number);
            foreach (var packet in packetList)
            {
                if (detectFlg && !detectionNumbers[detectionNumber].Contains(packet.Number))
                {
                    dWindow.show_detection(packet, detectionNumber);
                    detectionNumbers[detectionNumber].Add(packet.Number);
                    mailDetectionCount++;
                }

            }
            while (recordPacketNo.Count > mostDitectionCount + 10 && memoryPackets.Count > 100)
            {
                while (memoryPackets[0].Number < recordPacketNo[1])
                {
                    memoryPackets.RemoveAt(0);
                }
                recordPacketNo.RemoveAt(0);

            }
        }
        private void ruleGroupDataSplit(ruleGroupData ruleGroup, int detectionNumber)
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
                    if (Directory.Exists("temps"))
                    {
                        dataCount++;
                        packetCount++;
                        using (StreamWriter sw = new StreamWriter($"temps\\temp{writePlace}.tmp", true))
                        {
                            sw.WriteLine(msg);
                        }
                        if (viewUpdateFlg)
                        {
                            viewPlace = dataCount / 500;
                        }
                        if (dataCount % 5000 == 0)
                        {
                            writePlace++;
                        }
                        memoryPackets.Add(new packetData((JObject)packetObject["layers"]));
                    }
                }
            }
            catch
            {
                if (Directory.Exists("temps") && stop.IsEnabled)
                {
                    dataCount++;
                    using (StreamWriter sw = new StreamWriter($"temps\\temp{writePlace}.tmp", true))
                    {
                        sw.WriteLine(msg);
                    }
                    if (viewUpdateFlg)
                    {
                        viewPlace = dataCount / 500;
                    }
                    if (dataCount % 5000 == 0)
                    {
                        writePlace++;
                    }
                    memoryPackets.Add(new packetData(msg));
                }
            }

        }
        private void chaptureDataGrid_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {

            ScrollViewer scrollViewer = GetScrollViewer(CaptureData);

            if (!viewUpdateFlg)
            {
                if (!processFlg && scrollViewer.VerticalOffset + scrollViewer.ViewportHeight >= scrollViewer.ExtentHeight * 0.85 && CaptureData.Items.Count % 500 == 0)
                {
                    processFlg = true;

                    if (beforeFlg)
                    {
                        viewPlace++;
                        beforeFlg = false;
                    }

                    if (viewPlace < writePlace * 10 - 1)
                    {

                        readToNext(scrollViewer);
                        nextFlg = true;
                    }

                    processFlg = false;
                }
                if (!processFlg && scrollViewer.VerticalOffset + scrollViewer.ViewportHeight <= scrollViewer.ExtentHeight * 0.15)
                {
                    processFlg = true;

                    if (nextFlg)
                    {
                        viewPlace--;
                        nextFlg = false;
                    }
                    if (viewPlace > 0)
                    {

                        readTobefore(scrollViewer);
                        beforeFlg = true;
                    }
                    processFlg = false;
                }
            }
            else
            {
                if (scrollViewer.VerticalOffset + scrollViewer.ViewportHeight > scrollViewer.ExtentHeight * 0.95 || CaptureData.Items.Count <= 100)
                {
                    scrollFlg = true;
                }
            }
        }
        private void readTobefore(ScrollViewer scrollViewer)
        {
            scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset + 500);
            scrollViewer.IsEnabled = false;
            viewPlace--;
            string[] viewPacketStrings = new string[501];
            using (StreamReader sr = new StreamReader($"temps\\temp{viewPlace / 10 + 1}.tmp"))
            {
                for (int i = 0; i < (viewPlace % 10) * 500; i++)
                {
                    sr.ReadLine();
                }
                for (int i = 0; i < 500; i++)
                {
                    viewPacketStrings[i] = sr.ReadLine();

                }
            }
            for (int i = 0; i < 500; i++)
            {
                CaptureData.Items.Insert(i, transfer(viewPacketStrings[i]));
            }
            if (CaptureData.Items.Count > 1000)
            {
                int end = CaptureData.Items.Count - 1000;
                for (int i = 0; i < end; i++)
                {
                    CaptureData.Items.RemoveAt(1000);
                }
            }
            viewPacketStrings = null;
            GC.Collect();
            scrollViewer.IsEnabled = true;

        }
        private void readToNext(ScrollViewer scrollViewer)
        {

            double scrollPlace = scrollViewer.VerticalOffset;
            scrollViewer.ScrollToVerticalOffset(scrollPlace % 500);
            scrollViewer.IsEnabled = false;

            viewPlace++;
            string[] viewPacketStrings = new string[501];
            using (StreamReader sr = new StreamReader($"temps\\temp{viewPlace / 10 + 1}.tmp"))
            {
                for (int i = 0; i < (viewPlace % 10) * 500; i++)
                {
                    sr.ReadLine();
                }
                for (int i = 0; i < 500; i++)
                {
                    string lines = sr.ReadLine();
                    if (lines != null)
                    {
                        viewPacketStrings[i] = lines;
                    }
                    else
                    {
                        viewPacketStrings[i] = lines;
                        break;
                    }
                }
            }
            for (int i = 0; viewPacketStrings[i] != null; i++)
            {
                CaptureData.Items.Add(transfer(viewPacketStrings[i]));
            }
            if (CaptureData.Items.Count > 1000)
            {
                for (int i = 0; i < 500; i++)
                {
                    CaptureData.Items.RemoveAt(0);
                }
            }
            viewPacketStrings = null;
            GC.Collect();
            scrollViewer.IsEnabled = true;
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
            try
            {
                Dispatcher.Invoke(new Action(() => Printpacket(msg)));
            }
            catch
            {

            }
        }
        private void closing()
        {
            if (processTscap != null && !processTscap.HasExited)
            {
                processTscap.Kill();
            }
            clockTimer.Stop();
            foreach (var detectTimer in detectTimerList)
            {
                detectTimer.Stop();
            }
            if (mailTimer != null)
            {
                mailTimer.Stop();
            }
            if (Directory.Exists("temps"))
            {
                Directory.Delete("temps", true);
            }
            if (Directory.Exists("tempdetectionData"))
            {
                Directory.Delete("tempdetectionData", true);
            }


        }
        private void Grid_Unloaded(object sender, RoutedEventArgs e)
        {
            closing();
        }

        private void up_Click(object sender, RoutedEventArgs e)
        {

            ScrollViewer scrollViewer = GetScrollViewer(CaptureData);
            scrollViewer.ScrollToVerticalOffset(0);
            CaptureData.ItemsSource = null;
            CaptureData.Items.Clear();

            string[] viewPacketStrings = new string[501];

            using (StreamReader sr = new StreamReader("temps\\temp1.tmp"))
            {
                for (int i = 0; i < 500; i++)
                {
                    string line = sr.ReadLine();
                    if (line != null)
                    {
                        viewPacketStrings[i] = line;
                    }
                    else
                    {
                        viewPacketStrings[i] = line;
                        break;
                    }

                }
            }

            for (int i = 0; viewPacketStrings[i] != null; i++)
            {
                CaptureData.Items.Add(transfer(viewPacketStrings[i]));
            }
            viewPacketStrings = null;
            GC.Collect();
            viewPlace = 0;
            beforeFlg = false;
            nextFlg = false;
            viewUpdateFlg = false;
        }
        private void down_Scroll()
        {
            viewPlace = dataCount / 500;
            if (stopflag)
            {

                CaptureData.ItemsSource = null;
                CaptureData.Items.Clear();
                string[] viewPacketStrings = new string[501];
                if (dataCount % 500 == 0)
                {
                    viewPlace--;
                }
                else if (dataCount % 500 < 30 && viewPlace > 0)
                {
                    using (StreamReader sr = new StreamReader($"temps\\temp{(viewPlace - 1) / 10 + 1}.tmp"))
                    {
                        for (int i = 0; i < ((viewPlace - 1) % 10) * 500; i++)
                        {
                            sr.ReadLine();
                        }
                        for (int i = 0; i < 500; i++)
                        {
                            string line = sr.ReadLine();
                            if (line != null)
                            {
                                viewPacketStrings[i] = line;
                            }
                            else
                            {
                                viewPacketStrings[i] = line;
                                break;
                            }
                        }
                    }
                    for (int i = 0; viewPacketStrings[i] != null; i++)
                    {
                        CaptureData.Items.Add(transfer(viewPacketStrings[i]));
                    }
                    viewPacketStrings = new string[501];
                    GC.Collect();
                }
                using (StreamReader sr = new StreamReader($"temps\\temp{viewPlace / 10 + 1}.tmp"))
                {
                    for (int i = 0; i < (viewPlace % 10) * 500; i++)
                    {
                        sr.ReadLine();
                    }
                    for (int i = 0; i < 500; i++)
                    {
                        string line = sr.ReadLine();
                        if (line != null)
                        {
                            viewPacketStrings[i] = line;
                        }
                        else
                        {
                            viewPacketStrings[i] = line;
                            break;
                        }
                    }
                }
                for (int i = 0; viewPacketStrings[i] != null; i++)
                {
                    CaptureData.Items.Add(transfer(viewPacketStrings[i]));
                }
                viewPacketStrings = null;
                GC.Collect();
                ScrollViewer scrollViewer = GetScrollViewer(CaptureData);
                scrollViewer.ScrollToVerticalOffset(CaptureData.Items.Count);
            }
            else
            {
                viewUpdateFlg = true;
                CaptureData.ScrollIntoView(CaptureData.Items.GetItemAt(CaptureData.Items.Count - 1));

                GC.Collect();
            }

        }

        private void down_Click(object sender, RoutedEventArgs e)
        {
            nextFlg = false;
            beforeFlg = false;
            down_Scroll();
        }

        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            packetData packet = (packetData)CaptureData.SelectedItem;
            if (packet != null)
            {
                packet_detail_Window packet_detail = new packet_detail_Window(packet.Data);

                double capchaLeft = App.Current.MainWindow.Left;
                double capchaTop = App.Current.MainWindow.Top;

                //新しいウィンドウを配置
                packet_detail.Left = capchaLeft + 50;
                packet_detail.Top = capchaTop + 50;
                packet_detail.Show();
            }
        }

        private void CaptureData_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                scrollFlg = false;
            }
        }
    } 
}
