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
using System.Windows.Controls.Primitives;
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
            public string ruleCategory { get; set; }
        }

        ObservableCollection<DataGridItem> gridItem;

        private string fileName;

        public ruleg_detail(int no ,String name, List<RuleData.ruleData> ruledata)
        {
            Application.Current.MainWindow.Width = 1200;
            InitializeComponent();
            title.Content = $"{title.Content} - {name}";
            fileName = name;
            gridItem = new ObservableCollection<DataGridItem>();
            
            foreach (RuleData.ruleData rd in ruledata)
            {
                DataGridItem gridData = new DataGridItem
                {
                    isCheck = false,
                    ruleNo = rd.ruleNo,
                    detectionInterval = rd.detectionInterval,
                    detectionCount = rd.detectionCount,
                    source = rd.Source,
                    protocol = rd.Protocol,
                    sourcePort = rd.sourcePort,
                    destinationPort = rd.destinationPort,
                    destination = rd.Destination,
                    frameLength = rd.frameLength,
                    ruleCategory = rd.ruleCategory.ToString()
                };

                if (gridData.ruleCategory=="0")
                {
                    gridData.ruleCategory = "検出";
                }
                else
                {
                    gridData.ruleCategory = "否検出";
                }
                rule_DataGrid.Items.Add(gridData);
            }

            //rule_DataGrid.ItemsSource= gridItem;
            
        }

        private void editButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedItems = rule_DataGrid.SelectedItems;
            if (selectedItems.Count==1)
            {
                var selectedGridItem = (DataGridItem)rule_DataGrid.SelectedItem;

                DataGridItem data = new DataGridItem 
                {
                    ruleCategory = selectedGridItem.ruleCategory,
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
            Application.Current.MainWindow.Width = 800;
        }

        //「削除」ボタンを押したとき
        private void inactivate_Click(object sender, RoutedEventArgs e)
        {
            var selectedItems = rule_DataGrid.SelectedItems;
            int[] selectedRules = GetSelectedRuleNumbers();
            string filePath = "./ruleGroup/" + fileName + ".txt";
            //チェックボックスで選んだ時
            if (selectedRules.Length > 0)
            {
                string[] targetFile = File.ReadAllLines(filePath);
                for (int i = selectedRules.Length - 1; i >= 0; i--)
                {
                    targetFile = RemoveLine(targetFile, selectedRules[i]);
                }

                File.WriteAllLines(filePath, targetFile);
                reDraw();
            }
            reDraw();
        }

        //「編集」ボタンを押したとき
        private void showPopup(DataGridItem sendData)
        {
            rule_edit_Window rule_Edit_Window = new rule_edit_Window(sendData);
            
            if (rule_Edit_Window.ShowDialog() == true)
            {
                // OKボタンがクリックされた場合の処理
                DataGridItem receivedData = rule_Edit_Window.sendData;
                string filePath = "./ruleGroup/"+fileName+".txt";
                int editLineNumber = receivedData.ruleNo;
                string insertText = exchangeText(receivedData);

                //RemoveAndInsertLine(filePath,editLineNumber,insertText);
                replaceLine(filePath,editLineNumber,insertText);
                ReadFileByLine(filePath);
                reDraw();
            }
            else
            {
                // キャンセルされた場合の処理
            }
        }

        //「追加」ボタンを押したとき
        private void showPopup()
        {
            int newRuleNo = rule_DataGrid.Items.Count;
            rule_add_Window ruleAddWin= new rule_add_Window(newRuleNo);
            if(ruleAddWin.ShowDialog() == true)
            {
                // OKボタンがクリックされた場合の処理
                DataGridItem addData = ruleAddWin.sendData;
                string filePath = "./ruleGroup/" + fileName + ".txt";
                string addText = exchangeText(addData);
                string[] lines = File.ReadAllLines(filePath);
                string[] result = addLine(lines,addText);
                File.WriteAllLines(filePath, result);
                reDraw();
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
                        
                    }
                }
            }
            else
            {
                Console.WriteLine("ファイルが存在しません。");
            }
        }

        private void reDraw()
        {
            string filePath = "./ruleGroup/" + fileName + ".txt";
            string[] lines = File.ReadAllLines(filePath);
            rule_DataGrid.Items.Clear(); // ObservableCollection の Clear メソッドを呼び出すだけで十分です
            for (int ruleNum = 0; ruleNum < lines.Length; ruleNum++)
            {
                RuleData.ruleData rd = new RuleData.ruleData(lines[ruleNum]);
                var gridData = new DataGridItem
                {
                    isCheck = false,
                    ruleNo = ruleNum,
                    detectionInterval = rd.detectionInterval,
                    detectionCount = rd.detectionCount,
                    source = rd.Source,
                    protocol = rd.Protocol,
                    sourcePort = rd.sourcePort,
                    destinationPort = rd.destinationPort,
                    destination = rd.Destination,
                    frameLength = rd.frameLength,
                    ruleCategory = rd.ruleCategory.ToString()
                };
                if(gridData.ruleCategory == "0")
                {
                    gridData.ruleCategory = "検出";
                }
                else
                {
                    gridData.ruleCategory = "否検出";
                }
                rule_DataGrid.Items.Add(gridData);
            }
        }

        private string exchangeText(DataGridItem originalData)
        {
            string exchangeText="";

            exchangeText += originalData.ruleCategory + ",";
            exchangeText += originalData.detectionInterval + ",";
            exchangeText += originalData.detectionCount + ",";
            exchangeText += originalData.source + ",";
            exchangeText += originalData.destination + ",";
            exchangeText += originalData.protocol + ",";
            exchangeText += originalData.sourcePort + ",";
            exchangeText += originalData.destinationPort + ",";
            exchangeText += originalData.frameLength;

            return exchangeText;
        }

        static void replaceLine(string filePath,int lineNumber,string insertText)
        {
            string[] lines=File.ReadAllLines(filePath);
            if (lineNumber >= 0 && lineNumber <= lines.Length)
            {
                lines[lineNumber] = insertText;
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
                if (i != lineNumber)  
                {
                    newLines[currentIndex] = lines[i];
                    currentIndex++;
                }
            }

            return newLines;
        }

        static string[] addLine(string[] lines, string insertText)
        {
            // 指定された行にテキストを挿入して新しい配列を作成
            string[] newLines = new string[lines.Length + 1];
            int lastIndex=lines.Length;

            for (int i = 0; i < lines.Length; i++)
            {
                newLines[i] = lines[i];
            }
            newLines[lastIndex]= insertText;

            return newLines;
        }

        private int[] GetSelectedRuleNumbers()
        {
            List<int> selectedRuleNumbers = new List<int>();

            foreach (DataGridItem item in rule_DataGrid.Items)
            {
                DataGridItem result=(DataGridItem)rule_DataGrid.Items[0];
                if (item.isCheck)
                {
                    selectedRuleNumbers.Add(item.ruleNo);
                }
            }

            int[] selectedRuleNoArray = selectedRuleNumbers.ToArray();

            return selectedRuleNoArray;
            // selectedRuleNoArrayを適切に使用する処理を記述してください
        }
        int checkCount = 0;
        private void DataGrid_Selected(object sender, RoutedEventArgs e)
        {
            edit.IsEnabled = true;
        }
        private void IsChecked_Checked(object sender, RoutedEventArgs e)
        {
            inactivate.IsEnabled = true;
            checkCount++;
        }
        private void IsChecked_Unchecked(object sender, RoutedEventArgs e)
        {
            checkCount--;
            if (checkCount == 0)
                inactivate.IsEnabled = false;
        }
    }
}
