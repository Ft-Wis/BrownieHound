using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private string ruleSheet = "1,5,209.152.76.123,172.0.0.1,TCP,80,8080,300000";

        public struct DataGridItem
        {
            public bool isCheck { get; set; }
            public int ruleNo { get; set; }
            public int detectionInterval { get; set; }
            public int detectionCount { get; set; }
            public string source { get; set; }
            public string protocol { get; set; }
            public string destination { get; set; }
            public string sourcePort { get; set; }
            public string port { get; set; }
            public int frameLength { get; set; }
        }

        ObservableCollection<DataGridItem> gridItem;

        public ruleg_detail()
        {
            InitializeComponent();
        }

        private void AddToDatagrid()
        {
            var data = new ruleData(ruleSheet,1,1);
            gridItem = new ObservableCollection<DataGridItem>();
            var gridData = new DataGridItem
            { 
                isCheck = false,
                ruleNo=data.ruleNo,
                detectionInterval=data.detectionInterval,
                detectionCount=data.detectionCount,
                source=data.Source,
                protocol=data.Protocol,
                port=data.sourcePort,
                destination=data.Destination,
                frameLength=data.frameLength

            };
            gridItem.Add(gridData);
            rule_DataGrid.ItemsSource = gridItem;
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
            DataGridItem data = new DataGridItem {
                ruleNo = 1,
                source = "209.152.76.123",
                destination = "172.0.0.1",
                protocol = "TCP",
                sourcePort = "80",
                port = "8080",
                frameLength = 300000,
                detectionInterval = 1,
                detectionCount  = 5
            };

            showPopup(data);

            // 1列だけ選択していた場合のみ
            if (rule_DataGrid.SelectedItems.Count == 1)
            {
                var selectedItem = rule_DataGrid.SelectedItem;
                if (selectedItem is ruleData ruleData)
                {
                    

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
            showPopup();
        }

        private void redoButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void inactivate_Click(object sender, RoutedEventArgs e)
        {

        }

        private void showPopup(DataGridItem sendData)
        {
            rule_edit_Window rule_Edit_Window = new rule_edit_Window(sendData);
            
            if (rule_Edit_Window.ShowDialog() == true)
            {
                // OKボタンがクリックされた場合の処理
                DataGridItem receivedData = rule_Edit_Window.sendData;
                MessageBox.Show(receivedData.destination);
            }
            else
            {
                // キャンセルされた場合の処理
            }
        }

        private void showPopup()
        {
            rule_add_Window ruleAddWin= new rule_add_Window();
            if(ruleAddWin.ShowDialog() == true)
            {
                // OKボタンがクリックされた場合の処理
            }
            else
            {
                // キャンセルされた場合の処理
            }
        }
    }
}
