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
    /// top.xaml の相互作用ロジック
    /// </summary>
    public partial class top : Page
    {
        public top()
        {
            InitializeComponent();
        }
        private void topTos_r_Click(object sender, RoutedEventArgs e)
        {
            var nextPage = new standby_rule();
            NavigationService.Navigate(nextPage);
        }

        private void topTom_s_Click(object sender, RoutedEventArgs e)
        {
            var nextPage = new mail_settings();
            NavigationService.Navigate(nextPage);
        }

        private void topTorg_s_Click(object sender, RoutedEventArgs e)
        {
            var nextPage = new ruleg_settings();
            NavigationService.Navigate(nextPage);
        }
    }
}
