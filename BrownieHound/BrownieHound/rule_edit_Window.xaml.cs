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

namespace BrownieHound
{
    /// <summary>
    /// rule_edit_Window.xaml の相互作用ロジック
    /// </summary>
    public partial class rule_edit_Window : Window
    {
        private DataGridItem ruleItem;

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
            DialogResult = true;
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

        private void sourceComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //「手動で設定」からそれ以外の選択肢に変えたとき、IPアドレスを入力できないようにする。
            if (sourceComboBox.Text == "手動で設定")
            {
                sourceTextBox.IsEnabled = false;
            }

            //「手動で設定」を選択したときにIPアドレスを入力できるようにする。
            if (sourceComboBox.SelectedIndex.ToString() == "2")
            {
                MessageBox.Show(sourceComboBox.Text);
                sourceTextBox.IsEnabled = true;
            }
        }

        private void distinationComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //「手動で設定」からそれ以外の選択肢に変えたとき、IPアドレスを入力できないようにする。
            if (distinationComboBox.Text == "手動で設定")
            {
                distinationTextBox.IsEnabled = true;
            }

            //「手動で設定」を選択したときにIPアドレスを入力できるようにする。
            if (distinationComboBox.SelectedIndex.ToString() == "2")
            {
                distinationTextBox.IsEnabled = false;
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

            MessageBox.Show(selectedValue);

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
