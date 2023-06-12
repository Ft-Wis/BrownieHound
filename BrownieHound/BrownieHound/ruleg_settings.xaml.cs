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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static BrownieHound.App;

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
        
        private void addButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void detailButton_Click(object sender, RoutedEventArgs e)
        {
            if(ruleGroupList.SelectedItems.Count == 1) 
            {
                ruleGroupData lvi = (ruleGroupData)ruleGroupList.SelectedItem;
                Debug.WriteLine(lvi.Name);
                var nextPage = new ruleg_detail(lvi.No, lvi.Name, lvi.ruleDatas);
                NavigationService.Navigate(nextPage);
            }

        }


        private void redoButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
        ObservableCollection<ruleGroupData> Data;
        string path = @"ruleGroup";
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ruleGroupRead();

        }
        private void ruleGroupRead()
        {
            Data = new ObservableCollection<ruleGroupData>();

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(path);
            IEnumerable<System.IO.FileInfo> ruleFiles = di.EnumerateFiles("*.txt", System.IO.SearchOption.TopDirectoryOnly);
            foreach (var ruleFile in ruleFiles.Select((Value, Index) => new { Value, Index }))
            {
                string ruleGroupName = ruleFile.Value.Name.Remove(ruleFile.Value.Name.Length - 4);
                ruleGroupData ruleGroup = new ruleGroupData(ruleFile.Index, ruleGroupName);
                string filePath = $"{path}\\{ruleFile.Value.Name}";
                StreamReader sr = new StreamReader(filePath, Encoding.GetEncoding("UTF-8"));

                while (sr.Peek() != -1)
                {
                    ruleGroup.ruleSet(sr.ReadLine());
                }

                Data.Add(ruleGroup);
                ruleGroupList.DataContext = Data;

            }
        }

        private void ListViewItem_DoubleClikck(object sender, MouseButtonEventArgs e)
        {
            var lvi = sender as ListViewItem;
            ruleGroupData rgData = lvi.DataContext as ruleGroupData;
            var nextPage = new ruleg_detail(rgData.No, rgData.Name,rgData.ruleDatas);
            NavigationService.Navigate(nextPage);
        }

        private void delete_Click(object sender, RoutedEventArgs e)
        {
            string message = "以下のルールグループを削除しますか？\n";
            List<string> ruleGroupNames = new List<string>();
            foreach (ruleGroupData item in ruleGroupList.Items)
            {
                if (item.isCheck)
                {
                    message += $"{item.No}:{item.Name}\n";
                    ruleGroupNames.Add(item.Name);
                }
            }
            if (ruleGroupNames.Count > 0)
            {
                MessageBoxResult result = MessageBox.Show(message, "削除確認", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                if (result == MessageBoxResult.OK)
                {
                    foreach(string ruleGroupName in ruleGroupNames)
                    {
                        FileInfo file = new FileInfo($"{path}\\{ruleGroupName}.txt");
                        file.Delete();
                        ruleGroupRead();
                    }
                }
            }
            else
            {
                MessageBox.Show("ルールグループが選択されていません", "!警告!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
