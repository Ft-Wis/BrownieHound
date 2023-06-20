﻿using System;
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

namespace BrownieHound
{
    /// <summary>
    /// rule_add_Window.xaml の相互作用ロジック
    /// </summary>
    /// 


    //数値判定用のクラス(未完成)
    //public class NumericValidationRule : ValidationRule
    //{
    //    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    //    {
    //        if (value == null || string.IsNullOrEmpty(value.ToString()))
    //        {
    //            return new ValidationResult(false, "数値を入力してください。");
    //        }

    //        if (!double.TryParse(value.ToString(), out _))
    //        {
    //            return new ValidationResult(false, "数値を入力してください。");
    //        }

    //        return ValidationResult.ValidResult;
    //    }
    //}

    public partial class rule_add_Window : Window 
    {
        private string[] tcpChoiced = { "すべて", "HTTP(80)", "HTTPS(443)","手動で設定" };
        private string[] udpChoiced = { "すべて", "SNMP(162)", "DNS(53)", "手動で設定" };
        private string[] otherChoiced = { "すべて", "HTTP(80)", "HTTPS(443)", "SNMP(162)", "DNS(53)", "手動で設定" };

        private int _number;
        //public string FormattedNumber => Number.ToString("N0");

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
            portnumberComboBox.IsEnabled = true;

            portnumberComboBox.SelectedIndex = -1;

            //「プロトコル」で「TCP」を選択したときに、ポート番号の選択肢を変更する
            if(protocolComboBox.SelectedIndex.ToString() == "1")
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
                if(protocolComboBox.SelectedIndex.ToString().Equals("3")) 
                {
                    protocolTextBox.IsEnabled = true;
                }
                else
                {
                    protocolTextBox.IsEnabled = false;
                }

                portnumberComboBox.Items.Clear();

                for( int i = 0 ; i<otherChoiced.Length ; i++)
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



        //数値以外が入力された場合にエラーを出す(未完成)

        //public int Number
        //{
        //    get { return _number; }
        //    set
        //    {
        //        _number = value;
        //        OnPropertyChanged(nameof(Number));
        //    }

        // }

        //public string this[string columnName]
        //{ 
        //    get
        //    {
        //        string error = null;

        //        switch(columnName)
        //        {
        //            case nameof(Number):
        //                if (Number <= 0)
        //                    error = "数値を入力してください";
        //                break;
        //        }

        //        return error;
        //    }
        //}

        //public string Error => null;

        //public event PropertyChangedEventHandler PropertyChanged;

        //protected virtual void OnPropertyChanged(string propertyName)
        //{
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //}
    }
}
