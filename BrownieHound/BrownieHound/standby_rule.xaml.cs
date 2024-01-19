using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
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
using System.Windows.Threading;
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
            string message = "ルール件数が0の物を除く、\n以下のルールグループで開始しますか？\n\n";
            //メール検証処理
            HashFunction hashFunction = new HashFunction();
            Mail_Validation mailValidation = new Mail_Validation();
            using (StreamReader sr = new StreamReader(@$"conf\mail.conf", Encoding.GetEncoding("UTF-8")))
            {
                for(int i = 0; i < 4; i++)
                {
                    sr.ReadLine();
                }
                mailValidation.mailAddress.Value = sr.ReadLine().Split(":")[1];

                if (!hashFunction.verifyMail(mailValidation.mailAddress.Value, @$"conf\authorize.conf"))
                {
                    MessageBox.Show("メールアドレスが認証されておりません。\nメール設定にてメールアドレスの認証を行ってください。");
                    NavigationService.GoBack();
                    return;
                    
                }
            }

            List<ruleGroupData> detectionRuleGroups = new List<ruleGroupData>();
            foreach (ruleGroupData item in ruleGroupList.Items)
            {
                if (item.isCheck && item.ruleItems > 0)
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
                                if (detectionRuleGroups[i].ruleDatas[j].Destination.Equals("broadcast"))
                                {
                                    RuleData.ruleData broadcast = detectionRuleGroups[i].ruleDatas[j];
                                    detectionRuleGroups[i].blackListRules.Add(new RuleData.ruleData { ruleGroupNo = broadcast.ruleGroupNo, ruleNo = broadcast.ruleNo, detectionInterval = broadcast.detectionInterval, detectionCount = broadcast.detectionCount, Source = broadcast.Source, Destination = "255.255.255.255", Protocol = broadcast.Protocol, sourcePort = broadcast.sourcePort, destinationPort = broadcast.destinationPort, frameLength = broadcast.frameLength, ruleCategory = broadcast.ruleCategory });
                                    detectionRuleGroups[i].blackListRules.Add(new RuleData.ruleData { ruleGroupNo = broadcast.ruleGroupNo, ruleNo = broadcast.ruleNo, detectionInterval = broadcast.detectionInterval, detectionCount = broadcast.detectionCount, Source = broadcast.Source, Destination = "ff:ff:ff:ff:ff:ff", Protocol = broadcast.Protocol, sourcePort = broadcast.sourcePort, destinationPort = broadcast.destinationPort, frameLength = broadcast.frameLength, ruleCategory = broadcast.ruleCategory });
                                }
                                else
                                {
                                    detectionRuleGroups[i].blackListRules.Add(detectionRuleGroups[i].ruleDatas[j]);
                                }
                            }
                            else if(detectionRuleGroups[i].ruleDatas[j].ruleCategory == 1)
                            {
                                if (detectionRuleGroups[i].ruleDatas[j].Destination.Equals("broadcast"))
                                {
                                    RuleData.ruleData broadcast = detectionRuleGroups[i].ruleDatas[j];
                                    detectionRuleGroups[i].whiteListRules.Add(new RuleData.ruleData{ruleGroupNo = broadcast.ruleGroupNo,ruleNo = broadcast.ruleNo,detectionInterval = broadcast.detectionInterval,detectionCount = broadcast.detectionCount,Source = broadcast.Source,Destination = "255.255.255.255",Protocol = broadcast.Protocol,sourcePort=broadcast.sourcePort,destinationPort = broadcast.destinationPort,frameLength = broadcast.frameLength,ruleCategory = broadcast.ruleCategory});
                                    detectionRuleGroups[i].whiteListRules.Add(new RuleData.ruleData { ruleGroupNo = broadcast.ruleGroupNo, ruleNo = broadcast.ruleNo, detectionInterval = broadcast.detectionInterval, detectionCount = broadcast.detectionCount, Source = broadcast.Source, Destination = "ff:ff:ff:ff:ff:ff", Protocol = broadcast.Protocol, sourcePort = broadcast.sourcePort, destinationPort = broadcast.destinationPort, frameLength = broadcast.frameLength, ruleCategory = broadcast.ruleCategory });
                                }
                                else
                                {
                                    detectionRuleGroups[i].whiteListRules.Add(detectionRuleGroups[i].ruleDatas[j]);
                                }
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
            Show_Group(RuleGroupDataReader.Read(path));
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

        private void checkAll_Unchecked(object sender, RoutedEventArgs e)
        {
            bool allSelect;
            allSelect = (bool)checkAll.IsChecked;
            foreach (ruleGroupData item in Data)
            {
                item.isCheck = allSelect;
                checkAll.Content = "すべて選択";

            }
            ruleGroupList.ItemsSource = null;
            ruleGroupList.ItemsSource = Data;
        }

        private void checkAll_Checked(object sender, RoutedEventArgs e)
        {
            bool allSelect;
            allSelect = (bool)checkAll.IsChecked;
            foreach (ruleGroupData item in Data)
            {
                item.isCheck = allSelect;
                checkAll.Content = "すべて選択解除";

            }
            ruleGroupList.ItemsSource = null;
            ruleGroupList.ItemsSource = Data;
        }
        private void ListViewItem_Selected(object sender, RoutedEventArgs e)
        {
            ruleGroupDetail.IsEnabled = true;
        }
    }
}
