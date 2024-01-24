using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static BrownieHound.RuleGroupNameValidation;

namespace BrownieHound
{
    /// <summary>
    /// ruleg_add_Window.xaml の相互作用ロジック
    /// </summary>
    public partial class ruleg_add_Window : Window
    {
        RuleName_Validation ruleGroupNameValidation = new RuleName_Validation();
        public ruleg_add_Window()
        {
            InitializeComponent();

            this.Owner = App.Current.MainWindow;
            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            DataContext = ruleGroupNameValidation;
        }
        public string newGroupName { get; set; } = "";
        private void ruleg_add_submit_Click(object sender, RoutedEventArgs e)
        {
            newGroupName = ruleGroupNameValidation.ruleGroupName.Value;
            if (!ruleGroupNameValidation.ruleGroupName.HasErrors)
            {
                if (newGroupName.Length != 0)
                {
                    for (int i = 1; File.Exists($"ruleGroup\\{newGroupName}.txt"); i++)
                    {
                        if (!File.Exists($"ruleGroup\\{newGroupName}-{i}.txt"))
                        {
                            newGroupName = $"{newGroupName}-{i}";
                            break;
                        }
                    }
                    if (newGroupName.Length > 34)
                    {
                        int overLength = newGroupName.Length - 34;
                        MessageBox.Show($"ルールグループ名が重複している為、連番を振ると文字列の制限を超えます。\nもう一度ご確認ください。\n超過する文字数：{overLength}\nファイル名：{newGroupName}", "入力エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        this.DialogResult = true;
                        this.Close();
                    }

                }
                else
                {
                    MessageBox.Show("ルールグループの名前を\n入力してください。", "!警告!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("ルールグループの名前の形式が正しくありません。\nもう一度ご確認ください。","入力エラー",MessageBoxButton.OK,MessageBoxImage.Error);
            }

        }

        private void ruleg_add_redo_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
