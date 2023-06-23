﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static BrownieHound.ruleg_detail;
using System.Text.RegularExpressions;
using System.Drawing.Imaging;

namespace BrownieHound
{
    /// <summary>
    /// rule_edit_Window.xaml の相互作用ロジック
    /// </summary>
    public partial class rule_edit_Window : Window
    {
        private DataGridItem ruleItem;
        public DataGridItem sendData;

        //ipアドレスの正規表現
        private static readonly string ipAddressPattern =@"^(?:[0-9]{1,3}\.){3}[0-9]{1,3}$";
        //選択肢
        private string[] tcpChoiced = { "すべて", "HTTP(80)", "HTTPS(443)", "手動で設定" };
        private string[] udpChoiced = { "すべて", "SNMP(162)", "DNS(53)", "手動で設定" };
        private string[] otherChoiced = { "すべて", "HTTP(80)", "HTTPS(443)", "SNMP(162)", "DNS(53)", "手動で設定" };


        public rule_edit_Window(DataGridItem receivedData)
        {
            InitializeComponent();
            this.ruleItem = receivedData;

            setItem();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void editButton_Click(object sender, RoutedEventArgs e)
        {
            setSendData();
            this.DialogResult = true;
        }

        private void setItem()
        {
            //送信元IPアドレスに値を代入
            if(judgeIPAdress(ruleItem.source))
            {
                //選択肢を「手動で設定」にする
                sourceComboBox.SelectedIndex = 2;
                sourceTextBox.IsEnabled = true;
                sourceTextBox.Text = ruleItem.source;
            }
            else
            { 
                if(ruleItem.source == "allIP")
                {
                    //選択肢を「すべてのIP」にする
                    sourceComboBox.SelectedIndex = 0;
                }
                else
                {
                    //選択肢を「このPCのアドレス」にする
                    sourceComboBox.SelectedIndex = 1;
                }
            }

            //送信先IPアドレスに値を代入
            if (judgeIPAdress(ruleItem.destination))
            {
                //選択肢を「手動で設定」にする
                destinationComboBox.SelectedIndex = 2;
                destinationTextBox.IsEnabled = true;
                destinationTextBox.Text = ruleItem.destination;
            }
            else
            {
                if (ruleItem.source == "allIP")
                {
                    //選択肢を「すべてのIP」にする
                    destinationComboBox.SelectedIndex = 0;
                }
                else
                {
                    //選択肢を「このPCのアドレス」にする
                    destinationComboBox.SelectedIndex = 1;
                }
            }

            //プロトコルに値を代入
            switch (ruleItem.protocol) 
            {
                case "allProtocol":
                    protocolComboBox.SelectedIndex = 0;
                    switch (ruleItem.sourcePort)
                    {
                        case "allPort":
                            protocolComboBox.SelectedIndex = 0;
                            break;
                        case "80":
                            protocolComboBox.SelectedIndex= 1;
                            break;
                        case "443":
                            protocolComboBox.SelectedIndex = 2;
                            break;
                        case "162":
                            protocolComboBox.SelectedIndex = 3;
                            break;
                        case "53":
                            protocolComboBox.SelectedIndex= 4; 
                            break;
                        default:
                            protocolComboBox.SelectedIndex = 5;
                            protocolTextBox.Text = ruleItem.sourcePort;
                            break;
                    }

            break;
                case "TCP":
                    protocolComboBox.SelectedIndex = 1;
                    switch (ruleItem.sourcePort)
                    {
                        case "allPort":
                            portnumberComboBox.SelectedIndex = 0;
                            break;
                        case "80":

                            portnumberComboBox.SelectedIndex = 1;
                            break;
                        case "443":
                            portnumberComboBox.SelectedIndex= 2;
                            break;
                        default:
                            portnumberComboBox.SelectedIndex = 3;
                            protocolTextBox.Text= ruleItem.sourcePort;
                            break;
                    }
                    break;
                case "UDP":
                    protocolComboBox.SelectedIndex = 2;
                    switch (ruleItem.sourcePort)
                    {
                        case "allPort":
                            protocolComboBox.SelectedIndex = 0;
                            break;
                        case "162":
                            protocolComboBox.SelectedIndex = 1;
                            break;
                        case "53":
                            protocolComboBox.SelectedIndex = 2;
                            break;
                        default:
                            protocolComboBox.SelectedIndex = 3;
                            protocolTextBox.Text = ruleItem.sourcePort;
                            break;
                    }
                    break;
                default:
                    //「手動で設定」のときの処理
                    protocolComboBox.SelectedIndex = 3;
                    protocolTextBox.IsEnabled = true;
                    protocolTextBox.Text = ruleItem.protocol;
                    break;
            }


            //ポート番号に値を代入
          

            //サイズに値を代入
            sizeTextBox.Text = ruleItem.frameLength.ToString();

            //間隔に値を代入
            secondsTextBox.Text = ruleItem.detectionInterval.ToString();
            timesTextBox.Text = ruleItem.detectionCount.ToString();

        }
        
        private bool judgeIPAdress(string ip)
        {
            Regex regex = new Regex(ipAddressPattern);
            return regex.IsMatch(ip);
        }

        private void setSendData()
        {
            string sourceIP;
            string destinationIP;
            string protocolName;
            string portNum;

            if (sourceComboBox.SelectedIndex.ToString() == "2")
            {
                sourceIP=sourceTextBox.Text;
            }
            else
            {
                if(sourceComboBox.SelectedIndex.ToString() == "1")
                {
                    sourceIP = "myAddress";
                }
                else
                {
                    sourceIP = "allIP";
                }
            }

            if(destinationComboBox.SelectedIndex.ToString() == "2") 
            {
                destinationIP = destinationTextBox.Text;
            }
            else
            {
                if(destinationComboBox.SelectedIndex.ToString() == "1")
                {
                    destinationIP = "myAddress";
                }
                else
                {
                    destinationIP = "allIP";
                }
            }

            if(protocolComboBox.SelectedIndex.ToString() == "0") 
            {
                protocolName = "allProtocol";
            }
            else
            {
                protocolName = protocolTextBox.Text;
            }

            if(portnumberComboBox.SelectedIndex.ToString() == "0") 
            {
                portNum = "allPortnumber";
            }
            else
            {
                portNum = portnumberTextBox.Text;
            }

            //sendDataに格納
            sendData = new DataGridItem {
                ruleNo = ruleItem.ruleNo,
                source = sourceIP,
                destination = destinationIP,
                protocol = protocolName,
                sourcePort = portNum,
                frameLength =int.Parse(sizeTextBox.Text),
                detectionInterval = int.Parse(secondsTextBox.Text),
                detectionCount = int.Parse(timesTextBox.Text)
            };
        }

        private void sourceComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //「手動で設定」からそれ以外の選択肢に変えたとき、IPアドレスを入力できないようにする。
            if (sourceComboBox.SelectedIndex == 2)
            {
                sourceTextBox.IsEnabled = true;
            }
            else
            {
                sourceTextBox.IsEnabled = false;
            }
        }

