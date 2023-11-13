using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BrownieHound
{
    /// <summary>
    /// top.xaml の相互作用ロジック
    /// </summary>
    public partial class top : Page
    {
        //public top()
        //{
            //InitializeComponent();

       // }
        Process processTsinterface = null;
        string path = @"conf";
        private void tsharkconnect()
        {
            string tsDirectory = "";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);

            }
            if (!File.Exists(@$"{path}\path.conf"))
            {
                using (StreamWriter sw = new StreamWriter(@$"{path}\path.conf", false, Encoding.GetEncoding("UTF-8")))
                {
                    sw.WriteLine(@"C:\Program Files\Wireshark");
                }
            }
            using (StreamReader sr = new StreamReader(@$"{path}\path.conf", Encoding.GetEncoding("UTF-8")))
            {
                tsDirectory=sr.ReadLine();
            }
            string args = "-D";
            processTsinterface = new Process();
            ProcessStartInfo processSinfo = new ProcessStartInfo(@$"{tsDirectory}\tshark.exe", args);
            processSinfo.CreateNoWindow = true;
            processSinfo.UseShellExecute = false;
            processSinfo.RedirectStandardOutput = true;
            processSinfo.RedirectStandardError = true;

            processSinfo.StandardErrorEncoding = Encoding.UTF8;
            processSinfo.StandardOutputEncoding = Encoding.UTF8;
            try
            {
                processTsinterface = Process.Start(processSinfo);
                processTsinterface.OutputDataReceived += dataReceived;
                processTsinterface.ErrorDataReceived += errReceived;
                processTsinterface.BeginErrorReadLine();
                processTsinterface.BeginOutputReadLine();
            }
            catch
            {
                Tsharkpath tsharkPathInput = new Tsharkpath();
                if(tsharkPathInput.ShowDialog()==true)
                {
                    using (StreamWriter sw = new StreamWriter(@$"{path}\path.conf", false, Encoding.GetEncoding("UTF-8")))
                    {
                        sw.WriteLine(tsharkPathInput.tsharkPath);
                    }
                    tsharkconnect();
                }
                else
                {
                    interfaceList.Items.Add("パスが通っていません");
                    processTsinterface = null;
                    topTos_r.IsEnabled = false;
                }
            }
        }
        private void Page_loaded(object sender, RoutedEventArgs e)
        {
            interfaceList.Items.Clear();
            tsharkconnect();
        }

        private void errReceived(object sender, DataReceivedEventArgs e)
        {
            string packetText = e.Data;
            if (packetText != null && packetText.Length > 0)
            {
                PrintTextBoxByThread("ERR:" + packetText);
            }
        }

        private void dataReceived(object sender, DataReceivedEventArgs e)
        {
            string packetText = e.Data;
            if (packetText != null && packetText.Length > 0)
            {
                PrintTextBoxByThread(packetText);
            }
        }
        private void PrintText(string msg)
        {
            interfaceList.Items.Add(msg);
        }
        private void PrintTextBoxByThread(string msg)
        {
            Dispatcher.Invoke(new Action(() => PrintText(msg)));
        }
        private void topTos_r_Click(object sender, RoutedEventArgs e)
        {
            if (interfaceList.SelectedItems.Count > 0)
            {
                sendTos_r(interfaceList.SelectedItems[0].ToString());
            }
        }
        private void sendTos_r(string tsInterface)
        {
            var nextPage = new standby_rule(tsInterface);
            NavigationService.Navigate(nextPage);
        }

        private void topTom_s_Click(object sender, RoutedEventArgs e)
        {
            var nextPage = new mail_settings();
            NavigationService.Navigate(nextPage);
        }

        private void topTorg_s_Click(object sender, RoutedEventArgs e)
        {
            var nextPage = new ruleg_settings();
            NavigationService.Navigate(nextPage);
        }

        private void Grid_Unloaded(object sender, RoutedEventArgs e)
        {
            if (processTsinterface != null && !processTsinterface.HasExited)
            {
                processTsinterface.Kill();
            }
        }

        private void listBoxItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var listBoxItem = sender as ListBoxItem;
            if (listBoxItem != null && processTsinterface != null)
            {
                sendTos_r(listBoxItem.Content.ToString());
            }
        }
    }
}
