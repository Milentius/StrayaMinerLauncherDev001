using Microsoft.VisualBasic;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
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

namespace StrayaMinerLauncherDev001.Pages
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            InitializeComponent();
            // load sliders
            initSlider_Affinity();
            initSlider_Priority();
            InitCheckboxes();
            // load paths to required files into the textboxes
            tboxPathWallet.Text = Properties.Settings.Default.PathToWallet;
            tboxPathWallet.Text = Properties.Settings.Default.PathToCLI;
            tboxPathWallet.Text = Properties.Settings.Default.PathToMiner;


        }

        

        private void btnPathWallet_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.DefaultExt = "exe";
            ofd.Filter = "Executable Files (*.exe)|*.exe";
            bool? result = ofd.ShowDialog();
            if(result == true)
            {
                string filename = ofd.FileName;
                if (filename.Contains("strayacoin-qt.exe"))
                {
                    tboxPathWallet.Text = filename;
                }
                else
                {
                    MessageBoxResult msgResult = MessageBox.Show("The file you have selected is not the Wallet file (strayacoin-qt.exe)", "Error", MessageBoxButton.OK);
                    if (msgResult == MessageBoxResult.OK)
                    {
                        tboxPathWallet.Text = "";
                    }
                }
            }
        }

        private void btnPathCLI_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.DefaultExt = ".exe";
            ofd.Filter = "Executable Files (*.exe)|*.exe";
            bool? result = ofd.ShowDialog();
            if (result == true)
            {
                string filename = ofd.FileName;
                if (filename.Contains("strayacoin-cli.exe"))
                {
                    tboxPathCLI.Text = filename;
                }
                else
                {
                    MessageBox.Show("The file you have selected is not the wallet file (strayacoin-cli.exe)", "Error", MessageBoxButton.OK);
                    MessageBoxResult msgResult = MessageBox.Show("Error", "The file you have selected is not the CLI file (strayacoin-cli.exe)", MessageBoxButton.OK);
                    if (msgResult == MessageBoxResult.OK)
                    {
                        tboxPathWallet.Text = "";
                    }
                }
            }
        }

        private void btnPathMine_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.DefaultExt = ".bat";
            ofd.Filter = "Batch Files (*.bat)|*.bat";
            bool? ofdresult = ofd.ShowDialog();
            if (ofdresult == true)
            {
                string filename = ofd.FileName;
                if (filename.Contains("mine.bat"))
                {
                    tboxPathMine.Text = filename;
                }
                else
                {
                    MessageBoxResult msgResult = MessageBox.Show("The file you have selected is not the Miner file (mine.bat)", "Error", MessageBoxButton.OK);
                    if(msgResult == MessageBoxResult.OK)
                    {
                        tboxPathMine.Text = "";
                    }                
                }

            }
        }

        private void btnResetToDefault_Click(object sender, RoutedEventArgs e)
        {
            //if this button is pressed we should show a messagebox stating that this wil reset al lthe settings to their default values and there is is no way to undo the changes if they proceed
            // i have comentedthis out untill the if statement is implemented
            
            //tboxPathCLI.Text = "";
            //tboxPathWallet.Text = "";
            //tboxPathMine.Text = "";
            //checkboxCpuSettings.IsChecked = false;
            //Properties.Settings.Default.Save();
            //Properties.Settings.Default.Reload();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Save_NumCores();
            Save_PathSettings();
            Save_SliderValues();
            Save_CpuCheckBoxStates();
            Save_VerbositySettings();
            
            // save the settings to the application
            Properties.Settings.Default.Save();
            Properties.Settings.Default.Reload();
        }

        private void Save_VerbositySettings()
        {
            // save the verbosity settings checkbox states
            if (checkboxCustomVerbositySettings.IsChecked == true)
            {
                Properties.Settings.Default.CustomVerbosityEnabled = true;
            }
        }

        private void Save_CpuCheckBoxStates()
        {
            // save checkbox states
            if (checkboxCpuSettings.IsChecked == true)
            {
                Properties.Settings.Default.CpuSettingsEnabled = true;
            }

        }

        private void Save_SliderValues()
        {
            // save the value of the sliders
            Properties.Settings.Default.AffinitySetting = sliderAffinity.Value;
            Properties.Settings.Default.PrioritySetting = sliderPriority.Value;
        }

        private void Save_PathSettings()
        {
            // save the path to the location of the files
            Properties.Settings.Default.PathToWallet = tboxPathWallet.Text;
            Properties.Settings.Default.PathToCLI = tboxPathCLI.Text;
            Properties.Settings.Default.PathToMiner = tboxPathMine.Text;
        }

        private void Save_NumCores()
        {
            //save number of cores to use
            int result;
            bool success = int.TryParse(CheckInputIsInteger(tboxCoreToUse).ToString(), out result);

            if (success)
            {
                if (result > 0)
                {
                    Properties.Settings.Default.NumberofCores = result;
                }
            }
            else
            {
                MessageBox.Show("The number of cores to use returned a value that is not an integer, saving it as 1", "cores to use error", MessageBoxButton.OK);
                Properties.Settings.Default.NumberofCores = 1;
            }
        }

        private void btnDiscard_Click(object sender, RoutedEventArgs e)
        {

        }

        private int CheckInputIsInteger(TextBox textBox)
        {
            // Try to parse the text as an integer
            int num;
            bool success = int.TryParse(textBox.Text, out num);

            // If parsing failed or the input is empty, show an error message
            if (!success || textBox.Text == "")
            {
                // show a message box to tell the user, this need to be an integer, set it to 1 in case they do not enter an integer or leave it empty
                MessageBox.Show("Please enter an integer.","Error",MessageBoxButton.OKCancel);
                return 1;
            }
            return num;
        }

        // if the Affinity slider is changed by the user we should snap the slider to the appropriate location and reflect the change in the labels
        private void sliderAffinity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //if the slider is equal to or greater than 0.5 we snap it to 0 and show the "off" label and hide the others
            if (sliderAffinity.Value <= 0.5)
            {
                sliderAffinity.Value = 0;
                checkboxCpuAffinity.IsChecked = false;
                lblAffinityOff.Visibility = Visibility.Visible;
                lblAffinity1Core.Visibility = Visibility.Hidden;
                lblAffinity1Core1Thread.Visibility = Visibility.Hidden;
                lblAffinity2Core.Visibility = Visibility.Hidden;
                lblAffinity2Core2Thread.Visibility = Visibility.Hidden;
            }
            //if the slider is greater than 0.5 and less than 1.5 we snap it to 1 and show the "1 Core" label and hide the others
            else if (sliderAffinity.Value > 0.5 & sliderAffinity.Value <= 1.5)
            {
                sliderAffinity.Value = 1;
                checkboxCpuAffinity.IsChecked = true;
                lblAffinityOff.Visibility = Visibility.Hidden;
                lblAffinity1Core.Visibility = Visibility.Visible;
                lblAffinity1Core1Thread.Visibility = Visibility.Hidden;
                lblAffinity2Core.Visibility = Visibility.Hidden;
                lblAffinity2Core2Thread.Visibility = Visibility.Hidden;
            }
            //if the slider is greater than 1.5 and less than 2.5 we snap it to 2 and show the "1 Core 1 Thread" label and hide the others
            else if (sliderAffinity.Value > 1.5 & sliderAffinity.Value <= 2.5)
            {
                sliderAffinity.Value = 2;
                checkboxCpuAffinity.IsChecked = true;
                lblAffinityOff.Visibility = Visibility.Hidden;
                lblAffinity1Core.Visibility = Visibility.Hidden;
                lblAffinity1Core1Thread.Visibility = Visibility.Visible;
                lblAffinity2Core.Visibility = Visibility.Hidden;
                lblAffinity2Core2Thread.Visibility = Visibility.Hidden;
            }
            //if the slider is greater than 2.5 and less than 3.5 we snap it to 3 and show the "2 Cores" label and hide the others
            else if (sliderAffinity.Value > 2.5 & sliderAffinity.Value <= 3.5)
            {
                sliderAffinity.Value = 3;
                checkboxCpuAffinity.IsChecked = true;
                lblAffinityOff.Visibility = Visibility.Hidden;
                lblAffinity1Core.Visibility = Visibility.Hidden;
                lblAffinity1Core1Thread.Visibility = Visibility.Hidden;
                lblAffinity2Core.Visibility = Visibility.Visible;
                lblAffinity2Core2Thread.Visibility = Visibility.Hidden;
            }
            //if the slider is greater than 3.5 and less than 4.5 we snap it to 4 and show the "2 Cores 2 Threads" label and hide the others
            else if (sliderAffinity.Value > 3.5 & sliderAffinity.Value <= 4)
            {
                sliderAffinity.Value = 4;
                checkboxCpuAffinity.IsChecked = true;
                lblAffinityOff.Visibility = Visibility.Hidden;
                lblAffinity1Core.Visibility = Visibility.Hidden;
                lblAffinity1Core1Thread.Visibility = Visibility.Hidden;
                lblAffinity2Core.Visibility = Visibility.Hidden;
                lblAffinity2Core2Thread.Visibility = Visibility.Visible;
            }
        }

        // if the Priority slider is changed by the user we should snap the slider to the appropriate location and reflect the change in the labels
        private void sliderPriority_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //if the slider is equal to or greater than 0.5 we snap it to 0 and show the "off" label and hide the others
            if (sliderPriority.Value <= 0.5)
            {
                sliderPriority.Value = 0;
                checkboxCpuPriority.IsChecked = false;
                lblPriorityOff.Visibility = Visibility.Visible;
                lblPriorityLow.Visibility = Visibility.Hidden;
                lblPriorityBelowNormal.Visibility = Visibility.Hidden;
                lblPriorityAboveNormal.Visibility = Visibility.Hidden;
                lblPriorityHigh.Visibility = Visibility.Hidden;
                lblPriorityRealtime.Visibility = Visibility.Hidden;
            }
            //if the slider is greater than 0.5 and less than 1.5 we snap it to 1 and show the "Low" label and hide the others
            else if (sliderPriority.Value > 0.5 & sliderPriority.Value < 1.5)
            {
                sliderPriority.Value = 1;
                checkboxCpuPriority.IsChecked = true;
                lblPriorityOff.Visibility = Visibility.Hidden;
                lblPriorityLow.Visibility = Visibility.Visible;
                lblPriorityBelowNormal.Visibility = Visibility.Hidden;
                lblPriorityAboveNormal.Visibility = Visibility.Hidden;
                lblPriorityHigh.Visibility = Visibility.Hidden;
                lblPriorityRealtime.Visibility = Visibility.Hidden;
            }
            //if the slider is greater than 1.5 and less than 2.5 we snap it to 2 and show the "Below Normal" label and hide the others
            else if (sliderPriority.Value > 1.5 & sliderPriority.Value < 2.5)
            {
                sliderPriority.Value = 2;
                checkboxCpuPriority.IsChecked = true;
                lblPriorityOff.Visibility = Visibility.Hidden;
                lblPriorityLow.Visibility = Visibility.Hidden;
                lblPriorityBelowNormal.Visibility = Visibility.Visible;
                lblPriorityAboveNormal.Visibility = Visibility.Hidden;
                lblPriorityHigh.Visibility = Visibility.Hidden;
                lblPriorityRealtime.Visibility = Visibility.Hidden;
            }
            //if the slider is greater than 2.5 and less than 3.5 we snap it to 3 and show the "Above Normal" label and hide the others
            else if (sliderPriority.Value > 2.5 & sliderPriority.Value < 3.5)
            {
                sliderPriority.Value = 3;
                checkboxCpuPriority.IsChecked = true;
                lblPriorityOff.Visibility = Visibility.Hidden;
                lblPriorityLow.Visibility = Visibility.Hidden;
                lblPriorityBelowNormal.Visibility = Visibility.Hidden;
                lblPriorityAboveNormal.Visibility = Visibility.Visible;
                lblPriorityHigh.Visibility = Visibility.Hidden;
                lblPriorityRealtime.Visibility = Visibility.Hidden;
            }
            // if the slider is greater than 3.5 and less than 4.5 we snap it to 4 and show the "high" label and hide the others
            else if (sliderPriority.Value > 3.5 & sliderPriority.Value < 4.5)
            {
                sliderPriority.Value = 4;
                checkboxCpuPriority.IsChecked = true;
                lblPriorityOff.Visibility = Visibility.Hidden;
                lblPriorityLow.Visibility = Visibility.Hidden;
                lblPriorityBelowNormal.Visibility = Visibility.Hidden;
                lblPriorityAboveNormal.Visibility = Visibility.Hidden;
                lblPriorityHigh.Visibility = Visibility.Visible;
                lblPriorityRealtime.Visibility = Visibility.Hidden;
            }
            // if the slider is greater 4.5 we snap it to 5 and show the "Realtime" label and hide the others
            else if (sliderPriority.Value > 4.5)
            {
                sliderPriority.Value = 5;
                checkboxCpuPriority.IsChecked = true;
                lblPriorityOff.Visibility = Visibility.Hidden;
                lblPriorityLow.Visibility = Visibility.Hidden;
                lblPriorityBelowNormal.Visibility = Visibility.Hidden;
                lblPriorityAboveNormal.Visibility = Visibility.Hidden;
                lblPriorityHigh.Visibility = Visibility.Hidden;
                lblPriorityRealtime.Visibility = Visibility.Visible;
            }
        }

        // this function initializes the sliders for CPU Priority
        private double initSlider_Priority()
        {
            // check if the priority slider value is 0
             if (Properties.Settings.Default.PrioritySetting == 0)
             {
                // if the setting for priority is 0 then we set the slider to 0 and the checkbox to false
                checkboxCpuPriority.IsChecked = false;
                sliderPriority.Value = 0;
                // set the visibility of the labels to reflect the 0 value of the slider
                lblPriorityOff.Visibility = Visibility.Visible;
                lblPriorityLow.Visibility = Visibility.Hidden;
                lblPriorityBelowNormal.Visibility = Visibility.Hidden;
                lblPriorityAboveNormal.Visibility = Visibility.Hidden;
                lblPriorityHigh.Visibility = Visibility.Hidden;
                lblPriorityRealtime.Visibility = Visibility.Hidden;
                return 0;
             }
             // check if the slider value is 1 
             else if (Properties.Settings.Default.PrioritySetting == 1)
             {
                // if the setting for priority is 1 then we set the slider to 1 and the checkbox to true
                checkboxCpuPriority.IsChecked = true;
                sliderPriority.Value = 1;
                lblPriorityOff.Visibility = Visibility.Hidden;
                lblPriorityLow.Visibility = Visibility.Visible;
                lblPriorityBelowNormal.Visibility = Visibility.Hidden;
                lblPriorityAboveNormal.Visibility = Visibility.Hidden;
                lblPriorityHigh.Visibility = Visibility.Hidden;
                lblPriorityRealtime.Visibility = Visibility.Hidden;
                return 1;
             }
            // check if the priority slider is 2
            else if (Properties.Settings.Default.PrioritySetting == 2)
             {
                // if the setting for priority is 2 then we set the slider to 2 and the checkbox to true
                checkboxCpuPriority.IsChecked = true;
                sliderPriority.Value = 2;
                lblPriorityOff.Visibility = Visibility.Hidden;
                lblPriorityLow.Visibility = Visibility.Hidden;
                lblPriorityBelowNormal.Visibility = Visibility.Visible;
                lblPriorityAboveNormal.Visibility = Visibility.Hidden;
                lblPriorityHigh.Visibility = Visibility.Hidden;
                lblPriorityRealtime.Visibility = Visibility.Hidden;
                return 2;
             }
            // check if the priority slider is 3
            else if (Properties.Settings.Default.PrioritySetting == 3)
             {
                // if the setting for priority is 3 then we set the slider to 3 and the checkbox to true
                checkboxCpuPriority.IsChecked = true;
                sliderPriority.Value = 3;
                lblPriorityOff.Visibility = Visibility.Hidden;
                lblPriorityLow.Visibility = Visibility.Hidden;
                lblPriorityBelowNormal.Visibility = Visibility.Hidden;
                lblPriorityAboveNormal.Visibility = Visibility.Visible;
                lblPriorityHigh.Visibility = Visibility.Hidden;
                lblPriorityRealtime.Visibility = Visibility.Hidden;
                return 3;
             }
            // check if the priority slider is 4
            else if (Properties.Settings.Default.PrioritySetting == 4)
             {
                // if the setting for priority is 4 then we set the slider to 4 and the checkbox to true
                checkboxCpuPriority.IsChecked = true;
                sliderPriority.Value = 4;
                lblPriorityOff.Visibility = Visibility.Hidden;
                lblPriorityLow.Visibility = Visibility.Hidden;
                lblPriorityBelowNormal.Visibility = Visibility.Hidden;
                lblPriorityAboveNormal.Visibility = Visibility.Hidden;
                lblPriorityHigh.Visibility = Visibility.Visible;
                lblPriorityRealtime.Visibility = Visibility.Hidden;
                return 4;
             }
            // check if the priority slider is 5
            else if (Properties.Settings.Default.PrioritySetting == 5)
             {
                // if the setting for priority is 5 then we set the slider to 5 and the checkbox to true
                checkboxCpuPriority.IsChecked = true;
                sliderPriority.Value = 5;
                lblPriorityOff.Visibility = Visibility.Visible;
                lblPriorityLow.Visibility = Visibility.Hidden;
                lblPriorityBelowNormal.Visibility = Visibility.Hidden;
                lblPriorityAboveNormal.Visibility = Visibility.Hidden;
                lblPriorityHigh.Visibility = Visibility.Hidden;
                lblPriorityRealtime.Visibility = Visibility.Visible;
                return 5;
             }
            // If none of the above conditions are met
            else
            {
                // otherwise the slider is set to 0 and set the checkbox to false
                checkboxCpuPriority.IsChecked = false;
                return 9;
             }
            
        }

        // This function initializes the slider for CPU affinity
        private double initSlider_Affinity()
        {
            // Check if AffinitySetting is 0
            if (Properties.Settings.Default.AffinitySetting == 0)
            {
                // Enable slider and uncheck checkbox
                sliderAffinity.IsEnabled = true;
                checkboxCpuAffinity.IsChecked = false;
                sliderAffinity.Value = 0;
                // Set visibility of labels
                lblAffinityOff.Visibility = Visibility.Visible;
                lblAffinity1Core.Visibility = Visibility.Hidden;
                lblAffinity1Core1Thread.Visibility = Visibility.Hidden;
                lblAffinity2Core.Visibility = Visibility.Hidden;
                lblAffinity2Core2Thread.Visibility = Visibility.Hidden;
                return 0;
            }
            // Check if AffinitySetting is 1
            else if (Properties.Settings.Default.AffinitySetting == 1)
            {
                // Enable slider and check checkbox
                sliderAffinity.IsEnabled = true;
                checkboxCpuAffinity.IsChecked = true;
                sliderAffinity.Value = 1;
                // Set visibility of labels
                lblAffinityOff.Visibility = Visibility.Hidden;
                lblAffinity1Core.Visibility = Visibility.Visible;
                lblAffinity1Core1Thread.Visibility = Visibility.Hidden;
                lblAffinity2Core.Visibility = Visibility.Hidden;
                lblAffinity2Core2Thread.Visibility = Visibility.Hidden;
                return 1;
            }
            // Check if AffinitySetting is 2
            else if (Properties.Settings.Default.AffinitySetting == 2)
            {
                // Enable slider and check checkbox
                sliderAffinity.IsEnabled = true;
                checkboxCpuAffinity.IsChecked = true;
                sliderAffinity.Value = 2;
                // Set visibility of labels
                lblAffinityOff.Visibility = Visibility.Hidden;
                lblAffinity1Core.Visibility = Visibility.Visible;
                lblAffinity1Core1Thread.Visibility = Visibility.Hidden;
                lblAffinity2Core.Visibility = Visibility.Hidden;
                lblAffinity2Core2Thread.Visibility = Visibility.Hidden;
                return 2;
            }
            // Check if AffinitySetting is 3
            else if (Properties.Settings.Default.AffinitySetting == 3)
            {
                // Enable slider and check checkbox
                sliderAffinity.IsEnabled = true;
                checkboxCpuAffinity.IsChecked = true;
                sliderAffinity.Value = 3;
                // Set visibility of labels
                lblAffinityOff.Visibility = Visibility.Hidden;
                lblAffinity1Core.Visibility = Visibility.Hidden;
                lblAffinity1Core1Thread.Visibility = Visibility.Visible;
                lblAffinity2Core.Visibility = Visibility.Hidden;
                lblAffinity2Core2Thread.Visibility = Visibility.Hidden;
                return 3;
            }
            // Check if AffinitySetting is 4
            else if (Properties.Settings.Default.AffinitySetting == 4)
            {
                // Enable slider and check checkbox
                sliderAffinity.IsEnabled = true;
                checkboxCpuAffinity.IsChecked = true;
                sliderAffinity.Value = 4;
                // Set visibility of labels
                lblAffinityOff.Visibility = Visibility.Hidden;
                lblAffinity1Core.Visibility = Visibility.Hidden;
                lblAffinity1Core1Thread.Visibility = Visibility.Hidden;
                lblAffinity2Core.Visibility = Visibility.Hidden;
                lblAffinity2Core2Thread.Visibility = Visibility.Visible;
                return 4;
            }
            // If none of the above conditions are met
            else
            {
                // Uncheck checkbox
                sliderAffinity.IsEnabled = false;
                checkboxCpuAffinity.IsChecked = false;
                return 9;
            }

        }

        private void InitCheckboxes()
        {
            // if the setting for cpu affinity is true we set the slider to the value we have saved and set the slider to enabled
            // in this case the slider value should be > 0
            if (Properties.Settings.Default.CpuAffinityEnabled == true)
            {
                sliderAffinity.IsEnabled = true;
                sliderAffinity.Value = Properties.Settings.Default.AffinitySetting;
            }
            // if the affinity setting is false we still set the value we have saved to the slider bt we disable the slider to indicate its not in use
            // in this case the slider value should be 0
            else
            {
                sliderAffinity.IsEnabled = false;
                sliderAffinity.Value = Properties.Settings.Default.AffinitySetting;
            }

            // if the setting for cpu priority is set to true we set the slider to the value we have saved and set the slider to enabled
            // in this case the slider value should be > 0
            if (Properties.Settings.Default.CpuPriorityEnabled == true)
            {
                sliderPriority.IsEnabled = true;
                sliderPriority.Value = Properties.Settings.Default.PrioritySetting;
            }
            // if the priority setting is false we still set the value we have saved to the slider bt we disable the slider to indicate its not in use
            // in this case the slider value should be 0
            else
            {
                sliderPriority.IsEnabled = false;
                sliderPriority.Value = Properties.Settings.Default.PrioritySetting;
            }
        }

        private void checkboxCpuAffinity_Checked(object sender, RoutedEventArgs e)
        {
            // if the CPU Affinnity setting is tr
            if(checkboxCpuAffinity.IsChecked == true)
            {
                sliderAffinity.Value = Properties.Settings.Default.AffinitySetting;
            }
        }

        private void checkboxCpuAffinity_Unchecked(object sender, RoutedEventArgs e)
        {
            // when the user checks the CPU affinity checkbox we should set the slider to the last know value from the settings
            if (checkboxCpuAffinity.IsChecked == false)
            {
                sliderAffinity.Value = 0;
            }
        }

        private void checkboxCpuPriority_Checked(object sender, RoutedEventArgs e)
        {
            // when the user checks the CPU Piority checkbox we should set the value of the slider to the last known value from the settings
            if (checkboxCpuPriority.IsChecked == true)
            {
                sliderPriority.Value = Properties.Settings.Default.PrioritySetting;
            }
        }

        private void checkboxCpuPriority_Unchecked(object sender, RoutedEventArgs e)
        {
            // when the user unchecks the CPU Piority checkbox we should set the value of the slider to 0
            if (checkboxCpuPriority.IsChecked == false)
            {
                sliderPriority.Value = 0;
            }
        }

        private void tboxCoreToUse_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Try to parse the text as an integer
            int num;
            bool success = int.TryParse(tboxCoreToUse.Text, out num);

            // If parsing failed or the input is empty, show an error message
            if (!success || tboxCoreToUse.Text == "")
            {
                MessageBox.Show("Please enter an integer.");
            }
        }
    }
}
