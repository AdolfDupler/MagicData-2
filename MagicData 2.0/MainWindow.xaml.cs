using MagicData_2._0.Forms;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

namespace MagicData_2._0
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        bool _instanceInstalled = false;
        bool InstanceInstalled
        {
            get { return _instanceInstalled; }
            set
            {
                _instanceInstalled = value;
                this.Dispatcher.Invoke((Action)(() => loadingBar.Value++));
                if(_instanceInstalled)
                {
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        loadingBar.Value = 50;
                        Process IntStart = new Process();
                        IntStart.StartInfo.UseShellExecute = false;
                        IntStart.StartInfo.CreateNoWindow = true;
                        IntStart.StartInfo.FileName = "SqlLocalDB.exe";
                        IntStart.StartInfo.Arguments = "s MagicData";
                        IntStart.Start();
                        IntStart.WaitForExit();
                        loadingBar.Value = 100;

                    }
                   ));
                   
                }
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            loadingBar.Value++;
            Process localDBrun = new Process();
            localDBrun.StartInfo.UseShellExecute = false;
            localDBrun.StartInfo.RedirectStandardInput = true;
            localDBrun.StartInfo.RedirectStandardOutput = true;
            localDBrun.StartInfo.FileName = "SQLLocalDB.exe";
            localDBrun.StartInfo.Arguments = "info";
            localDBrun.StartInfo.CreateNoWindow = true;
            localDBrun.OutputDataReceived += LocalDBrun_OutputDataReceived;
            localDBrun.Exited += LocalDBrun_Exited;
            try
            {
                localDBrun.Start();
                localDBrun.BeginOutputReadLine();
                
                
                
                
            }
            catch(Exception wew)
            {
                Process DBinstall;
                if (IntPtr.Size == 8)
                {
                    DBinstall = Process.Start(@"redist\LocalDBx64.msi");
                }
                else
                {
                    DBinstall = Process.Start(@"redist\LocalDBx32.msi");
                }
                DBinstall.WaitForExit();
                if(DBinstall.ExitCode != 0)
                {
                    MessageBox.Show("LocalDB must be installed. Please restart the application.");
                    Close();
                    return;
                }
                else
                {
                    MessageBox.Show("LocalDB has been installed. Please restart the application.");
                    Close();
                    return;
                }
            }

        }

        private void LocalDBrun_Exited(object sender, EventArgs e)
        {
            
            if (!InstanceInstalled)
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    loadingBar.Value = 25;
                    Process IntStart = new Process();
                    IntStart.StartInfo.UseShellExecute = false;
                    IntStart.StartInfo.CreateNoWindow = true;
                    IntStart.StartInfo.FileName = "SqlLocalDB.exe";
                    IntStart.StartInfo.Arguments = "c MagicData";
                    IntStart.Start();
                    IntStart.WaitForExit();
                    InstanceInstalled = true;


                }));
             }
        }

        private void LocalDBrun_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!String.IsNullOrEmpty(e.Data) && e.Data.Contains("MagicData"))
            {
                InstanceInstalled = true;
                
            }
            else
            {
                if (String.IsNullOrEmpty(e.Data)) Console.WriteLine(((Process)sender).ExitCode.ToString());
            }
           
            
        }

        private void loadingBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(loadingBar.Value == 100)
            {
                new MainView().Show();
                this.Close();
            }
        }
    }
}
