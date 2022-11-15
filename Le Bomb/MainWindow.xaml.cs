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

namespace Le_Bomb
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int currentLevel { get; set; }
        public MainWindow()
        {
            InitializeComponent();
        }

        private void StartLevel(int level)
        {
            DirectoryInfo rootInfo = new DirectoryInfo(Directory.GetCurrentDirectory());

            switch (level)
            {
                case 1: // Find the Bomb
                    {
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
        }

        private void Door_Click(object sender, RoutedEventArgs e)
        {
            currentLevel++;
            StartLevel(currentLevel);

            Process[] iets = Process.GetProcessesByName("Woordzoeker");
            
            if (iets.Length > 0)
            {
                MessageBox.Show("Woordzoeker is al open!");
            } else
            {
                DirectoryInfo root = new DirectoryInfo(Directory.GetCurrentDirectory());
                root = root.Parent.Parent.Parent;

                Process.Start(root.FullName + "\\Kamers\\Woordzoeker\\Woordzoeker\\Woordzoeker\\bin\\Debug\\Woordzoeker.exe");
            }
        }
    }
}
