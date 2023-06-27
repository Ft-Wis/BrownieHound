using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
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
        List<detectionData> detectionDatas = new List<detectionData>();
        public class detectionData
        {
            public string data { get; set; }
            public string color { get; set; }
            public ObservableCollection<detectionData> children { get; set; } = new ObservableCollection<detectionData>();  
        }
        public detectWindow(List<ruleGroupData> ruleGroupDatas)
        {
            InitializeComponent();
            for(int i = 0; i < ruleGroupDatas.Count; i++)
            {
                detectionDatas.Add(new detectionData() { data = ruleGroupDatas[i].Name,color= "#e6e6fa" });
                foreach(ruleData detectionRuleData in ruleGroupDatas[i].ruleDatas)
                {
                    string message = $"{detectionRuleData.ruleNo}::[interval:{detectionRuleData.detectionInterval}][count:{detectionRuleData.detectionCount}][source:{detectionRuleData.Source}][destination{detectionRuleData.Destination}][protocol:{detectionRuleData.Protocol}][sourceport:{detectionRuleData.sourcePort}][destport:{detectionRuleData.destinationPort}][length:{detectionRuleData.frameLength}]";
                    detectionDatas[i].children.Add(new detectionData() { data = message,color= "IndianRed" });
                }
            }
            DataContext = detectionDatas;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.WindowState = WindowState.Minimized;
        }
        public void show_detection(packetData pd,int detectionNumber,int ruleNo)
        {
            string message = $"[No:{pd.Number}]::[source:{pd.Source}][destination{pd.Destination}][protocol:{pd.Protocol}][sourceport:{pd.sourcePort}][destport:{pd.destinationPort}][length:{pd.frameLength}]";
            detectionDatas[detectionNumber].children[ruleNo].children.Add(new detectionData() { data = message, color = "#000000" });

        }

    }
}
