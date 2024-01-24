using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace BrownieHound
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        System.Threading.Mutex mutex = new System.Threading.Mutex(false,"BrownieHound");
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (!mutex.WaitOne(0, false))
            {
                mutex.Close();
                this.Shutdown(); 
            }
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            if (mutex != null)
            {
                mutex.ReleaseMutex();
                mutex.Close();
            }
        }
    }
}
