using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Principal;
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

namespace AP.CCTV.RMA.Installer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        static string ProgramFilesx86()
        {
            if (8 == IntPtr.Size
                || (!String.IsNullOrEmpty(Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432"))))
            {
                return Environment.GetEnvironmentVariable("ProgramFiles(x86)");
            }

            return Environment.GetEnvironmentVariable("ProgramFiles");
        }

        private bool IsRunAsAdministrator()
        {
            var wi = WindowsIdentity.GetCurrent();
            var wp = new WindowsPrincipal(wi);

            return wp.IsInRole(WindowsBuiltInRole.Administrator);
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (!IsRunAsAdministrator())
            {
                var processInfo = new ProcessStartInfo(Assembly.GetExecutingAssembly().CodeBase);
                processInfo.Arguments = "\"" + textBox.Text + "\"";
                if ((bool)itv.IsChecked)
                {
                    processInfo.Arguments += " /full";
                }
                // The following properties run the new process as administrator
                processInfo.UseShellExecute = true;
                processInfo.Verb = "runas";

                // Start the new process
                try
                {
                    Process.Start(processInfo);
                }
                catch (Exception)
                {
                    // The user did not allow the application to run as administrator
                    MessageBox.Show("Sorry, this application must be run as Administrator.");
                }

                // Shut down the current process
                Application.Current.Shutdown();
            }
            else
            {
                ProgressWindow install = new ProgressWindow(new string[] { textBox.Text, "/full" });
                install.Show();
                /*f (!AssemblyHelpers.Install(textBox.Text, (bool)itv.IsChecked))
                    MessageBox.Show("Installation compleate with errors!", "Warning!");*/
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            FileVersionInfo info = FileVersionInfo.GetVersionInfo(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
            textBox.Text = System.IO.Path.Combine(ProgramFilesx86(), info.CompanyName, info.ProductName);

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog path = new System.Windows.Forms.FolderBrowserDialog();
            if (path.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                textBox.Text = path.SelectedPath;
            //path.
        }
    }
}
