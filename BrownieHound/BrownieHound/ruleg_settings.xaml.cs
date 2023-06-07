using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace BrownieHound
{
    /// <summary>
    /// ruleg_settings.xaml の相互作用ロジック
    /// </summary>
    public partial class ruleg_settings : Page
    {
        public ruleg_settings()
        {
            InitializeComponent();
        }
        public class ruleGroupData
        {
            public int No { get; set; }
            public String Name { get; set; }
            public int ruleItems { get; set; }
            public String[] rules { get; set; }

            public ruleGroupData(int no, String name, int ruleItems,String ruledata)
            {
                this.No = no;
                this.Name = name;
                this.ruleItems = ruleItems;
                this.rules = ruledata.Split(':');
            }
        }
        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            var nextPage = new rule_edit();
            NavigationService.Navigate(nextPage);
        }

        private void detailButton_Click(object sender, RoutedEventArgs e)
        {
            if(ruleGroupList.SelectedItems.Count == 1) 
            {
                ruleGroupData lvi = (ruleGroupData)ruleGroupList.SelectedItems[0];
                Debug.WriteLine(lvi.Name);
                var nextPage = new ruleg_detail(lvi.No,lvi.Name,lvi.rules);
                NavigationService.Navigate(nextPage);
            }

        }

        private void inactivate_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void redoButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
        ObservableCollection<ruleGroupData> Data = new ObservableCollection<ruleGroupData>();
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            for(int i = 0; i < 10; i++)
            {
                Data.Add(new ruleGroupData(i, "aaaa" + i, 5, "0,60,1,8.8.8.8,,,,,0:0,60,1,9.8.8.8,,,,,0:0,60,1,10.8.8.8,,tcp,80,,0"));
                ruleGroupList.DataContext = Data;
            }
        }

        private void ListViewItem_DoubleClikck(object sender, MouseButtonEventArgs e)
        {
            var lvi = sender as ListViewItem;
            ruleGroupData rgData = lvi.DataContext as ruleGroupData;
            var nextPage = new ruleg_detail(rgData.No, rgData.Name,rgData.rules);
            NavigationService.Navigate(nextPage);
        }
    }
}
