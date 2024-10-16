using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

partial
// Código do gpt desisti de entender e decidi fazer de outro jeito
class TuringMachine
{
    private List<char> inputTape = [];
    private List<char> historyTape = [];
    private List<char> outputTape = [];

    private Dictionary<(int, char), (int, char, char)> transitions = new();
    private int currentState;
    private int finalState;

    public TuringMachine() {

    }

    public TuringMachine(string path)
    {
        LoadConfiguration(path);
    }

    public static async Task<TuringMachine> FromStreamAsync(StreamReader sr) {
        var tm = new TuringMachine();
        string? f1 = await sr.ReadLineAsync() ?? throw new Exception("Could not read line 1");
        var config = f1.Split(' ');
        if(config.Length != 4) {
            throw new Exception("Missing information on line 1");
        }
        int numStates = int.Parse(config[0]);
        int numAlphaInput = int.Parse(config[1]);
        int numAlphaTape = int.Parse(config[2]);
        int numTransitions = int.Parse(config[3]);

        string? f2 = await sr.ReadLineAsync() ?? throw new Exception("Could not read line 2");
        var states = f2.Split(' ').Select(int.Parse).ToList();
        int initialState = states.First();
        int finalState = states.Last();
        List<int> intermediateStates = states.Skip(1).Take(states.Count - 2).ToList();

        string? f3 = await sr.ReadLineAsync() ?? throw new Exception("Could not read line 3");
        List<char> inputAlphabet = f3.Split(' ').Select(char.Parse).ToList();

        string? f4 = await sr.ReadLineAsync() ?? throw new Exception("Could not read line 4");
        List<char> machineAlphabet = f4.Split(' ').Select(char.Parse).ToList();

        List<Quintuple> quints = [];
        for(int i = 0; i < numTransitions; i++) {
            string line = await sr.ReadLineAsync() ?? throw new Exception("Could not read transition line");
            Regex r = TransitionRegex();
            Match m = r.Match(line);
            Direction direction = m.Groups["dir"].Value switch {
                "L" => Direction.L,
                "R" => Direction.R,
                _ => throw new Exception("Invalid direction")
            };
            Quintuple quint = new(
                currentState: int.Parse(m.Groups["a0"].Value), 
                readSymbol: m.Groups["b0"].Value[0], 
                writeSymbol: m.Groups["b1"].Value[0], 
                nextState: int.Parse(m.Groups["a1"].Value), 
                direction);
            quints.Add(quint);
        }
        return tm;
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

    [GeneratedRegex(@"\((?<a0>\d+),(?<b0>\w+)\)=\((?<a1>\d+),(?<b1>\w+),(?<dir>[LR])\)")]
    private static partial Regex TransitionRegex();
}
