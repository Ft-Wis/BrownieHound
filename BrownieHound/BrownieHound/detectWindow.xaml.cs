using Org.BouncyCastle.Asn1.Pkcs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;
using static BrownieHound.App;
using static BrownieHound.capture;
using static BrownieHound.ReadPacketData;

namespace BrownieHound
{
    /// <summary>
    /// detectWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class detectWindow : Window
    {
        public ObservableCollection<detectionData> detectionDatas = new ObservableCollection<detectionData>();
        public List<string> detectionRuleNames = new List<string>();
        public int maildataCount = 0;
        public int mailFileCount = 0;
        public class detectionData
        {
            public string data { get; set; }
            public string color { get; set; }
            public string jpacketData { get; set; }
            public ObservableCollection<detectionData> children { get; set; } = new ObservableCollection<detectionData>();
        }
        public detectWindow(List<ruleGroupData> ruleGroupDatas)
        {
            InitializeComponent();

            //this.Owner = App.Current.MainWindow;

            double xOffset = -200;  // X軸方向のオフセット
            double yOffset = 50;  // Y軸方向のオフセット

            double newX = App.Current.MainWindow.Left + App.Current.MainWindow.Width / 2 + xOffset;
            double newY = App.Current.MainWindow.Top - yOffset;

            this.Left = newX;
            this.Top = newY;

            this.WindowStartupLocation = WindowStartupLocation.Manual;
            this.Show();

            if (!Directory.Exists("tempdetectionData"))
            {
                Directory.CreateDirectory("tempdetectionData");
            }
            for(int i = 0; i < ruleGroupDatas.Count; i++)
            {
                if (ruleGroupDatas[i].extendflg)
                {
                    detectionDatas.Add(new detectionData() { data = $"LinkRuleGroup:{ruleGroupDatas[i].Name}", color = "#b8860b" });
                }
                else
                {
                    detectionDatas.Add(new detectionData() { data = $"RuleGroup:{ruleGroupDatas[i].Name}", color = "#0000cd" });
                }
                DetectionTreeLabel detectionTreeLabel = new DetectionTreeLabel()
                {
                    Content = detectionDatas[i].data,
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(detectionDatas[i].color)),
                    Background = new SolidColorBrush(Color.FromArgb(0xFF,0xE5,0xE5,0xE5)),
                    Margin = new Thickness(4,5,4,0),
                
                };
                DetectionTreeView detectionTreeView = new DetectionTreeView()
                {
                    Name = ruleGroupDatas[i].Name,
                };
                DetectionPanel.Children.Add(detectionTreeLabel);
                DetectionPanel.Children.Add(detectionTreeView);
                string message = "";
                using (File.Create($"tempdetectionData\\{ruleGroupDatas[i].Name}.tmp")) { }
                detectionRuleNames.Add(ruleGroupDatas[i].Name);
                foreach (RuleData.ruleData detectionRuleData in ruleGroupDatas[i].ruleDatas)
                {
                    string category;
                    if(detectionRuleData.ruleCategory == 0)
                    {
                        category = "black";
                    }
                    else
                    {
                        category = "white";
                    }
                    if (detectionRuleData.ruleNo != 0)
                    {
                        message += "\n";
                    }
                    message += $"{detectionRuleData.ruleNo}::[category:{category}][interval:{detectionRuleData.detectionInterval}][count:{detectionRuleData.detectionCount}][source:{detectionRuleData.Source}][destination:{detectionRuleData.Destination}][protocol:{detectionRuleData.Protocol}][sourceport:{detectionRuleData.sourcePort}][destport:{detectionRuleData.destinationPort}][length:{detectionRuleData.frameLength}]";

                }
                detectionDatas[i].children.Add(new detectionData() { data = message, color = "IndianRed" });
            }
            for(int i = 0;i < ruleGroupDatas.Count;i++)
            {
                DetectionTreeView tree = DetectionPanel.Children[i * 2 + 1] as DetectionTreeView;
                if (tree != null)
                {
                    tree.DataContext = detectionDatas[i];
                }
            }
        }

        bool closeflg = false;

