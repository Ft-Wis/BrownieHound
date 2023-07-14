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
    /// Tsharkpath.xaml の相互作用ロジック
    /// </summary>
    public partial class Tsharkpath : Window
    {
        public Tsharkpath()
        {
            InitializeComponent();
            this.Owner = App.Current.MainWindow;
        }
        public string tsharkPath { get; set; } = "";
        private void Tshark_path_submit_Click(object sender, RoutedEventArgs e)
        {
            tsharkPath = TsharkpathText.Text;
            this.DialogResult = true;
            this.Close();
        }

        private void Tshark_path_redo_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
