﻿using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Automation.Peers;
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
    /// ruleg_detail.xaml の相互作用ロジック
    /// </summary>
    public partial class ruleg_detail : Page
    {
        private string ruleSheet = "1,1,10,5,209.152.76.123,172.0.0.1,TCP,80,8080,300000";

        public ruleg_detail()
        {
            InitializeComponent();
        }

        private void AddToDatagrid()
        {
            var data = new ruleData(ruleSheet,1,1);
            var ruleList = new List<App.ruleData>();
            ruleList.Add(data);
            rule_DataGrid.ItemsSource = ruleList;
        }
        public ruleg_detail(int no ,String name, List<ruleData> ruledata)
        {
            InitializeComponent();
            AddToDatagrid();
            title.Content = $"{title.Content} - {name}";
            foreach (ruleData rd in ruledata)
            {
                Debug.WriteLine($"{rd}");
            }
            
        }



        private void editButton_Click(object sender, RoutedEventArgs e)
        {
            // 1列だけ選択していた場合のみ
            if (rule_DataGrid.SelectedItems.Count == 1)
            {
                var selectedItem = rule_DataGrid.SelectedItem;
                if (selectedItem is ruleData ruleData)
                {
                    MessageBox.Show(ruleData.Source);
                    rule_edit_Window rule_Edit_Window = new rule_edit_Window();
                    if (rule_Edit_Window.ShowDialog() == true)
                    {
                        // OKボタンがクリックされた場合の処理
                    }
                    else
                    {
                        // キャンセルされた場合の処理
                    }
                }
                else
                {
                    // 選択された項目を ruleData 型にキャストできません
                }
            }
            else
            {
                // 1列以外が選択されている場合の処理
            }


        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            rule_add_Window rule_Add_Window = new rule_add_Window();
            if (rule_Add_Window.ShowDialog() == true)
            {

            }
            else
            {

            }
        }

        private void redoButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void inactivate_Click(object sender, RoutedEventArgs e)
        {

        }

        public class IntervalConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                if (value is int interval && parameter is int count)
                {
                    return $"{interval}秒間に{count}回";
                }
                return null;
            }

            public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }

    }
}
