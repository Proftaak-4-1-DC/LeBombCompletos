using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Diagnostics;
using System.IO;
using System.Media;
using System.Threading;
using System.IO.Ports;
using System.Linq;

namespace Le_Bomb
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int currentLevel { get; set; } = 1;
        private SoundPlayer soundPlayer = new SoundPlayer("door.wav");
        private Stopwatch stopwatch = new Stopwatch();

        // Serial data transfer
        private SerialPort serialPort = new SerialPort();
        private String dataString;

        public MainWindow()
        {
            InitializeComponent();

            // Open SQL connection

            // Connect serial port
            try
            {
                serialPort.PortName = "COM4";
                serialPort.BaudRate = 9600;
                serialPort.DataReceived += new SerialDataReceivedEventHandler(OnDataReceived);
                serialPort.Open();
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            stopwatch.Start();
        }

        private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            String message = serialPort.ReadExisting();
            dataString += message;

            if (!String.IsNullOrEmpty(dataString) && dataString.Last() == '\n')
            {
                TxtTimer.Content = dataString.Trim('\n');
                dataString = String.Empty;
            }
        }

        private void PushTimeDataToDataBase()
        {
            //stopwatch.Elapsed;

            // Create query

            // Insert into
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
                case 1: // Picklocker
                {
                    string filepath = rootInfo.FullName + "\\Picklocker\\LockPicker\\bin\\Release\\net6.0-windows\\LockPicker.exe";
                    ProcessStartInfo startInfo = new ProcessStartInfo(filepath);
                    startInfo.WorkingDirectory = System.IO.Path.GetDirectoryName(filepath);

                    Process.Start(startInfo);
                    break;
                }

                case 2: // Woordzoeker
                {
                    string filepath = rootInfo.FullName + "\\Woordzoeker\\Woordzoeker\\Woordzoeker\\bin\\Release\\Woordzoeker.exe";
                    Process.Start(filepath);
                    break;
                }

                case 3: // Raadsel
                {
                    string filepath = rootInfo.FullName + "\\VirtualDesktop\\index.html";
                    filepath = "\"" + filepath + "\"";
                    string arguments = "--start-maximized --start-fullscreen";

                    ProcessStartInfo startInfo = new ProcessStartInfo("chrome.exe", arguments + " " + filepath);
                    Process.Start(startInfo);
                    break;
                }

                case 4: // Find the bomb
                {
                    string filepath = rootInfo.FullName + "\\FindTheBomb\\findthebom.html";
                    filepath = "\"" + filepath + "\"";
                    string arguments = "--start-maximized --start-fullscreen";

                    ProcessStartInfo startInfo = new ProcessStartInfo("chrome.exe", arguments + " " + filepath);
                    Process.Start(startInfo);
                    break;
                }
            }

            PushTimeDataToDataBase();
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
            this.InvalidateVisual();

            soundPlayer.PlaySync();

            StartLevel();

            if (currentLevel == 5)
            {
                bool runWhileLoop = true;
                while (runWhileLoop)
                {
                    if (!IsRunning())
                    {
                        DirectoryInfo rootInfo = new DirectoryInfo(Directory.GetCurrentDirectory());
                        rootInfo = rootInfo.Parent.Parent.Parent;
                        rootInfo = new DirectoryInfo(rootInfo.FullName + "\\Kamers");

                        string filepath = rootInfo.FullName + "\\FindTheBomb\\rebus.html";
                        filepath = "\"" + filepath + "\"";
                        string arguments = "--start-maximized --start-fullscreen";

                        ProcessStartInfo startInfo = new ProcessStartInfo("chrome.exe", arguments + " " + filepath);
                        Process.Start(startInfo);

                        runWhileLoop = false;
                    }
                }

                // Close SQL connection
            }
        }
    }
}
