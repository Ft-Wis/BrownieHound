using System;
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
using System.Diagnostics;

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
            //ブラックリスト・ホワイトリストに値を代入
            if (ruleItem.ruleCategory == "否検出")
            {
                blackListRadioButton.IsChecked = true;
            }
            else
            {
                whiteListRadioButton.IsChecked = true;
            }

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
                if(ruleItem.source.Equals("all"))
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
                if (ruleItem.Equals("all"))
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

            string setPort;

            if (ruleItem.sourcePort.Equals("all"))
            {
                destinationRadioButton.IsChecked = true;
                setPort = ruleItem.destinationPort;
            }
            else
            {
                sourceRadioButton.IsChecked = true;
                setPort = ruleItem.sourcePort;
            }
            //プロトコルに値を代入
            switch (ruleItem.protocol)
            {
                case "all":
                    protocolComboBox.SelectedIndex = 0;
                    switch (setPort)
                    {
                        case "all":
                            break;
                        case "80":
                            portnumberComboBox.SelectedIndex = 1;
                            break;
                        case "443":
                            portnumberComboBox.SelectedIndex = 2;
                            break;
                        case "162":
                            portnumberComboBox.SelectedIndex = 3;
                            break;
                        case "53":
                            portnumberComboBox.SelectedIndex = 4;
                            break;
                        default:
                            portnumberComboBox.SelectedIndex = 5;
                            portnumberTextBox.Text = setPort;
                            break;
                    }
                    break;
                case "TCP":
                    protocolComboBox.SelectedIndex = 1;
                    switch (setPort)
                    {
                        case "all":
                            portnumberComboBox.SelectedIndex = 0;
                            break;
                        case "80":
                            portnumberComboBox.SelectedIndex = 1;
                            break;
                        case "443":
                            portnumberComboBox.SelectedIndex = 2;
                            break;
                        default:
                            portnumberComboBox.SelectedIndex = 3;
                            portnumberTextBox.Text = setPort;
                            break;
                    }
                    break;
                case "UDP":
                    protocolComboBox.SelectedIndex = 2;
                    switch (setPort)
                    {
                        case "all":
                            break;
                        case "162":
                            portnumberComboBox.SelectedIndex = 1;
                            break;
                        case "53":
                            portnumberComboBox.SelectedIndex = 2;
                            break;
                        default:
                            portnumberComboBox.SelectedIndex = 3;
                            portnumberTextBox.Text = setPort;
                            break;
                    }
                    break;
                default:
                    // 「手動で設定」のときの処理
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
            string sourcePortNum;
            string destinationPortNum;
            string category;

            sourceIP = sourceTextBox.Text;
            destinationIP = destinationTextBox.Text;
            protocolName = protocolTextBox.Text;

            //ホワイトリストにチェックがされている場合
            if ((bool)whiteListRadioButton.IsChecked)
            {
                category = "1";
            }
            else
            {
                category = "0";
            }

            //送信元ポートにチェックがされている場合
            if ((bool)sourceRadioButton.IsChecked)
            {
                sourcePortNum = portnumberTextBox.Text;
                destinationPortNum = "all";
            }
            else
            {
                sourcePortNum = "all";
                destinationPortNum = portnumberTextBox.Text;
            }

            //sendDataに格納
            sendData = new DataGridItem {
                ruleCategory = category,
                ruleNo = ruleItem.ruleNo,
                source = sourceIP,
                destination = destinationIP,
                protocol = protocolName,
                sourcePort = sourcePortNum,
                destinationPort = destinationPortNum,
                frameLength =int.Parse(sizeTextBox.Text),
                detectionInterval = int.Parse(secondsTextBox.Text),
                detectionCount = int.Parse(timesTextBox.Text)
            };
        }

        private void sourceComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //「手動で設定」からそれ以外の選択肢に変えたとき、IPアドレスを入力できないようにする。
            var selectedIdx = sourceComboBox.SelectedIndex;
            switch (selectedIdx)
            {
                //「すべて」
                case 0:
                    sourceTextBox.IsEnabled = false;
                    sourceTextBox.Text = "all";
                    break;
                //「このPCのアドレス」
                case 1:
                    sourceTextBox.IsEnabled = false;
                    sourceTextBox.Text = "myAddress";
                    break;
                //「手動で設定」
                case 2:
                    sourceTextBox.IsEnabled = true;
                    sourceTextBox.Focus();
                    sourceTextBox.SelectAll();
                    break;
                default:
                    break;
            }
        }

        private void destinationComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //「手動で設定」からそれ以外の選択肢に変えたとき、IPアドレスを入力できないようにする。
            var selectedIdx = destinationComboBox.SelectedIndex;
            switch (selectedIdx)
            {
                //「すべて」
                case 0:
                    destinationTextBox.IsEnabled = false;
                    destinationTextBox.Text = "all";
                    break;
                //「このPCのアドレス」
                case 1:
                    destinationTextBox.IsEnabled = false;
                    destinationTextBox.Text = "myAddress";
                    break;
                //「手動で設定」
                case 2:
                    destinationTextBox.IsEnabled = true;
                    destinationTextBox.Focus();
                    destinationTextBox.SelectAll();
                    break;
                default:
                    break;
            }
        }

        private void protocolComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            portnumberComboBox.IsEnabled = true;
            

            portnumberComboBox.SelectedIndex = -1;

            //「プロトコル」で「TCP」を選択したときに、ポート番号の選択肢を変更する
            if (protocolComboBox.SelectedIndex.ToString().Equals("1"))
            {
                protocolTextBox.Text = "TCP";

                portnumberComboBox.Items.Clear();

                for (int i = 0; i < tcpChoiced.Length; i++)
                {
                    portnumberComboBox.Items.Add(tcpChoiced[i]);
                }

            }
            else if (protocolComboBox.SelectedIndex.ToString().Equals("2"))
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
                    protocolTextBox.Focus();
                    protocolTextBox.SelectAll();
                }
                else
                {
                    //「すべてのプロトコル」のとき
                    protocolTextBox.Text = "all";
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
                portnumberTextBox.IsEnabled = false;
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
                        portnumberTextBox.IsEnabled = true;
                        portnumberTextBox.Focus();
                        portnumberTextBox.SelectAll();
                        break;
                    default:
                        portnumberTextBox.Text = "all";
                        portnumberTextBox.IsEnabled = false;
                        break;
                }
            }
        }
    }
}
