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
using System.Diagnostics;
using System.IO;
using System.Timers;
using System.Media;

namespace Le_Bomb
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int currentLevel { get; set; } = 1;
        private bool soundPlayed = false;
        private SoundPlayer soundPlayer = new SoundPlayer("door.wav");

        public MainWindow()
        {
            InitializeComponent();
        }

        private bool IsRunning()
        {
            Process[] processes = Process.GetProcesses();
            
            foreach (Process process in processes)
            {
                if (process.ProcessName == "Woordzoeker" || process.ProcessName == "chrome" || process.ProcessName == "LockPicker")
                    return true;
            }

            return false;
        }

        private void StartLevel()
        {
            DirectoryInfo rootInfo = new DirectoryInfo(Directory.GetCurrentDirectory());
            rootInfo = rootInfo.Parent.Parent.Parent;
            rootInfo = new DirectoryInfo(rootInfo.FullName + "\\Kamers");

            // Start next level
            switch (currentLevel)
            {
                case 1: // Find the Bomb
                {
                    string filepath = rootInfo.FullName + "\\FindTheBomb\\findthebom.html";
                    filepath = "\"" + filepath + "\"";
                    string arguments = "-start-maximized";
                    ProcessStartInfo startInfo = new ProcessStartInfo("chrome.exe", arguments + " " + filepath);
                    Process.Start(startInfo);
                    break;
                }

                case 2: // Woordzoeker
                {
                    break;
                }

                case 3: // Raadsel
                {
                    break;
                }

                case 4: // Picklocker
                {
                    break;
                }
            }

            currentLevel++;
        }

        private void Door_Click(object sender, RoutedEventArgs e)
        {
            // Do check if a level is running
            if (IsRunning())
            {
                MessageBox.Show("Uhm, je bent nog met een spel bezig... Maak die eerst af en sluit het scherm.");
                return;
            }

            // Right door?
            Button button = (Button)sender;
            if (button.Name != "BtnDoor" + currentLevel)
            {
                MessageBox.Show("Ha, valsspelen?! Dat doen we niet. Ga naar de goede deur.");
                return;
            }

            button.Background = new ImageBrush(new BitmapImage(new Uri("doorOpen.png", UriKind.Relative)));
            soundPlayed = false;
            soundPlayed = true;
            soundPlayer.PlaySync();
            StartLevel();
        }

        private void BtnDoor_MouseEnter(object sender, MouseEventArgs e)
        {
            Button button = (Button)sender;
            button.Background = new ImageBrush(new BitmapImage(new Uri("door.png", UriKind.Relative)));
        }
    }
}
