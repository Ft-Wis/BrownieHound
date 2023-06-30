using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
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

        private string fileName;
        private List<ruleData> ruledata;

        public ruleg_detail(int no ,String name, List<ruleData> ruledata)
        {
            InitializeComponent();
            title.Content = $"{title.Content} - {name}";
            fileName = name;
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
                //MessageBox.Show(receivedData.protocol+receivedData.source+receivedData.destination);
                string filePath = "./ruleGroup/"+fileName+".txt";
                int editLineNumber = receivedData.ruleNo - 1;
                string insertText = exchangeText(receivedData);
                //MessageBox.Show(insertText);

                RemoveAndInsertLine(filePath,editLineNumber,insertText);
                ReadFileByLine(filePath);

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

        private void ReadFileByLine(string filePath)
        {
            if (File.Exists(filePath))
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        Debug.WriteLine(line);
                        
                    }
                }
            }
            else
            {
                Console.WriteLine("ファイルが存在しません。");
            }
        }

        private void Draw()
        {
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

        private string exchangeText(DataGridItem originalData)
        {
            string exchangeText="";

            exchangeText += originalData.detectionInterval + ",";
            exchangeText += originalData.detectionCount + ",";
            exchangeText += originalData.source + ",";
            exchangeText += originalData.destination + ",";
            exchangeText += originalData.protocol + ",";
            exchangeText += originalData.sourcePort + ",";
            exchangeText += originalData.frameLength;

            return exchangeText;
        }

        static void RemoveAndInsertLine(string filePath,int lineNumber,string insertText)
        {
            string[]  lines=File.ReadAllLines(filePath);
            if (lineNumber >= 1 && lineNumber <= lines.Length)
            {
                // 行の削除
                lines = RemoveLine(lines, lineNumber);
                // 行の挿入
                lines = InsertLine(lines, lineNumber, insertText);
                // ファイルを上書きする
                File.WriteAllLines(filePath, lines);
            }
            else
            {

            }
        }

        static string[] RemoveLine(string[] lines, int lineNumber)
        {
            // 指定された行を削除して新しい配列を作成
            string[] newLines = new string[lines.Length - 1];

            int currentIndex = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                // 削除する行以外を新しい配列に追加
                if (i + 1 != lineNumber)  
                {
                    newLines[currentIndex] = lines[i];
                    currentIndex++;
                }
            }

            return newLines;
        }

        static string[] PackLines(string[] lines)
        {
            // 行を詰めるために、削除された行以降の行を1つずつ前に詰める
            for (int i = 0; i < lines.Length; i++)
            {
                if (string.IsNullOrEmpty(lines[i]))
                {
                    int j = i + 1;
                    while (j < lines.Length && string.IsNullOrEmpty(lines[j]))
                    {
                        j++;
                    }
                    if (j < lines.Length)
                    {
                        lines[i] = lines[j];
                        lines[j] = null;
                    }
                }
            }

            return lines;
        }

        static string[] InsertLine(string[] lines, int lineNumber, string insertText)
        {
            // 指定された行にテキストを挿入して新しい配列を作成
            string[] newLines = new string[lines.Length + 1];

            int currentIndex = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                // 挿入する行の位置にテキストを挿入
                if (i + 1 == lineNumber)  
                {
                    newLines[currentIndex] = insertText;
                    currentIndex++;
                }
                newLines[currentIndex] = lines[i];
                currentIndex++;
            }

            return newLines;
        }


    }
}
