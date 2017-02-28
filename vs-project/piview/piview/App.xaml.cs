using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace piview
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            if (e.Args.Length < 1)
            {
                MessageBox.Show("Miss arguments.", "Error!");
                this.Shutdown();
                return;
            }

            MainWindow mainWnd = new MainWindow(e.Args[0]); // test

            if (mainWnd.IsShowable())
            {
                mainWnd.Show();
            }
            else
            {
                this.Shutdown();
            }
        }
    }
}
