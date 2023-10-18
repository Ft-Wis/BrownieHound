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
using System.Windows.Media.Animation;
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
            ruleg_add_Window addWindow = new ruleg_add_Window();
            if(addWindow.ShowDialog() == true ) 
            {
                string newGroupName = addWindow.newGroupName;
                if (newGroupName.Length != 0)
                {
                    for (int i = 1; File.Exists($"{path}\\{newGroupName}.txt");i++)
                    {
                        if (!File.Exists($"{path}\\{newGroupName} - {i}.txt"))
                        {
                            newGroupName = $"{newGroupName} - {i}";
                            break;
                        }
                    }
                    using (File.Create($"{path}\\{newGroupName}.txt")) { }
                    MessageBox.Show($"以下のルールグループを追加しました。\n{newGroupName}", "インフォメーション", MessageBoxButton.OK, MessageBoxImage.Information);
                    Show_Group(RuleGroupDataReader.Read(path));
                }
                else
                {
                    MessageBox.Show("ルールグループの名前を\n入力してください。", "!警告!",MessageBoxButton.OK,MessageBoxImage.Error);
                }
            }
        }

        private void detailButton_Click(object sender, RoutedEventArgs e)
        {
            if(ruleGroupList.SelectedItems.Count == 1) 
            {
                ruleGroupData listViewItems = (ruleGroupData)ruleGroupList.SelectedItem;
                Debug.WriteLine(listViewItems.Name);
                var nextPage = new ruleg_detail(listViewItems.No, listViewItems.Name, listViewItems.ruleDatas);
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

            Show_Group(RuleGroupDataReader.Read(path));

        }
        private void Show_Group(List<ruleGroupData> datas)
        {
            Data = new ObservableCollection<ruleGroupData>();
            foreach (ruleGroupData data in datas)
            {
                Data.Add(data); 
            }
            ruleGroupList.DataContext = Data;
        }

        private void ListViewItem_DoubleClikck(object sender, MouseButtonEventArgs e)
        {
            var listViewItems = sender as ListViewItem;
            ruleGroupData rgData = listViewItems.DataContext as ruleGroupData;
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
                        
                    }
                    Show_Group(RuleGroupDataReader.Read(path));
                }
            }
            else
            {
                MessageBox.Show("ルールグループが選択されていません", "!警告!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
