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
    /// mail_settings.xaml の相互作用ロジック
    /// </summary>
    public partial class mail_settings : Page
    {
        public mail_settings()
        {
            InitializeComponent();
        }

        private void s_rTotop_redo_Click(object sender, RoutedEventArgs e)
        {
            var nextPage = new top();
            NavigationService.Navigate(nextPage);
        }

        private void s_rTotop_submit_Click(object sender, RoutedEventArgs e)
        {
            var nextPage = new top();
            NavigationService.Navigate(nextPage);
        }
    }
}
