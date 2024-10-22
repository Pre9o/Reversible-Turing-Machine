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
    private readonly List<Emoji.Wpf.RichTextBox> inputTapeBoxes = [];
    private readonly List<Emoji.Wpf.RichTextBox> historyTapeBoxes = [];
    private readonly List<Emoji.Wpf.RichTextBox> outputTapeBoxes = [];
    private int inputHeadPosition = 0;   // atualizar isso quando for muito para a direita
    private int historyHeadPosition = 0; // e precisar rolar a fita
    private int outputHeadPosition = 0;

    private Core.ReversibleTuringMachine? rtm;

    private const string Blank = "";
    private const string Limit = "❌";

    public MainWindow() {
        InitializeComponent();
        DataContext = this;

        CreateTapes(inputStackPanel, inputTapeBoxes);
        CreateTapes(historyStackPanel, historyTapeBoxes);
        CreateTapes(outputStackPanel, outputTapeBoxes);
    }



    #region User Interface

    private static void CreateTapes(StackPanel parent, List<Emoji.Wpf.RichTextBox> boxList) {
        int boxCount = 9;
        for (int i = 0; i < boxCount; i++) {
            Emoji.Wpf.RichTextBox tb = new();
            if (i == 0) {
                tb.BorderThickness = new(3);
                tb.BorderBrush = Brushes.Red;
            }
            parent.Children.Add(tb);
            boxList.Add(tb);
        }
    }

    private void UpdateInterface() {
        computeStepButton.IsEnabled = !(rtm?.IsFinished(Core.ReversibleTuringMachine.Stage.Compute) ?? false);
        copyStepButton.IsEnabled = (!(rtm?.IsFinished(Core.ReversibleTuringMachine.Stage.Copy) ?? false))
            && !computeStepButton.IsEnabled;
        retraceStepButton.IsEnabled = (!(rtm?.IsFinished(Core.ReversibleTuringMachine.Stage.Retrace) ?? false))
            && !computeStepButton.IsEnabled 
            && !copyStepButton.IsEnabled;
        if (rtm is null) {
            return;
        }
        currentStateTextbox.Text = rtm.CurrentState.ToString();
        computeTransitionsListView.ItemsSource = null; // tem q setar p/ null para forcar refresh do controle
        computeTransitionsListView.ItemsSource = rtm.ComputeTransitions;

        copyTransitionsListView.ItemsSource = null;
        copyTransitionsListView.ItemsSource = rtm.CopyTransitions;

        retraceTransitionsListView.ItemsSource = null;
        retraceTransitionsListView.ItemsSource = rtm.RetraceTransitions;

        RenderTape(rtm.InputTape, inputTapeBoxes, ref inputHeadPosition);
        RenderTape(rtm.HistoryTape, historyTapeBoxes, ref historyHeadPosition);
        RenderTape(rtm.OutputTape, outputTapeBoxes, ref outputHeadPosition);
        inputPosLabel.Text = $"Pos: {rtm.InputTape.HeadPosition}";
        historyPosLabel.Text = $"Pos: {rtm.HistoryTape.HeadPosition}";
        outputPosLabel.Text = $"Pos: {rtm.OutputTape.HeadPosition}";
    }

    private void RenderTape(Tape tape, List<Emoji.Wpf.RichTextBox> boxList, ref int headPosition) {
        int offset = 0;
        if (tape.TotalSize > boxList.Count)
        {
            offset = tape.HeadPosition - boxList.Count + 1;
        }
        if(offset < 0) {
            offset = 0;
        }
        for (int i = 0; i < boxList.Count; i++) {
            Emoji.Wpf.RichTextBox tb = boxList[i];
            if(i >= tape.TotalSize) {
                tb.Text = Blank;
                StyleTapeBox(tb, false);
                continue;
            }
            tb.Text = tape.Cells[i + offset] switch {
                Tape.BlankSymbol => Blank,
                Tape.LimitSymbol => Limit,
                _ => tape.Cells[i + offset],
            };
            StyleTapeBox(tb, i + offset == tape.HeadPosition);
        }
    }

    private static void StyleTapeBox(Emoji.Wpf.RichTextBox tb, bool isHead) {
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
            rtm = new Core.ReversibleTuringMachine(tm);
            UpdateInterface();
        }
    }

    public void StepCompute(object sender, RoutedEventArgs e) {
        if(rtm is null) {
            MessageBox.Show("No machine loaded");
            return;
        }
        if (rtm.IsFinished(Core.ReversibleTuringMachine.Stage.Compute)) {
            MessageBox.Show("Machine finished execution. Proceed to Copy stage.");
            UpdateInterface();
            return;
        }

        rtm.ExecuteStep();
        UpdateInterface();
    }

    public void StepCopy(object sender, RoutedEventArgs e) {
        if (rtm is null) {
            MessageBox.Show("No machine loaded");
            return;
        }
        if (rtm.IsFinished(Core.ReversibleTuringMachine.Stage.Copy)) {
            MessageBox.Show("Machine finished copy. Proceed to Retrace stage.");
            UpdateInterface();
            return;
        }

        rtm.CopyStep();
        UpdateInterface();
    }

    public void StepRetrace(object sender, RoutedEventArgs e) {
        if (rtm is null) {
            MessageBox.Show("No machine loaded");
            return;
        }
        if (rtm.IsFinished(Core.ReversibleTuringMachine.Stage.Retrace)) {
            MessageBox.Show("Machine finished all simulations.");
            UpdateInterface();
            return;
        }

        rtm.RetraceStep();
        UpdateInterface();
    }

    #endregion
}