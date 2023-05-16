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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BrownieHound
{
    /// <summary>
    /// ruleg_settings.xaml の相互作用ロジック
    /// </summary>
    public partial class ruleg_settings : Page
    {
        public ruleg_settings()
        {
            InitializeComponent();
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            rule_add_Window ruleAddWin = new rule_add_Window();
            ruleAddWin.ShowDialog();
            //var nextPage = new rule_edit();
            //NavigationService.Navigate(nextPage);
        }

        private void detailButton_Click(object sender, RoutedEventArgs e)
        {
            rule_edit_Window ruleEditWin = new rule_edit_Window();
            ruleEditWin.ShowDialog();
            //var nextPage = new ruleg_detail();
            //NavigationService.Navigate(nextPage);
        }

        private void inactivate_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void redoButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
