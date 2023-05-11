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
    /// detectWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class detectWindow : Window
    {
        public detectWindow()
        {
            InitializeComponent();

            Uri uri = new Uri("/rule_add.xaml", UriKind.Relative);
            frame.Source = uri;
        }
    }
}
