using System;
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

namespace BrownieHound
{
    /// <summary>
    /// rule_add_Window.xaml の相互作用ロジック
    /// </summary>
    public partial class rule_add_Window : Window
    {
        private string[] tcpChoiced = { "HTTP", "HTTPS" };
        private string[] udpChoiced = { "SNMP", "DNS" };
        public rule_add_Window()
        {
            InitializeComponent();

        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void sourceComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //「手動で設定」からそれ以外の選択肢に変えたとき、IPアドレスを入力できないようにする。
            if (sourceComboBox.Text == "手動で設定")
            {
                sourceTextBox.IsEnabled = false;
            }

            //「手動で設定」を選択したときにIPアドレスを入力できるようにする。
            if (sourceComboBox.SelectedIndex.ToString()=="2")
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
            //「プロトコル」で「TCP」を選択したときに、ポート番号の選択肢を変更する
            if(protocolComboBox.SelectedIndex.ToString() == "1")
            {

            }

            //「プロトコル」で「UDP」を選択したときに、ポート番号の選択肢を変更する
            if (protocolComboBox.SelectedIndex.ToString() == "2")
            {

            }
        }

        private void portnumberComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
