using System;
using System.Collections.Generic;
using System.IO;
// Código do gpt desisti de entender e decidi fazer de outro jeito
class TuringMachine
{
    private List<char> inputTape;
    private List<char> historyTape;
    private List<char> outputTape;

    private Dictionary<(int, char), (int, char, char)> transitions;
    private int currentState;
    private int finalState;

    public TuringMachine(string path)
    {
        inputTape = new List<char>();
        historyTape = new List<char>();
        outputTape = new List<char>();
        transitions = new Dictionary<(int, char), (int, char, char)>();

        LoadConfiguration(path);
    }

    private void LoadConfiguration(string path)
    {
        var lines = File.ReadAllLines(path);

        var config = lines[0].Split(' ');
        int numStates = int.Parse(config[0]);
        int numInputSymbols = int.Parse(config[1]);
        int numTapeSymbols = int.Parse(config[2]);
        int numTransitions = int.Parse(config[3]);

        var states = lines[1].Split(' ');
        var inputAlphabet = lines[2].Split(' ');
        var tapeAlphabet = lines[3].Split(' ');

        for (int i = 4; i < 4 + numTransitions; i++)
        {
            var transition = lines[i]
                .Replace("(", "")
                .Replace(")", "")
                .Split('=');

            var left = transition[0].Split(',');
            var right = transition[1].Split(',');

            int currentState = int.Parse(left[0]);
            char readSymbol = left[1][0];

            int newState = int.Parse(right[0]);
            char writeSymbol = right[1][0];
            char move = right[2][0];  // 'R' ou 'L'

            transitions.Add((currentState, readSymbol), (newState, writeSymbol, move));
        }

        var input = lines[^1];
        inputTape.AddRange(input.ToCharArray());

        currentState = 1;  // Estado inicial
        finalState = numStates;  // Último estado é o final
    }

    public void Run()
    {
        int tapePosition = 0;

        while (currentState != finalState)
        {
            char currentSymbol = inputTape[tapePosition];

            if (transitions.TryGetValue((currentState, currentSymbol), out var transition))
            {
                var (newState, writeSymbol, move) = transition;

                inputTape[tapePosition] = writeSymbol;
                currentState = newState;

                tapePosition += (move == 'R') ? 1 : -1;

                if (tapePosition >= inputTape.Count) inputTape.Add('B');
                if (tapePosition < 0) { inputTape.Insert(0, 'B'); tapePosition = 0; }
            }
            else
            {
                Console.WriteLine("Erro: Transição não encontrada!");
                return;
            }
        }

        outputTape.AddRange(inputTape);

        Console.WriteLine("Simulação concluída!");
    }

    public void PrintTapes()
    {
        Console.WriteLine($"Input Tape: {string.Join("", inputTape)}");
        Console.WriteLine($"History Tape: {string.Join("", historyTape)}");
        Console.WriteLine($"Output Tape: {string.Join("", outputTape)}");
    }

    // public static void Main(string[] args)
    // {
    //     var tm = new TuringMachine("entrada_quintupla.txt");
    //     tm.Run();
    //     tm.PrintTapes();
    // }
}
