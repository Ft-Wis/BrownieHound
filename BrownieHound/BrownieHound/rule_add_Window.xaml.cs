using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
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

namespace BrownieHound
{
    /// <summary>
    /// rule_add_Window.xaml の相互作用ロジック
    /// </summary>
    /// 

    public partial class rule_add_Window : Window 
    {
        private string[] tcpChoiced = { "すべて", "HTTP(80)", "HTTPS(443)","手動で設定" };
        private string[] udpChoiced = { "すべて", "SNMP(162)", "DNS(53)", "手動で設定" };
        private string[] otherChoiced = { "すべて", "HTTP(80)", "HTTPS(443)", "SNMP(162)", "DNS(53)", "手動で設定" };

        public DataGridItem sendData;
        private int newRuleNo;

        public rule_add_Window(int ruleNo)
        {
            InitializeComponent();
            newRuleNo = ruleNo;
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            setSendData();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {

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
                destinationPortNum ="all";
            }
            else
            {
                sourcePortNum = "all";
                destinationPortNum = portnumberTextBox.Text;
            }

            //sendDataに格納
            sendData = new DataGridItem
            {
                ruleCategory = category,
                ruleNo = newRuleNo,
                source = sourceIP,
                destination = destinationIP,
                protocol = protocolName,
                sourcePort = sourcePortNum,
                destinationPort = destinationPortNum,
                frameLength = int.Parse(sizeTextBox.Text),
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
