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

namespace BrownieHound
{
    /// <summary>
    /// top.xaml の相互作用ロジック
    /// </summary>
    public partial class top : Page
    {
        public top()
        {
            InitializeComponent();
        }
        Process processTsinterface = null;
        private void Page_loaded(object sender, RoutedEventArgs e)
        {
            string Command = "C:\\Program Files\\Wireshark\\tshark.exe";

            string args = "-D";
            processTsinterface = new Process();
            ProcessStartInfo processSinfo = new ProcessStartInfo(Command, args);
            processSinfo.CreateNoWindow = true;
            processSinfo.UseShellExecute = false;
            processSinfo.RedirectStandardOutput = true;
            processSinfo.RedirectStandardError = true;

            processSinfo.StandardErrorEncoding = Encoding.UTF8;
            processSinfo.StandardOutputEncoding = Encoding.UTF8;

            processTsinterface = Process.Start(processSinfo);

            processTsinterface.OutputDataReceived += dataReceived;
            processTsinterface.ErrorDataReceived += errReceived;

            processTsinterface.BeginErrorReadLine();
            processTsinterface.BeginOutputReadLine();
            
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
            ListBoxItem item = new ListBoxItem();
            item.Content = msg;
            item.MouseDoubleClick += listBoxItem_MouseDoubleClick;
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
            if (listBoxItem != null)
            {
                sendTos_r(listBoxItem.Content.ToString());
            }
        }
    }
}
