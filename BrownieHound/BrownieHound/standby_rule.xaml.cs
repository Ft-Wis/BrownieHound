using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static BrownieHound.App;

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
            var nextPage = new top();
            NavigationService.Navigate(nextPage);
        }

        private void activate_Click(object sender, RoutedEventArgs e)
        {
            string interfaceText = interfaceLabel.Content.ToString();
            string interfaceNumber = interfaceText.Substring(0,interfaceText.IndexOf("."));
            
            var nextPage = new capture(interfaceNumber);
            NavigationService.Navigate(nextPage);
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
    }
}
