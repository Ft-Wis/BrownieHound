using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
        public class detectionData
        {
            public string data { get; set; }
            public string color { get; set; }
            public packetData packet { get; set; }
            public ObservableCollection<detectionData> children { get; set; } = new ObservableCollection<detectionData>();
        }
        public detectWindow(List<ruleGroupData> ruleGroupDatas)
        {
            InitializeComponent();

            this.Owner = App.Current.MainWindow;

            double xOffset = -400;  // X軸方向のオフセット
            double yOffset = 200;  // Y軸方向のオフセット

            double newX = this.Owner.Left + this.Owner.Width / 2 + xOffset;
            double newY = this.Owner.Top + this.Owner.Height / 2 + yOffset;

            this.Left = newX;
            this.Top = newY;

            this.WindowStartupLocation = WindowStartupLocation.Manual;
            this.Show();


            for (int i = 0; i < ruleGroupDatas.Count; i++)
            {
                detectionDatas.Add(new detectionData() { data = $"RuleGroup:{ruleGroupDatas[i].Name}",color= "#0000cd" });
                string message = "";
                foreach(RuleData.ruleData detectionRuleData in ruleGroupDatas[i].ruleDatas)
                {
                    if(detectionRuleData.ruleNo != 0)
                    {
                        message += "\n";
                    }
                    message += $"{detectionRuleData.ruleNo}::[interval:{detectionRuleData.detectionInterval}][count:{detectionRuleData.detectionCount}][source:{detectionRuleData.Source}][destination:{detectionRuleData.Destination}][protocol:{detectionRuleData.Protocol}][sourceport:{detectionRuleData.sourcePort}][destport:{detectionRuleData.destinationPort}][length:{detectionRuleData.frameLength}]";
                    
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
            detectionDatas[detectionNumber].children[0].children.Add(new detectionData() { data = message, color = "#000000" ,packet = pd});
            detection_tree.MouseDoubleClick += TreeViewItem_MouseDoubleClick;

        }

        private void TreeViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            detectionData dD = (detectionData)detection_tree.SelectedValue as detectionData;
            try
            {
                if (dD != null && dD.packet != null)
                {
                    packet_detail_Window packet_detail = new packet_detail_Window(dD.packet.Data);
                    packet_detail.Show();
                }
            }
            catch
            {

            }

        }
    }
}
