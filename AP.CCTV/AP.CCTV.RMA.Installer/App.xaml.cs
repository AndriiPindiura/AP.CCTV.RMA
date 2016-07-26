using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;

namespace AP.CCTV.RMA.Installer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (e.Args.Length > 0)
            {
                ProgressWindow install = new ProgressWindow(e.Args);
                install.Show();
            }
            /*if (e.Args.Length == 2 && e.Args[1].Contains("/full"))
            {
                if (!AssemblyHelpers.Install(e.Args[0], true))
                    MessageBox.Show("Installation compleate with errors!", "Warning!");
            }
            else if (e.Args.Length == 1)
            {
                if (!AssemblyHelpers.Install(e.Args[0], false))
                    MessageBox.Show("Installation compleate with errors!", "Warning!");
            }*/
            else
            {
                MainWindow app = new MainWindow();
                app.Show();
            }

        }

    }
}
