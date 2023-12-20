﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static BrownieHound.ReadPacketData;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace BrownieHound
{
    /// <summary>
    /// detectionPacketsView.xaml の相互作用ロジック
    /// </summary>
    public partial class detectionPacketsView : Page
    {
        private string pathToFolder;
        public detectionPacketsView(string path)
        {
            InitializeComponent();
            Application.Current.MainWindow.Width = 850;
            this.pathToFolder = path;

            //コンボボックスのアイテムにファイル名を代入
            string[] folderNames = GetFolderNames(pathToFolder);
            foreach (string folderName in folderNames)
            {
                filenameComboBox.Items.Add(System.IO.Path.GetFileName(folderName));
            }

            //一番上を選択しておき、読み込み開始
            filenameComboBox.SelectedIndex = 0;
        }

        private void  showDetectionPackets(string filePath)
        {
            if(File.Exists(filePath))
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    string line = "";
                    List<String> detectionPackets = new List<String> { };
                    int i = 0;

                    while ((line = sr.ReadLine()) != null)
                    {
                        detectionPackets.Add(line);
                        packetData pd;
                        try
                        {
                            JObject packetObject = JObject.Parse(detectionPackets[i++]);
                            pd = new packetData(packetObject);
                            detectionPacketDataGrid.Items.Add(pd);
                        }
                        catch(Exception ex)
                        {
                            MessageBox.Show($"ファイル形式が正しくないため読み込みができませんでした。: {ex.Message}");
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("ファイルが存在しませんでした。もう一度お試しください。");
                redo();
            }

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

        //戻るボタンを押したとき
        private void redoButton_Click(object sender, RoutedEventArgs e)
        {
            redo();
        }

        //top画面に戻る処理
        private void redo()
        {
            closeMainWindow();
            NavigationService.GoBack();
            Application.Current.MainWindow.Width = 800;
        }

        private void closeMainWindow()
        {
            List<Window> windows = App.Current.Windows.Cast<Window>().ToList();
            Window mainWindow = windows.FirstOrDefault(window => window is MainWindow);
            foreach (Window window in windows)
            {
                if (window != mainWindow)
                {
                    window.Close();
                }
            }
        }

        //コンボボックスの選択肢が変更されたとき
        private void filenameComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedFileName = filenameComboBox.SelectedItem.ToString();
            string pathToSelectedFile = pathToFolder + "\\" + selectedFileName + "\\" + selectedFileName + ".txt";
            detectionPacketDataGrid.Items.Clear();
            showDetectionPackets(pathToSelectedFile);
        }

        //DataGridの行をダブルクリックしたときにパケットの詳細画面を表示する
        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            packetData packet = (packetData)detectionPacketDataGrid.SelectedItem;
            if (packet != null)
            {
                packet_detail_Window packet_detail = new packet_detail_Window(packet.Data);
                packet_detail.Show();
            }
        }
    }
}
