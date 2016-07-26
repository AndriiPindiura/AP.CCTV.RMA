using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AP.CCTV.RMA.Installer
{
    /// <summary>
    /// Interaction logic for Progress.xaml
    /// </summary>
    public partial class ProgressWindow : Window
    {
        //private WebClient web;

        private Queue<string> downloadUrls = new Queue<string>();

        private List<string> downloadedFiles = new List<string>();

        private void DownloadFile()
        {
            if (downloadUrls.Any())
            {
                WebClient client = new WebClient();
                client.DownloadProgressChanged += client_DownloadProgressChanged;
                client.DownloadFileCompleted += client_DownloadFileCompleted;

                var url = downloadUrls.Dequeue();
                string fileName = url.Substring(url.LastIndexOf("/") + 1,
                            (url.Length - url.LastIndexOf("/") - 1));
                downloadedFiles.Add(fileName);
                status.AppendText("завантаження " + fileName + "... ");
                status.ScrollToEnd();
                client.DownloadFileAsync(new Uri(url), fileName);
                //lblFileName.Text = url;
                return;

                // End of the download
                //btnGetDownload.Text = "Download Complete";
            }
        }

        private void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                // handle error scenario
                status.AppendText(e.Error.Message + "\r\n");
                //throw e.Error;
            }
            else
            {
                status.AppendText("завантажено.\r\n");
            }
            if (e.Cancelled)
            {
                // handle cancelled scenario
            }
            if (downloadUrls.Any())
            {
                DownloadFile();
            }
            else
            {
                Install(cmdArgs[0], true);
            }

        }

        private void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            double bytesIn = double.Parse(e.BytesReceived.ToString());
            double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
            double percentage = bytesIn / totalBytes * 100;
            progress.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                new Action(delegate ()
                {
                    progress.Value = int.Parse(Math.Truncate(percentage).ToString());
                }));
        }

        private void Install(string installPath, bool itv)
        {
            if (itv)
            {
                Process sdk = new Process();
                foreach (string fileName in downloadedFiles)
                {
                    string fileExt = fileName.Substring(fileName.LastIndexOf(".") + 1, (fileName.Length - fileName.LastIndexOf(".") - 1));
                    try
                    {
                        if (fileExt == "exe")
                        {
                            status.AppendText("інсталяція " + fileName + "...\r\n");
                            sdk.StartInfo.FileName = fileName;
                            sdk.StartInfo.Arguments = "/Q";
                            sdk.Start();
                            sdk.WaitForExit();
                        }
                        if (fileExt == "msi")
                        {
                            status.AppendText("інсталяція " + fileName + "...\r\n");
                            sdk.StartInfo.FileName = fileName;
                            sdk.StartInfo.Arguments = "/passive /norestart";
                            sdk.Start();
                            sdk.WaitForExit();
                        }
                        if (fileExt == "dll" || fileExt == "ocx")
                        {
                            status.AppendText("копіювання " + fileName + "...\r\n");
                            File.Copy(fileName, System.IO.Path.Combine(installPath, fileName), true);
                        }
                    }
                    catch (Exception ex)
                    {
                        status.AppendText("помилка при інсталяції компонент розробника:\r\n" + ex.Message + "\r\n");
                    }
                }
            }
            StringBuilder sb = new StringBuilder();
            string path = new Uri(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase)).LocalPath;
            bool result = false;
            if (!Directory.Exists(installPath))
            {
                status.AppendText("створення теки " + installPath + "\r\n");
                Directory.CreateDirectory(installPath);
            }
            else
            {
                if (MessageBox.Show("Вказана тека вєе існує\r\nЗамінити?", "Попередження!", MessageBoxButton.YesNo) == MessageBoxResult.No)
                {
                    Close();
                }
            }
            status.AppendText("копіювання системних файлів до теки...\r\n");

            foreach (string dll in Directory.GetFiles(path, "*.dll"))
            {
                try
                {
                    status.AppendText("копіювання " + System.IO.Path.GetFileName(dll) + "...\r\n");
                    File.Copy(dll, dll.Replace(System.IO.Path.GetDirectoryName(dll), installPath), true);
                }
                catch (Exception ex)
                {
                    status.AppendText("помилка при копіюванні системних файлів:" + ex.Message + "\r\nподальша робота неможлива. зверніться до адміністратора\r\n");
                    this.Close();
                }
            }
            status.AppendText("реєстрація типів...\r\n");
            foreach (string dll in Directory.GetFiles(installPath))
            {
                try
                {
                    status.AppendText("реєстрація " + System.IO.Path.GetFileName(dll) + "...\r\n");
                    RegistrationServices regAsm = new RegistrationServices();
                    result = result & regAsm.RegisterAssembly(Assembly.LoadFile(dll), AssemblyRegistrationFlags.SetCodeBase);
                }
                catch 
                {
                    try
                    {
                        Process reg = new Process();
                        reg.StartInfo.FileName = "regsvr32.exe";
                        reg.StartInfo.Arguments = "/s \"" + dll + "\"";
                        reg.StartInfo.UseShellExecute = false;
                        reg.StartInfo.CreateNoWindow = true;
                        reg.StartInfo.RedirectStandardOutput = true;
                        reg.Start();
                        reg.WaitForExit();
                        if (reg.ExitCode != 0)
                        {
                            status.AppendText("помилка при реєстраціі " + System.IO.Path.GetFileName(dll) + ":\r\n");
                        }
                        reg.Close();
                    }
                    catch (Exception ex)
                    {
                        status.AppendText("помилка при реєстраціі " + System.IO.Path.GetFileName(dll) + ":\r\n" + ex.Message + "\r\n");
                    }
                }
            }
            status.AppendText("завершення інсталяції\r\n");
            Thread.Sleep(1000);
            this.Close();
        }

        private string[] cmdArgs;

        public ProgressWindow(string[] args)
        {
            InitializeComponent();
            cmdArgs = args;
            //web = new WebClient();
            //web.DownloadProgressChanged += Web_DownloadProgressChanged;
        }

        private void Web_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progress.Value = e.ProgressPercentage;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Thread install;
            status.Text = "";
            if (cmdArgs.Length == 2 && cmdArgs[1].Contains("/full"))
            {
                downloadedFiles.Clear();
                downloadUrls.Enqueue("http://support.agroprosperis.com/video/AP.ActiveX/sdk/vcredist_2005x86.exe");
                downloadUrls.Enqueue("http://support.agroprosperis.com/video/AP.ActiveX/sdk/vcredist_2010x86.exe"); ;
                downloadUrls.Enqueue("http://support.agroprosperis.com/video/AP.ActiveX/sdk/vcredist_2010x64.exe");
                downloadUrls.Enqueue("http://support.agroprosperis.com/video/AP.ActiveX/sdk/cammonitorinstaller.msi");
                downloadUrls.Enqueue("http://support.agroprosperis.com/video/AP.ActiveX/sdk/CodecPackInstaller.msi");
                downloadUrls.Enqueue("http://support.agroprosperis.com/video/AP.ActiveX/sdk/CamMonitor.ocx");
                status.AppendText("завантаження файлів компонент рорзобника...\r\n");
                DownloadFile();
                //install = new Thread(() => Install(cmdArgs[0], true));
                //Install(cmdArgs[0], true);
            }
            else
            {
                //install = new Thread(() => Install(cmdArgs[0], false));
                Install(cmdArgs[0], false);
            }
            //install.Start();
        }

        private void status_TextChanged(object sender, TextChangedEventArgs e)
        {
            ((TextBox)sender).ScrollToEnd();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            status.AppendText("видалення тимчасових файлів...\r\n");
            foreach (string file in downloadedFiles)
            {
                try
                {
                    File.Delete(file);
                }
                catch (Exception ex)
                {
                    status.AppendText("помилка при видаленні файлів:\r\n" + ex.Message + "\r\n");
                }
            }
        }
    }
}
