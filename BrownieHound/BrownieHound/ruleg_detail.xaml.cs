using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using static BrownieHound.App;

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
        public ruleg_detail(int no ,String name, List<ruleData> ruledata)
        {
            InitializeComponent();
            title.Content = $"{title.Content} - {name}";
            foreach (ruleData rd in ruledata)
            {
                Debug.WriteLine($"{rd}");
            }
            
        }

        private void editButton_Click(object sender, RoutedEventArgs e)
        {
            rule_edit_Window rule_Edit_Window = new rule_edit_Window();
            if (rule_Edit_Window.ShowDialog() == true)
            {

            }
            else
            {

            }
            
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            rule_add_Window rule_Add_Window = new rule_add_Window();
            if (rule_Add_Window.ShowDialog() == true)
            {

            }
            else
            {

            }
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
