using System.Text.RegularExpressions;

namespace ReversibleTuringMachine.Core;

public partial class TuringMachine
{
    protected List<char> inputTape = [];
    protected List<char> inputAlphabet = [];
    protected List<char> tapeAlphabet = [];
    protected List<Quintuple> transitions = [];
    private List<int> states = [];
    protected int currentState = -1;
    protected int finalState = -1;

    public static async Task<TuringMachine> FromStreamAsync(StreamReader sr) {
        var tm = new TuringMachine();
        string? f1 = await sr.ReadLineAsync() ?? throw new TuringException("Could not read line 1");
        var config = f1.Split(' ');
        if(config.Length != 4) {
            throw new TuringException("Missing information on line 1");
        }
        int numStates = int.Parse(config[0]);
        int numAlphaInput = int.Parse(config[1]);
        int numAlphaTape = int.Parse(config[2]);
        int numTransitions = int.Parse(config[3]);

        string? f2 = await sr.ReadLineAsync() ?? throw new TuringException("Could not read line 2");
        var statesRead = f2.Split(' ').Select(int.Parse).ToList();
        if (statesRead.Count != numStates) {
            throw new TuringException($"Invalid number of states. Expected {numStates}. Got {statesRead.Count}!");
        }
        tm.states = statesRead.ToList();
        tm.currentState = tm.states.First();
        tm.finalState = tm.states.Last();

        string? f3 = await sr.ReadLineAsync() ?? throw new TuringException("Could not read line 3");
        List<char> starterAlphabet = f3.Split(' ')
            .Where(x => x.Length > 0)
            .Select(x => x[0])
            .ToList();
        if(starterAlphabet.Count != numAlphaInput) {
            throw new TuringException($"Invalid number of input alphabet symbols. Expected {numAlphaInput}. Got {starterAlphabet.Count}!");
        }
        tm.inputAlphabet = starterAlphabet;

        string? f4 = await sr.ReadLineAsync() ?? throw new TuringException("Could not read line 4");
        List<char> machineAlphabet = f4.Split(' ')
            .Where(x => x.Length>0)
            .Select(x => x[0])
            .ToList();
        if (machineAlphabet.Count != numAlphaTape) {
            throw new TuringException($"Invalid number of machine alphabet symbols. Expected {numAlphaTape}. Got {machineAlphabet.Count}!");
        }
        tm.tapeAlphabet = machineAlphabet;

        List<Quintuple> quints = [];
        for(int i = 0; i < numTransitions; i++) {
            string line = await sr.ReadLineAsync() ?? throw new TuringException("Could not read transition line");
            Regex r = TransitionRegex();
            Match m = r.Match(line);
            Direction direction = m.Groups["dir"].Value switch {
                "L" => Direction.L,
                "R" => Direction.R,
                _ => throw new TuringException($"Invalid direction. Expected 'L' or 'R'. Got: '{m.Groups["dir"].Value}'")
            };
            Quintuple quint = new(
                currentState: int.Parse(m.Groups["a0"].Value), 
                readSymbol: m.Groups["b0"].Value[0], 
                writeSymbol: m.Groups["b1"].Value[0], 
                nextState: int.Parse(m.Groups["a1"].Value), 
                direction);
            quints.Add(quint);
        }

        var input = await sr.ReadLineAsync() ?? throw new TuringException("Could not read input");
        tm.inputTape = input.ToList();
        return tm;
    }

    public List<Quadruple> ConvertQuintuplesToQuadruples(List<Quintuple> quintuples)
    {
        var quadruples = new List<Quadruple>();

        foreach (var quintuple in quintuples)
        {
            int aLinhaLinha = states[^1] + 1;
            if(aLinhaLinha >= 10) {
                throw new TuringException("Isso vai dar erro na 2a quad, write history tape. Mover todo codigo de simbolos p/ strings");
            }
            states.Add(aLinhaLinha);
            // primeira quadrupla. Escreve na fita
            Quadruple q1 = new() {
                StartState = quintuple.CurrentState,
                EndState = aLinhaLinha,
                ActionIn = [
                    new(){ // input
                        Read = true,
                        SymbolRead = quintuple.ReadSymbol
                    },
                    new(){ // history
                        Read = false,
                    },
                    new(){ // output
                        Read = true,
                        SymbolRead = 'b'
                    }
                ],
                ActionOut = [
                    new() { // input
                        Write = true,
                        SymbolWritten = quintuple.WriteSymbol,
                        Move = false,
                    },
                    new(){ // history
                        Write = false,
                        Move = true,
                        MoveDirection = Direction.R
                    },
                    new(){ // output
                        Write = true,
                        SymbolWritten = 'b',
                        Move = false
                    }
                ]
            };

            Quadruple q2 = new() {
                StartState = aLinhaLinha,
                EndState = quintuple.NextState,
                ActionIn = [
                    new(){ // input
                        Read = false
                    },
                    new(){ // history
                        Read = true,
                        SymbolRead = 'b'
                    },
                    new(){ // output
                        Read = false
                    }
                ],
                ActionOut = [
                    new(){ // input
                        Write = false,
                        Move = true,
                        MoveDirection = quintuple.MoveDirection
                    },
                    new(){ // history
                        Write = true,
                        SymbolWritten = aLinhaLinha.ToString()[0],
                    },
                    new(){ // output
                        Write = false,
                        Move = true,
                        MoveDirection = Direction.None
                    }
                ]
            };

            quadruples.Add(q1);
            quadruples.Add(q2);
        }

        return quadruples;
    }

    [GeneratedRegex(@"\((?<a0>\d+),(?<b0>.+)\)=\((?<a1>\d+),(?<b1>.+),(?<dir>[LR])\)")]
    private static partial Regex TransitionRegex();

    public ReversibleTuringMachine ToReversible() {
        ReversibleTuringMachine rtm = new();

        return rtm;
    }
}
