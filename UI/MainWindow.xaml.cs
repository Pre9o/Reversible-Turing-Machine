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
        private readonly List<TextBox> inputTapeBoxes = [];
        private readonly List<TextBox> historyTapeBoxes = [];
        private readonly List<TextBox> outputTapeBoxes = [];
        private int inputHeadPosition = 0;   // atualizar isso quando for muito para a direita
        private int historyHeadPosition = 0; // e precisar rolar a fita
        private int outputHeadPosition = 0;


        public MainWindow() {
            InitializeComponent();

            CreateTapes(inputStackPanel, inputTapeBoxes);
            CreateTapes(historyStackPanel, historyTapeBoxes);
            CreateTapes(outputStackPanel, outputTapeBoxes);
        }

        private static void CreateTapes(StackPanel parent, List<TextBox> boxList) {
            int boxCount = 9;
            for(int i = 0; i < boxCount; i++) {
                TextBox tb = new();
                if (i == 0) {
                    tb.BorderThickness = new(3);
                    tb.BorderBrush = Brushes.Red;
                }
                parent.Children.Add(tb);
                boxList.Add(tb);
            }
        }

        private static void OpenUrl(string url) {
            Process.Start(new ProcessStartInfo() {
                UseShellExecute = false,
                FileName = url
            });
        }

#pragma warning disable S1075 // URIs should not be hardcoded
        private void OpenPedroProfile(object sender, MouseButtonEventArgs e) {
            OpenUrl("https://github.com/PedroRamos360");
        }

        private void OpenPregoProfile(object sender, MouseButtonEventArgs e) {
            OpenUrl("https://github.com/Pre9o");
        }

        private void OpenRodrigoProfile(object sender, MouseButtonEventArgs e) {
            OpenUrl("https://github.com/Agentew04");
        }
#pragma warning restore S1075 // URIs should not be hardcoded

        private async void OpenTuringFile(object sender, RoutedEventArgs e) {
            OpenFileDialog dialog = new() {
                DefaultExt = ".txt"
            };

            if (dialog.ShowDialog() == true) {
                string path = dialog.FileName;
                using FileStream fs = File.OpenRead(path);
                using StreamReader sr = new(fs);
                TuringMachine tm = await TuringMachine.FromStreamAsync(sr);
                Core.ReversibleTuringMachine rtm = tm.ToReversible();
            }
        }
    }
}