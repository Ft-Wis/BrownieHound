using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Uri uri = new Uri("/top.xaml", UriKind.Relative);
            frame.Source = uri;
        }

        private void BrownieHound_Closed(object sender, EventArgs e)
        {
            if (Directory.Exists("temps"))
            {
                Directory.Delete("temps",true);
            }
            if (Directory.Exists("tempdetectionData"))
            {
                Directory.Delete("tempdetectionData",true);
            }
        }
    }
}
