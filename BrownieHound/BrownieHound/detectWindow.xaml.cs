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

namespace BrownieHound
{
    /// <summary>
    /// detectWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class detectWindow : Window
    {
        public List<detectionData> detectionDatas = new List<detectionData>();
        public List<string> detectionRuleNames = new List<string>();
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

            double xOffset = -150;  // X軸方向のオフセット
            double yOffset = -25;  // Y軸方向のオフセット

            double newX = App.Current.MainWindow.Left + App.Current.MainWindow.Width / 2 + xOffset;
            double newY = App.Current.MainWindow.Top + App.Current.MainWindow.Height / 2 + yOffset;

            //double newX = this.Owner.Left + this.Owner.Width / 2 + xOffset;
            //double newY = this.Owner.Top + this.Owner.Height / 2 + yOffset;

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
                detectionDatas.Add(new detectionData() { data = $"RuleGroup:{ruleGroupDatas[i].Name}",color= "#0000cd" });
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
            DataContext = detectionDatas;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.WindowState = WindowState.Minimized;
        }
        public void show_detection(packetData pd,int detectionNumber)
        {
            string message = $"[No:{pd.Number}]:: [src:{pd.Source}][dest:{pd.Destination}]  [proto:{pd.Protocol}]  [sPort:{pd.sourcePort}][dPort:{pd.destinationPort}]  [length:{pd.frameLength}]";
            detectionDatas[detectionNumber].children[0].children.Add(new detectionData() { data = message, color = "#000000" ,jpacketData = pd.Data});
            detection_tree.MouseDoubleClick += TreeViewItem_MouseDoubleClick;
            using (StreamWriter sw = new StreamWriter($"tempdetectionData\\{detectionRuleNames[detectionNumber]}.tmp", true))
            {
                sw.WriteLine(pd.Data);
            }
            if (File.Exists("temps\\maildata.tmp"))
            {
                using(StreamWriter sw = new StreamWriter("temps\\maildata.tmp", true))
                {
                    sw.WriteLine($"{detectionNumber}\\<tbody style='background-color: blanchedalmond;'><tr><td>{pd.Number}</td><td></td><td>{pd.Time.TimeOfDay}</td><td></td><td></td><td>{pd.Source}</td><td>{pd.Destination}</td><td>{pd.Protocol}</td><td>{pd.sourcePort}</td><td>{pd.destinationPort}</td><td>{pd.frameLength}</td></tr></tbody>");
                }
            }
            if(detectionDatas[detectionNumber].children[0].children.Count > 1000)
            {
                detectionDatas[detectionNumber].children[0].children.RemoveAt(0);
            }
        }

        private void TreeViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            detectionData dD = (detectionData)detection_tree.SelectedValue as detectionData;
            try
            {
                if (dD != null && dD.jpacketData != null)
                {
                    packet_detail_Window packet_detail = new packet_detail_Window(dD.jpacketData);
                    packet_detail.Show();
                }
            }
            catch
            {

            }

        }

        //検出したパケットの保存処理
        private void save_button_Click(object sender, RoutedEventArgs e)
        {
            DateTime currentDateTime = DateTime.Now;
            string defaultFileName = currentDateTime.ToString("yyyymmddHHmmss"); 
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.InitialDirectory = "C:\\Users\\nkc20\\source\\repos\\Ft-Wis\\sotsuken\\BrownieHound\\BrownieHound\\bin\\Debug\\netcoreapp3.1";
            dlg.FileName = defaultFileName+".tmp"; // Default file name
            dlg.DefaultExt = ".tmp"; // Default file extension
            dlg.Filter = "tempファイル(.tmp)|*.tmp"; // Filter files by extension

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                string filename = dlg.FileName;
                string textToSave = "書き込み完了";
                File.WriteAllText(filename,textToSave);
            }
        }
    }
}
