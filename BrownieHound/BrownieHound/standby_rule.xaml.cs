using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static BrownieHound.App;
using ListViewItem = System.Windows.Controls.ListViewItem;
using MessageBox = System.Windows.MessageBox;

namespace BrownieHound
{
    /// <summary>
    /// standby_rule.xaml の相互作用ロジック
    /// </summary>
    public partial class standby_rule : Page
    {
        public standby_rule(string tsInterface)
        {
            InitializeComponent();
            this.interfaceLabel.Content = tsInterface;
        }
        ObservableCollection<ruleGroupData> Data;
        string path = @"ruleGroup";
        private void s_rTotop_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void activate_Click(object sender, RoutedEventArgs e)
        {
            string interfaceText = interfaceLabel.Content.ToString();
            string interfaceNumber = interfaceText.Substring(0, interfaceText.IndexOf("."));
            string message = "以下のルールグループで開始しますか？\n";
            List<ruleGroupData> detectionRuleGroups = new List<ruleGroupData>();
            foreach (ruleGroupData item in ruleGroupList.Items)
            {
                if (item.isCheck)
                {
                    message += $"{item.No}:{item.Name}\n";
                    detectionRuleGroups.Add(item);
                }
            }
            if (detectionRuleGroups.Count > 0)
            {
                IPAddress[] addresses = Dns.GetHostAddresses(Dns.GetHostName());
                IPAddress myAddress = addresses[0];
                foreach(IPAddress address in addresses)
                {
                    if(address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        myAddress = address;
                    }
                }
                MessageBoxResult result = MessageBox.Show(message, "起動確認", MessageBoxButton.OKCancel, MessageBoxImage.Information);
                if (result == MessageBoxResult.OK)
                {
                    for(int i = 0;i < detectionRuleGroups.Count; i++)
                    {
                        for(int j = 0;j < detectionRuleGroups[i].ruleDatas.Count;j++)
                        {
                            if(detectionRuleGroups[i].ruleDatas[j].ruleCategory == 0)
                            {
                                detectionRuleGroups[i].blackListRules.Add(detectionRuleGroups[i].ruleDatas[j]);
                            }
                            else if(detectionRuleGroups[i].ruleDatas[j].ruleCategory == 1)
                            {
                                detectionRuleGroups[i].whiteListRules.Add(detectionRuleGroups[i].ruleDatas[j]);
                            }
                            detectionRuleGroups[i].ruleDatas[j].Source = detectionRuleGroups[i].ruleDatas[j].Source.Replace("myAddress",myAddress.ToString());
                            detectionRuleGroups[i].ruleDatas[j].Destination = detectionRuleGroups[i].ruleDatas[j].Destination.Replace("myAddress", myAddress.ToString());
                        }
                    }
                    var nextPage = new capture(interfaceNumber,detectionRuleGroups);
                    NavigationService.Navigate(nextPage);
                }
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("ルールグループが選択されていません\nこのまま続けますか？", "!警告!", MessageBoxButton.OKCancel, MessageBoxImage.Warning);

                if (result == MessageBoxResult.OK) 
                {
                    var nextPage = new capture(interfaceNumber);
                    NavigationService.Navigate(nextPage);
                }
            }
        }
        private void Show_Group(List<ruleGroupData> datas)
        {
            foreach (ruleGroupData data in datas)
            {
                Data.Add(data);
            }
            ruleGroupList.DataContext = Data;
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Data = new ObservableCollection<ruleGroupData>();
            Show_Group(Read(path));
        }

        private void ruleGroupDetail_Click(object sender, RoutedEventArgs e)
        {
            if (ruleGroupList.SelectedItems.Count == 1)
            {
                ruleGroupData lvi = (ruleGroupData)ruleGroupList.SelectedItem;
                Debug.WriteLine(lvi.Name);
                var nextPage = new ruleg_detail(lvi.No, lvi.Name, lvi.ruleDatas);
                NavigationService.Navigate(nextPage);
            }
        }

        private void ListViewItem_DoubleClikck(object sender, MouseButtonEventArgs e)
        {
            var lvi = sender as ListViewItem;
            ruleGroupData rgData = lvi.DataContext as ruleGroupData;
            var nextPage = new ruleg_detail(rgData.No, rgData.Name, rgData.ruleDatas);
            NavigationService.Navigate(nextPage);
        }
    }
}
