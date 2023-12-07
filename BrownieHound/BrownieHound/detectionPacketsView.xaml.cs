using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BrownieHound
{
    /// <summary>
    /// detectionPacketsView.xaml の相互作用ロジック
    /// </summary>
    public partial class detectionPacketsView : Page
    {
        public detectionPacketsView(string pathToFolder)
        {
            InitializeComponent();
            System.Windows.MessageBox.Show($"{pathToFolder}を選択しました");

            //コンボボックスのアイテムにファイル名を代入
            string[] folderNames = GetFolderNames(pathToFolder);
            foreach (string folderName in folderNames)
            {
                System.Windows.MessageBox.Show($"{folderName}");
                filenameComboBox.Items.Add(System.IO.Path.GetFileName(folderName));
            }

            //一番上を選択しておき、読み込み開始
            filenameComboBox.SelectedIndex = 0;

        }

        private string[] GetFileNames(string folderPath)
        {
            try
            {
                // フォルダが存在するか確認
                if (Directory.Exists(folderPath))
                {
                    // フォルダ内のすべてのファイル名を取得
                    string[] fileNames = Directory.GetFiles(folderPath);
                    return fileNames;
                }
                else
                {
                    System.Windows.MessageBox.Show("指定されたフォルダは存在しません。");
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"エラーが発生しました: {ex.Message}");
            }

            return new string[0]; // エラーが発生した場合は空の配列を返す
        }

        private string[] GetFolderNames(string folderPath)
        {
            try
            {
                // フォルダが存在するか確認
                if (Directory.Exists(folderPath))
                {
                    // フォルダ内のすべてのサブフォルダ名を取得
                    string[] folderNames = Directory.GetDirectories(folderPath);
                    return folderNames;
                }
                else
                {
                    System.Windows.MessageBox.Show("指定されたフォルダは存在しません。");
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"エラーが発生しました: {ex.Message}");
            }

            return new string[0]; // エラーが発生した場合は空の配列を返す
        }

        private void redoButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

    }
}