        private void destinationComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //「手動で設定」からそれ以外の選択肢に変えたとき、IPアドレスを入力できないようにする。
            if (destinationComboBox.SelectedIndex == 2)
            {
                destinationTextBox.IsEnabled = true;
            }
            else
            {
                destinationTextBox.IsEnabled = false;
            }
        }

        private void protocolComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            portnumberComboBox.IsEnabled = true;

            portnumberComboBox.SelectedIndex = -1;

            //「プロトコル」で「TCP」を選択したときに、ポート番号の選択肢を変更する
            if (protocolComboBox.SelectedIndex.ToString() == "1")
            {
                protocolTextBox.Text = "TCP";

                portnumberComboBox.Items.Clear();

                for (int i = 0; i < tcpChoiced.Length; i++)
                {
                    portnumberComboBox.Items.Add(tcpChoiced[i]);
                }

            }
            else if (protocolComboBox.SelectedIndex.ToString() == "2")
            {
                //「プロトコル」で「UDP」を選択したときに、ポート番号の選択肢を変更する
                protocolTextBox.Text = "UDP";

                portnumberComboBox.Items.Clear();

                for (int i = 0; i < udpChoiced.Length; i++)
                {
                    portnumberComboBox.Items.Add(udpChoiced[i]);
                }
            }
            else
            {
                //他の選択肢を選択したときに、ポート番号の選択肢を変更する

                //手動で設定を選択したときは、テキストボックスに書き込めるようにする
                if (protocolComboBox.SelectedIndex.ToString().Equals("3"))
                {
                    protocolTextBox.IsEnabled = true;
                }
                else
                {
                    protocolTextBox.IsEnabled = false;
                }

                portnumberComboBox.Items.Clear();

                for (int i = 0; i < otherChoiced.Length; i++)
                {
                    portnumberComboBox.Items.Add(otherChoiced[i]);
                }
            }
        }

        private void portnumberComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedValue = portnumberComboBox.SelectedItem as string;
            if (selectedValue != null)
            {
                switch (selectedValue)
                {
                    case "HTTP(80)":
                        portnumberTextBox.Text = "80";
                        break;
                    case "HTTPS(443)":
                        portnumberTextBox.Text = "443";
                        break;
                    case "DNS(53)":
                        portnumberTextBox.Text = "53";
                        break;
                    case "SNMP(162)":
                        portnumberTextBox.Text = "162";
                        break;
                    case "手動で設定":
                        portnumberTextBox.Text = "";
                        portnumberTextBox.IsEnabled = true;
                        break;
                    default:
                        portnumberTextBox.Text = "";
                        portnumberTextBox.IsEnabled = false;
                        break;
                }
            }
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
           
        }
    }
}