        public void winClose()
        {
            closeflg = true;
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!closeflg)
            {
                e.Cancel = true;
                this.WindowState = WindowState.Minimized;
            }
        }
        public void show_detection(packetData pd,int detectionNumber)
        {
            try
            {
                string message = $"[No:{pd.Number}]:: [src:{pd.Source}][dest:{pd.Destination}]  [proto:{pd.Protocol}]  [sPort:{pd.sourcePort}][dPort:{pd.destinationPort}]  [length:{pd.frameLength}]";
                detectionDatas[detectionNumber].children[0].children.Add(new detectionData() { data = message, color = "#000000", jpacketData = pd.Data });
                DetectionTreeView tree = DetectionPanel.Children[detectionNumber * 2 + 1] as DetectionTreeView;
                if (tree != null)
                {
                    tree.MouseDoubleClick += TreeViewItem_MouseDoubleClick;
                }
                using (StreamWriter sw = new StreamWriter($"tempdetectionData\\{detectionRuleNames[detectionNumber]}.tmp", true))
                {
                    sw.WriteLine(pd.Data);
                }
                if (File.Exists($"temps\\maildata{mailFileCount}.tmp"))
                {
                    if(maildataCount % 3000 == 0 && maildataCount != 0)
                    {
                        mailFileCount++;
                    }
                    using (StreamWriter sw = new StreamWriter($"temps\\maildata{mailFileCount}.tmp", true))
                    {
                        sw.WriteLine($"{detectionNumber}\\<tbody style='background-color: blanchedalmond;'><tr><td>{pd.Number}</td><td></td><td>{pd.TimeString}</td><td></td><td></td><td>{pd.Source}</td><td>{pd.Destination}</td><td>{pd.Protocol}</td><td>{pd.sourcePort}</td><td>{pd.destinationPort}</td><td>{pd.frameLength}</td></tr></tbody>");
                    }
                    maildataCount++;
                }
                if (detectionDatas[detectionNumber].children[0].children.Count > 1000)
                {
                    detectionDatas[detectionNumber].children[0].children.RemoveAt(0);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("exception" + pd.Number + ex.ToString());
            }
        }
        private T FindAncestor<T>(DependencyObject current) where T : DependencyObject
        {
            do
            {
                if (current is T ancestor)
                {
                    return ancestor;
                }
                current = VisualTreeHelper.GetParent(current);
            } while (current != null);

            return null;
        }

        private void TreeViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            if(e.OriginalSource is FrameworkElement frameworkElement)
            {
                var treeViewitem = FindAncestor<TreeViewItem>(frameworkElement);
                if(treeViewitem != null)
                {
                    detectionData dD = (detectionData)treeViewitem.Header;
                    try
                    {
                        if (dD != null && dD.jpacketData != null)
                        {
                            packet_detail_Window packet_detail = new packet_detail_Window(dD.jpacketData);

                            double detectLeft = this.Left;
                            double detectTop = this.Top;

                            // 新しいウィンドウを配置
                            packet_detail.Left = detectLeft + 50;
                            packet_detail.Top = detectTop + 50;

                            packet_detail.Show();
                        }
                    }
                    catch
                    {

                    }
                }
            }



        }

        //検出したパケットの保存処理
        private void save_button_Click(object sender, RoutedEventArgs e)
        {
            //フォルダが存在しなければ新規作成する
            if (!File.Exists("tempDetectionPackets"))
            {
                Directory.CreateDirectory("tempDetectionPackets");
            }

            DateTime currentDateTime = DateTime.Now;
            string nowTimeFileName = currentDateTime.ToString("yyyy_MM_dd_HH_mm");
            //次のタスク「日付ごとにフォルダを作成する」
            if (!File.Exists("tempDetectionPackets\\"+nowTimeFileName))
            {
                Directory.CreateDirectory("tempDetectionPackets\\" + nowTimeFileName);
            }

            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();

            for (int i = 0; i < detectionRuleNames.Count; i++)
            {
                dlg.FileName = detectionRuleNames[i] + ".txt"; // Default file name
                dlg.DefaultExt = ".txt"; // Default file extension
                dlg.Filter = "textファイル(.txt)|*.txt"; // Filter files by extension

                string currentDirPath = Directory.GetCurrentDirectory();

                //ルールグループごとにフォルダを作成
                if (!Directory.Exists(detectionRuleNames[i]))
                {
                    Directory.CreateDirectory("tempDetectionPackets\\" + nowTimeFileName + "\\" + detectionRuleNames[i]);
                }

                dlg.InitialDirectory = Directory.GetCurrentDirectory() + "\\tempDetectionPackets\\" + nowTimeFileName + "\\" + detectionRuleNames[i];

                // Show save file dialog box
                Nullable<bool> result = dlg.ShowDialog();

                // Process save file dialog box results
                if (result == true)
                {
                    //ファイルをコピーしてくる
                    string tempContent = File.ReadAllText("tempdetectionData\\" + detectionRuleNames[i]+".tmp");

                    string filename = dlg.FileName;
                    File.WriteAllText(filename, tempContent);
                }
            }
        }
    }
}
