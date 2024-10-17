using System.Text.RegularExpressions;

namespace ReversibleTuringMachine.Core;

public partial class TuringMachine
{
    protected List<char> inputTape = [];
    protected List<char> inputAlphabet = [];
    protected List<char> tapeAlphabet = [];
    protected List<Quintuple> transitions = [];
    private List<int> states = [];
    private int currentState = -1;
    private int finalState = -1;

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
        var statesRead = f2.Split(' ').Select(int.Parse).ToList();
        if (statesRead.Count != numStates) {
            throw new Exception($"Invalid number of states. Expected {numStates}. Got {statesRead.Count}!");
        }
        tm.states = statesRead.ToList();
        tm.currentState = tm.states.First();
        tm.finalState = tm.states.Last();

        string? f3 = await sr.ReadLineAsync() ?? throw new Exception("Could not read line 3");
        List<char> starterAlphabet = f3.Split(' ')
            .Where(x => x.Length > 0)
            .Select(x => x[0])
            .ToList();
        if(starterAlphabet.Count != numAlphaInput) {
            throw new Exception($"Invalid number of input alphabet symbols. Expected {numAlphaInput}. Got {starterAlphabet.Count}!");
        }
        tm.inputAlphabet = starterAlphabet;

        string? f4 = await sr.ReadLineAsync() ?? throw new Exception("Could not read line 4");
        List<char> machineAlphabet = f4.Split(' ')
            .Where(x => x.Length>0)
            .Select(x => x[0])
            .ToList();
        if (machineAlphabet.Count != numAlphaTape) {
            throw new Exception($"Invalid number of machine alphabet symbols. Expected {numAlphaTape}. Got {machineAlphabet.Count}!");
        }
        tm.tapeAlphabet = machineAlphabet;

        List<Quintuple> quints = [];
        for(int i = 0; i < numTransitions; i++) {
            string line = await sr.ReadLineAsync() ?? throw new Exception("Could not read transition line");
            Regex r = TransitionRegex();
            Match m = r.Match(line);
            Direction direction = m.Groups["dir"].Value switch {
                "L" => Direction.L,
                "R" => Direction.R,
                _ => throw new Exception($"Invalid direction. Expected 'L' or 'R'. Got: '{m.Groups["dir"].Value}'")
            };
            Quintuple quint = new(
                currentState: int.Parse(m.Groups["a0"].Value), 
                readSymbol: m.Groups["b0"].Value[0], 
                writeSymbol: m.Groups["b1"].Value[0], 
                nextState: int.Parse(m.Groups["a1"].Value), 
                direction);
            quints.Add(quint);
        }

        var input = await sr.ReadLineAsync() ?? throw new Exception("Could not read input");
        tm.inputTape = input.ToList();
        return tm;
    }

    public List<Quadruple> ConvertQuintuplesToQuadruples(List<Quintuple> quintuples)
    {
        var quadruples = new List<Quadruple>();

        foreach (var quintuple in quintuples)
        {
            var quadruple = new Quadruple
            {
                StartState = quintuple.CurrentState,
                EndState = quintuple.NextState,
                ActionIn = new List<Quadruple.TapeActionIn>
                {
                    new Quadruple.TapeActionIn
                    {
                        Read = true,
                        SymbolRead = quintuple.ReadSymbol
                    }
                },
                ActionOut = new List<Quadruple.TapeActionOut>
                {
                    new Quadruple.TapeActionOut
                    {
                        Write = true,
                        SymbolWritten = quintuple.WriteSymbol,
                        Move = true,
                        MoveDirection = quintuple.MoveDirection == Direction.R ? 1 : -1
                    }
                }
            };

            quadruples.Add(quadruple);
        }

        return quadruples;
    }

    public void Run()
    {
        int tapePosition = 0;

        while (currentState != finalState)
        {
            char currentSymbol = inputTape[tapePosition];

            Quintuple? trans = transitions.Find(x => x.CurrentState == currentState && x.ReadSymbol == currentSymbol);
            if (trans is not null)
            {
                inputTape[tapePosition] = trans.WriteSymbol;
                currentState = trans.NextState;

                tapePosition += trans.MoveDirection == Direction.R ? 1 : -1;

                if (tapePosition >= inputTape.Count) inputTape.Add('B');
                if (tapePosition < 0) { inputTape.Insert(0, 'B'); tapePosition = 0; }
            }
            else
            {
                Console.WriteLine("Erro: Transição não encontrada!");
                return;
            }
        }

        //outputTape.AddRange(inputTape);

        Console.WriteLine("Simulação concluída!");
    }

    public void PrintTapes()
    {
        Console.WriteLine($"Input Tape: {string.Join("", inputTape)}");
        Console.WriteLine($"History Tape: {string.Join("", historyTape)}");
        Console.WriteLine($"Output Tape: {string.Join("", outputTape)}");
    }

    [GeneratedRegex(@"\((?<a0>\d+),(?<b0>.+)\)=\((?<a1>\d+),(?<b1>.+),(?<dir>[LR])\)")]
    private static partial Regex TransitionRegex();

    public ReversibleTuringMachine ToReversible() {
        ReversibleTuringMachine rtm = new();

        return rtm;
    }
}
