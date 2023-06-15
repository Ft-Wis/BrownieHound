using System;
using System.Collections;
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
    /// rule_edit_Window.xaml の相互作用ロジック
    /// </summary>
    public partial class rule_edit_Window : Window
    {
        public rule_edit_Window()
        {
            //this.SizeToContent = SizeToContent.WidthAndHeight;
            //this.SizeToContent = SizeToContent.Width;
            //this.SizeToContent = SizeToContent.Manual;
            //this.SizeToContent = SizeToContent.Height;
            InitializeComponent();
            Loaded += Window_Loaded;
            
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void editButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            double screenWidth = SystemParameters.PrimaryScreenWidth;
            double screenHeight = SystemParameters.PrimaryScreenHeight;

            double windowWidth = Width;
            double windowHeight = Height;

            if (windowWidth > screenWidth)
            {
                Width = screenWidth;
                Left = 0;
            }
            else
            {
                Left = (screenWidth - windowWidth) / 2;
            }

            if (windowHeight > screenHeight)
            {
                Height = screenHeight;
                Top = 0;
            }
            else
            {
                Top = (screenHeight - windowHeight) / 2;
            }
        }
    }
}
