using Microsoft.Win32;
using ReversibleTuringMachine.Core;
using System.Diagnostics;
using System.IO;
using System.Runtime;
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

namespace ReversibleTuringMachine;

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

    private Core.ReversibleTuringMachine? rtm;

    private const string Blank = "";

    public MainWindow() {
        InitializeComponent();
        DataContext = this;

        CreateTapes(inputStackPanel, inputTapeBoxes);
        CreateTapes(historyStackPanel, historyTapeBoxes);
        CreateTapes(outputStackPanel, outputTapeBoxes);
    }



    #region User Interface

    private static void CreateTapes(StackPanel parent, List<TextBox> boxList) {
        int boxCount = 9;
        for (int i = 0; i < boxCount; i++) {
            TextBox tb = new();
            if (i == 0) {
                tb.BorderThickness = new(3);
                tb.BorderBrush = Brushes.Red;
            }
            parent.Children.Add(tb);
            boxList.Add(tb);
        }
    }

    private void UpdateInterface() {
        stepButton.IsEnabled = rtm is not null;
        if (rtm is null) {
            return;
        }
        currentStateTextbox.Text = rtm.CurrentState.ToString();
        quadruplasListView.ItemsSource = rtm.Transitions;
        RenderTape(rtm.InputTape, inputTapeBoxes, ref inputHeadPosition);
        RenderTape(rtm.HistoryTape, historyTapeBoxes, ref historyHeadPosition);
        RenderTape(rtm.OutputTape, outputTapeBoxes, ref outputHeadPosition);
    }

    private void RenderTape(Tape tape, List<TextBox> boxList, ref int headPosition) {
        for (int i = 0; i < boxList.Count; i++) {
            TextBox tb = boxList[i];
            if(i >= tape.TotalSize) {
                tb.Text = Blank;
                StyleTapeBox(tb, false);
                continue;
            }
            if (tape.Cells[i] == Tape.BlankSymbol) {
                tb.Text = Blank;
            } else {
                tb.Text = tape.Cells[i];
            }
            StyleTapeBox(tb, i == tape.HeadPosition);
        }
    }

    private static void StyleTapeBox(TextBox tb, bool isHead) {
        if (isHead) {
            tb.BorderThickness = new(3);
            tb.BorderBrush = Brushes.Red;
        } else {
            tb.BorderThickness = new(1);
            tb.BorderBrush = Brushes.Gray;
        }
    }


    #endregion

    #region Profile Links

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

    #endregion

    #region Machine Control

    private async void OpenTuringFile(object sender, RoutedEventArgs e) {
        OpenFileDialog dialog = new() {
            DefaultExt = ".txt"
        };

        if (dialog.ShowDialog() == true) {
            string path = dialog.FileName;
            using FileStream fs = File.OpenRead(path);
            using StreamReader sr = new(fs);
            TuringMachine tm = await TuringMachine.FromStreamAsync(sr);
            rtm = tm.ToReversible();
            UpdateInterface();
        }
    }

    public void Step(object sender, RoutedEventArgs e) {
        if(rtm is null) {
            MessageBox.Show("No machine loaded");
            return;
        }
        if (rtm.IsFinished()) {
            MessageBox.Show("Machine is finished");
            return;
        }

        rtm.Step();
        UpdateInterface();
    }

    #endregion
}