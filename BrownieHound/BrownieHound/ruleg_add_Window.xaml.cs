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
    /// ruleg_add_Window.xaml の相互作用ロジック
    /// </summary>
    public partial class ruleg_add_Window : Window
    {
        public ruleg_add_Window()
        {
            InitializeComponent();

            this.Owner = App.Current.MainWindow;
            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        }
        public string newGroupName { get; set; } = "";
        private void ruleg_add_submit_Click(object sender, RoutedEventArgs e)
        {
            newGroupName = ruleGroupNameText.Text;
            this.DialogResult = true;
            this.Close();
        }

        private void ruleg_add_redo_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
