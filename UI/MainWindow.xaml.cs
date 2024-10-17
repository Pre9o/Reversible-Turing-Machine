using Microsoft.Win32;
using ReversibleTuringMachine.Core;
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
        private List<TextBox> inputTapeBoxes = [];
        private List<TextBox> historyTapeBoxes = [];
        private List<TextBox> outputTapeBoxes = [];
        private int inputHeadPosition = 0;
        private int historyHeadPosition = 0;
        private int outputHeadPosition = 0;


        public MainWindow() {
            InitializeComponent();

            CreateTapes(inputStackPanel, historyTapeBoxes);
            CreateTapes(historyStackPanel, historyTapeBoxes);
            CreateTapes(outputStackPanel, historyTapeBoxes);
        }

        private void CreateTapes(StackPanel parent, List<TextBox> boxList) {
            int boxCount = 9;
            for(int i = 0; i < boxCount; i++) {
                TextBox tb = new();
                if (i == 0) {
                    tb.BorderThickness = new(3);
                    tb.BorderBrush = Brushes.Red;
                }
                parent.Children.Add(tb);
            }
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

        private async void OpenTuringFile(object sender, RoutedEventArgs e) {
            OpenFileDialog dialog = new();
            dialog.DefaultExt = ".txt";

            if (dialog.ShowDialog() == true) {
                string path = dialog.FileName;
                using FileStream fs = File.OpenRead(path);
                using StreamReader sr = new(fs);
                TuringMachine tm = await TuringMachine.FromStreamAsync(sr);
            }
        }
    }
}