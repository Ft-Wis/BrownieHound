﻿using System;
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

                using (StreamWriter sw = new StreamWriter($"{path}\\{newGroupName}.txt"))
                {
                    sw.WriteLine("StandardRule");
                }
                MessageBox.Show($"以下のルールグループを追加しました。\n{newGroupName}", "インフォメーション", MessageBoxButton.OK, MessageBoxImage.Information);
                Show_Group(RuleGroupDataReader.Read(path));
            }
        }

        private void detailButton_Click(object sender, RoutedEventArgs e)
        {
            if(ruleGroupList.SelectedItems.Count == 1) 
            {
                ruleGroupData listViewItems = (ruleGroupData)ruleGroupList.SelectedItem;
                Debug.WriteLine(listViewItems.Name);
                var nextPage = new ruleg_detail(listViewItems.No, listViewItems.Name, listViewItems.RuleDatas);
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
            ruleGroupList.ItemsSource = Data;
        }

        private void ListViewItem_DoubleClikck(object sender, MouseButtonEventArgs e)
        {
            var listViewItems = sender as ListViewItem;
            ruleGroupData rgData = listViewItems.DataContext as ruleGroupData;
            var nextPage = new ruleg_detail(rgData.No, rgData.Name,rgData.RuleDatas);
            NavigationService.Navigate(nextPage);
        }

        private void delete_Click(object sender, RoutedEventArgs e)
        {
            delete.IsEnabled = false;
            string message = "以下のルールグループを削除しますか？\n";
            List<string> ruleGroupNames = new List<string>();
            foreach (ruleGroupData item in ruleGroupList.Items)
            {
                if (item.IsCheck)
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



        private void checkAll_Unchecked(object sender, RoutedEventArgs e)
        {
            bool allSelect;
            allSelect = (bool)checkAll.IsChecked;
            foreach (ruleGroupData item in Data)
            {
                item.IsCheck = allSelect;
                checkAll.Content = "すべて選択";
                
            }
            ruleGroupList.ItemsSource = null;
            ruleGroupList.ItemsSource = Data;
            checkCount = 0;
            delete.IsEnabled = false;
        }

        

        private void checkAll_Checked(object sender, RoutedEventArgs e)
        {
            bool allSelect;
            allSelect = (bool)checkAll.IsChecked;
            foreach (ruleGroupData item in Data)
            {
                item.IsCheck = allSelect;
                checkAll.Content = "すべて選択解除";

            }
            ruleGroupList.ItemsSource = null;
            ruleGroupList.ItemsSource = Data;
            checkCount = ruleGroupList.Items.Count;
            if (checkCount > 0)
            {
                delete.IsEnabled = true;
            }
            else
            {
                delete.IsEnabled = false;
            }
        }
        int checkCount = 0;
        private void ListViewItem_Selected(object sender, RoutedEventArgs e)
        {
            detail.IsEnabled = true;
        }
        private void IsChecked_Checked(object sender, RoutedEventArgs e)
        {
            delete.IsEnabled = true;
            checkCount++;
        }
        private void IsChecked_Unchecked(object sender, RoutedEventArgs e)
        {
            checkCount--;
            if(checkCount == 0)
                delete.IsEnabled = false;
        }

    }
}
