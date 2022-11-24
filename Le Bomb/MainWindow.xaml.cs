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
using System.Data.SqlClient;

namespace Le_Bomb
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SoundPlayer soundPlayer = new SoundPlayer("door.wav");
        private Stopwatch stopwatch = new Stopwatch();

        // Serial data transfer
        private SerialPort serialPort = new SerialPort();
        private String dataString { get; set; }
        // Database 
        private SqlConnection sqlConn = new SqlConnection("Data Source=145.220.75.88;User ID=Beheerder;Password=P@ssword;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
        private SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
        private SqlCommand sqlCommand { get; set; }
        private SqlDataReader sqlDataReader { get; set; }
        private int currentLevel { get; set; } = 1;
        private int RunID { get; set; }
        private String userName { get; set; }
        private int timer { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            // Open SQL Connection
            OpenSqlConnection();

            // Get amount of runs to determine the current run ID
            sqlCommand = new SqlCommand("SELECT COUNT(id) FROM runs", sqlConn);
            sqlDataReader = sqlCommand.ExecuteReader();
            
            while (sqlDataReader.Read())
            {
                RunID = sqlDataReader.GetInt32(0) + 1;
            }

            // Clean up
            sqlDataReader.Close();
            sqlCommand.Dispose();

            // Insert run into database
            sqlCommand = new SqlCommand("INSERT INTO runs (timer) VALUES (900)", sqlConn);
            sqlDataAdapter.InsertCommand = sqlCommand;
            sqlDataAdapter.InsertCommand.ExecuteNonQuery();

            sqlCommand.Dispose();

            // Connect serial port
            try
            {
                serialPort.PortName = "COM7";
                serialPort.BaudRate = 9600;
                serialPort.DataReceived += new SerialDataReceivedEventHandler(OnDataReceived);
                serialPort.Open();
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            stopwatch.Start();
        }

        private String ConvertSecondsToTime()
        {
            int secs, mins;
            String str = "";

            mins = timer / 60;
            secs = timer - (60 * mins);

            if (mins < 10)
                str += "0";
            str += mins;
            str += ":";

            if (secs < 10)
                str += "0";
            str += secs;

            return str;
        }

        private void OpenSqlConnection()
        {
            try
            {
                sqlConn.Open();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void UpdateSqlRun(int timer)
        {
            sqlCommand = new SqlCommand("UPDATE runs SET timer='" + timer + "' WHERE id=" + RunID, sqlConn);
            sqlDataAdapter.UpdateCommand = sqlCommand;
            sqlDataAdapter.UpdateCommand.ExecuteNonQuery();
        }

        private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            String message = serialPort.ReadExisting();
            dataString += message;

            if (!String.IsNullOrEmpty(dataString) && dataString.Last() == '\n')
            {
                String finalString = dataString.Trim('\n');
                finalString = finalString.Trim('\r');

                dataString = String.Empty;

                if (dataString.Length < 4)
                {
                    timer = Convert.ToInt32(finalString);
                    UpdateSqlRun(timer);
                }

                this.Dispatcher.Invoke(() =>
                {
                    TxtTimer.Content = ConvertSecondsToTime();
                }
                );
            }
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
