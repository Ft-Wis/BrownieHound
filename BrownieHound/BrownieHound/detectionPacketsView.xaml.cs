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
    /// detectionPacketsView.xaml の相互作用ロジック
    /// </summary>
    public partial class detectionPacketsView : Page
    {
        public detectionPacketsView(string pathToFolder)
        {
            InitializeComponent();
            System.Windows.MessageBox.Show($"{pathToFolder}を選択しました");
        }
    }
}
