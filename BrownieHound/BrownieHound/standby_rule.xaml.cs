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
    /// standby_rule.xaml の相互作用ロジック
    /// </summary>
    public partial class standby_rule : Page
    {
        public standby_rule(string tsInterface)
        {
            InitializeComponent();
            this.interfaceLabel.Content = tsInterface;
        }

        private void s_rTotop_Click(object sender, RoutedEventArgs e)
        {
            var nextPage = new top();
            NavigationService.Navigate(nextPage);
        }

        private void activate_Click(object sender, RoutedEventArgs e)
        {
            string interfaceText = interfaceLabel.Content.ToString();
            string interfaceNumber = interfaceText.Substring(0,interfaceText.IndexOf("."));
            var window = new detectWindow();
            window.Show();
            var nextPage = new capture(interfaceNumber);
            NavigationService.Navigate(nextPage);

        }
    }
}
