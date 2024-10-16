using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
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

namespace ReversibleTuringMachine {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }

        private void OpenUrl(string url) {
            Process.Start(new ProcessStartInfo() {
                UseShellExecute = false,
                FileName = url
            });
        }

        private void OpenPedroProfile(object sender, MouseButtonEventArgs e) {
            OpenUrl("https://github.com/PedroRamos360");
        }

        private void OpenPregoProfile(object sender, MouseButtonEventArgs e) {
            OpenUrl("https://github.com/Pre9o");
        }

        private void OpenRodrigoProfile(object sender, MouseButtonEventArgs e) {
            OpenUrl("https://github.com/Agentew04");
        }

        private void OpenTuringFile(object sender, RoutedEventArgs e) {
            OpenFileDialog dialog = new();
            dialog.DefaultExt = ".txt";

            if (dialog.ShowDialog() == true) {
                string path = dialog.FileName;
                using FileStream fs = File.OpenRead(path);
                using StreamReader sr = new(fs);
                TuringMachine tm = TuringMachine.FromStreamAsync(sr).Result;
            }
        }
    }
}