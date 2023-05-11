using MaterialDesignThemes.Wpf;
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
    /// ruleg_detail.xaml の相互作用ロジック
    /// </summary>
    public partial class ruleg_detail : Page
    {
        public ruleg_detail()
        {
            InitializeComponent();
        }

        private void editButton_Click(object sender, RoutedEventArgs e)
        {
            subWindow subWindow = new subWindow("edit");
            subWindow.ShowDialog();
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            subWindow subWindow = new subWindow("add");
            subWindow.ShowDialog();
        }

        private void redoButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void inactivate_Click(object sender, RoutedEventArgs e)
        {

        }

    }
}
