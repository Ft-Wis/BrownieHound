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
using System.Windows.Markup;
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
            public string destinationPort { get; set; }
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
                sourcePort=data.sourcePort,
                destination=data.Destination,
                frameLength=data.frameLength

            };
            gridItem.Add(gridData);
            rule_DataGrid.ItemsSource = gridItem;
        }
        public ruleg_detail(int no ,String name, List<ruleData> ruledata)
        {
            InitializeComponent();
            //AddToDatagrid();
            title.Content = $"{title.Content} - {name}";
            gridItem = new ObservableCollection<DataGridItem>();
            foreach (ruleData rd in ruledata)
            {
                var gridData = new DataGridItem
                {
                    isCheck = false,
                    ruleNo = rd.ruleNo,
                    detectionInterval = rd.detectionInterval,
                    detectionCount = rd.detectionCount,
                    source = rd.Source,
                    protocol = rd.Protocol,
                    sourcePort = rd.sourcePort,
                    destination = rd.Destination,
                    frameLength = rd.frameLength
                };
                gridItem.Add(gridData);
            }

            rule_DataGrid.ItemsSource = gridItem;

        }



        private void editButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedItems = rule_DataGrid.SelectedItems;
            if (selectedItems.Count==1)
            {
                var selectedGridItem = (DataGridItem)rule_DataGrid.SelectedItem;

                DataGridItem data = new DataGridItem 
                {
                    ruleNo = selectedGridItem.ruleNo,
                    source = selectedGridItem.source,
                    destination = selectedGridItem.destination,
                    protocol = selectedGridItem.protocol,
                    sourcePort = selectedGridItem.sourcePort,
                    destinationPort=selectedGridItem.destinationPort,
                    frameLength=selectedGridItem.frameLength,
                    detectionInterval=selectedGridItem.detectionInterval,
                    detectionCount = selectedGridItem.detectionCount
                };

                //{
                //    ruleNo = 1,
                //    source = "209.152.76.123",
                //    destination = "172.0.0.1",
                //    protocol = "TCP",
                //    sourcePort = "80",
                //    port = "8080",
                //    frameLength = 300000,
                //    detectionInterval = 1,
                //    detectionCount = 5
                //};
                showPopup(data);
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

        //「編集」ボタンを押したとき
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

        //「追加」ボタンを押したとき
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
