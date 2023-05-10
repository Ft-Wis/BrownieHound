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
    /// subWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class subWindow : Window
    {
        public subWindow(string mode)
        {
            InitializeComponent();

            Uri uri = new Uri("/rule_"+mode+".xaml", UriKind.Relative);

            MessageBox.Show(uri.ToString());

            frame.Source = uri;

            this.SizeToContent = SizeToContent.Manual;

            // コンテンツに合わせて自動的にWindow幅をリサイズする
            this.SizeToContent = SizeToContent.Width;

            // コンテンツに合わせて自動的にWindow高さをリサイズする
            this.SizeToContent = SizeToContent.Height;

            // コンテンツに合わせて自動的にWindow幅と高さをリサイズする
            this.SizeToContent = SizeToContent.WidthAndHeight;

        }
    }
}
