using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Timers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace StrayaMinerLauncherDev001.Pages
{
    /// <summary>
    /// Interaction logic for MinerLauncher.xaml
    /// </summary>
    public partial class MinerLauncher : Page
    {
        public MinerLauncher()
        {
            InitializeComponent();
            RunStrayacoinCli_GetMiningInfo();
        }

        public void RunStrayacoinCli_GetMiningInfo()
        {
            // setup the process that will run the strayacoin-cli.exe program
            string pathToCli = Properties.Settings.Default.PathToCLI;
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = pathToCli;
            startInfo.Arguments = "getmininginfo";
            startInfo.RedirectStandardOutput = true;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;

            // use the process to get the output 
            using (Process process = new Process())
            {
                process.StartInfo = startInfo;
                process.Start();
                //wait for the cli program to exit and then read the output to the end
                process.WaitForExit();
                string output = process.StandardOutput.ReadToEnd();

                // apply the output to a json object that we can then use to display information to the user
                JObject jsonObject = JObject.Parse(output);
                int? blocks = (int?)jsonObject["blocks"] as int?;
                double? difficulty = (double?)jsonObject["difficulty"] as double?;
                double? networkhashps = (double?)jsonObject["networkhashps"] as double?;
                string? chain = (string?)jsonObject["chain"] as string;
                // as a safety net we will use the null-coalescing operator (??) to provide a default value to the above variables if the result is null
                blocks ??= 0;
                difficulty ??= 0.0;
                networkhashps ??= 0.0;
                chain ??= "Unknown";


                // check the name of the chain and set the label accordingly, if its main we know the network is online and we set that too!
                if (chain == "main")
                {
                    lblNetworkNameResult.Foreground = Brushes.Green;
                    lblNetworkStatusResult.Foreground = Brushes.Green;

                    lblNetworkNameResult.Content = "Main";
                    lblNetworkStatusResult.Content = "Online";
                }
                else
                {
                    lblNetworkStatusResult.Foreground = Brushes.Red;
                    lblNetworkNameResult.Foreground = Brushes.Red;
                    lblNetworkNameResult.Content = "Unknown";
                    lblNetworkStatusResult.Content = "Offline";
                }

                // get the hashrate, convert to KH/s and then populate the label and set its foreground colour
                if (networkhashps.HasValue && networkhashps > 0)
                {
                    lblNetworkHashrateResult.Foreground = Brushes.Green;
                    double hashrate_khs = ((double)networkhashps / 1000);
                    double rounded_hashrate_khs = Math.Round(hashrate_khs, 2);
                    lblNetworkHashrateResult.Content = rounded_hashrate_khs + " KH/s";
                }
                else
                {
                    lblNetworkHashrateResult.Foreground = Brushes.Red;
                    lblNetworkHashrateResult.Content = "0 KH/s";
                }

                if (difficulty.HasValue && difficulty > 0.000000)
                {
                    lblNetworkDifficultyResult.Foreground = Brushes.Green;
                    double rounded_difficulty = Math.Round((double)difficulty, 6);
                    lblNetworkDifficultyResult.Content = rounded_difficulty;
                }
                else
                {
                    lblNetworkDifficultyResult.Foreground = Brushes.Red;
                    lblNetworkDifficultyResult.Content = "0";
                }
                //tblockMinerOutput.Text = blocks + "\n" + difficulty + "\n" + networkhashps + "\n" + chain + "\n";
                return;
            }
        }

        private void RunStrayacoinCli_GetNetworkInfo()
        {
            // setup the process that will run the strayacoin-cli.exe program
            string pathToCli = Properties.Settings.Default.PathToCLI;
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = pathToCli;
            startInfo.Arguments = "getnetworkinfo";
            startInfo.RedirectStandardOutput = true;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;

            // use the process to get the output 
            using (Process process = new Process())
            {
                process.StartInfo = startInfo;
                process.Start();
                //wait for the cli program to exit and then read the output to the end
                process.WaitForExit();
                string output = process.StandardOutput.ReadToEnd();

                // apply the output to a json object that we can then use to display information to the user
                JObject jsonObject = JObject.Parse(output);
            }
        }

        private void RunStrayacoinCli_GetPeerInfo()
        {
            // setup the process that will run the strayacoin-cli.exe program
            string pathToCli = Properties.Settings.Default.PathToCLI;
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = pathToCli;
            startInfo.Arguments = "getmininginfo";
            startInfo.RedirectStandardOutput = true;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;

            // use the process to get the output 
            using (Process process = new Process())
            {
                process.StartInfo = startInfo;
                process.Start();
                //wait for the cli program to exit and then read the output to the end
                process.WaitForExit();
                string output = process.StandardOutput.ReadToEnd();

                // apply the output to a json object that we can then use to display information to the user
                JObject jsonObject = JObject.Parse(output);
            }
        }

        private void UpdateTimer_Short()
        {
            DispatcherTimer UpdateTimer_Short = new DispatcherTimer();
            UpdateTimer_Short.Interval = TimeSpan.FromSeconds(30) ;
            UpdateTimer_Short.Tick += UpdateTimer_Short_tick;
            bool IsUpdateTimerRunning = false;
            if (IsUpdateTimerRunning == false)
            {
                IsUpdateTimerRunning = true;
                UpdateTimer_Short.Start();
            }
            else if (IsUpdateTimerRunning == true)
            {
                
                IsUpdateTimerRunning = false;
                UpdateTimer_Short.Stop();
            }

            
        }

        private void UpdateTimer_Short_tick(object sender, EventArgs e)
        {

        }
    }
}
