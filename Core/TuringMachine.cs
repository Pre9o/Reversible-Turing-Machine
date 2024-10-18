using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace ReversibleTuringMachine.Core;

public partial class TuringMachine
{
    public Tape InputTape { get; protected set; }
    protected List<string> inputAlphabet = [];
    protected List<string> tapeAlphabet = [];
    public List<Quintuple> Transitions { get; set; } = [];
    public List<int> States { get; set; } = [];
    public int CurrentState { get; protected set; } = -1;
    public int FinalState { get; protected set; } = -1;
    
    protected TuringMachine() { }

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
        tm.States = statesRead.ToList();
        tm.CurrentState = tm.States[0];
        tm.FinalState = tm.States[^1];

        string? f3 = await sr.ReadLineAsync() ?? throw new TuringException("Could not read line 3");
        List<string> starterAlphabet = f3.Split(' ')
            .Select(x => x.Trim())
            .Where(x => x.Length > 0)
            .ToList();
        if(starterAlphabet.Count != numAlphaInput) {
            throw new TuringException($"Invalid number of input alphabet symbols. Expected {numAlphaInput}. Got {starterAlphabet.Count}!");
        }
        tm.inputAlphabet = starterAlphabet;

        string? f4 = await sr.ReadLineAsync() ?? throw new TuringException("Could not read line 4");
        List<string> machineAlphabet = f4.Split(' ')
            .Select(x => x.Trim())
            .Where(x => x.Length>0)
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
                readSymbol: m.Groups["b0"].Value, 
                writeSymbol: m.Groups["b1"].Value, 
                nextState: int.Parse(m.Groups["a1"].Value), 
                direction);
            quints.Add(quint);
        }
        tm.Transitions = quints;

        var input = await sr.ReadLineAsync() ?? throw new TuringException("Could not read input");
        tm.InputTape = new Tape(input
            .Select(x => x.ToString()) // cada char vira uma string
            .ToList());
        return tm;
    }

    public static List<Quadruple> ConvertQuintuplesToQuadruples(List<Quintuple> quintuples, List<int> states)
    {
        var quadruples = new List<Quadruple>();

        foreach (var quintuple in quintuples)
        {
            int aLinhaLinha = states.AddState();
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
                        SymbolRead = Tape.BlankSymbol
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
                        SymbolWritten = Tape.BlankSymbol,
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
                        SymbolRead = Tape.BlankSymbol
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
                        SymbolWritten = aLinhaLinha.ToString(),
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

        List<int> newStates = new(States);
        ReversibleTuringMachine rtm = new();

        rtm.ComputeTransitions = ConvertQuintuplesToQuadruples(Transitions, newStates);
        if(rtm.ComputeTransitions.Count != 2 * Transitions.Count) {
            throw new TuringException("Number of quadruples is not twice the number of quintuples");
        }
        if(States.Count + Transitions.Count != newStates.Count) {
            throw new TuringException($"Expected one new state for each quintuple. Expected: {States.Count + Transitions.Count}.\n" +
                $"Got: {newStates.Count}");
        }

        rtm.CurrentState = CurrentState;
        rtm.FinalState = FinalState;
        rtm.States = newStates;
        rtm.tapeAlphabet = tapeAlphabet;
        rtm.inputAlphabet = inputAlphabet;
        rtm.InputTape = InputTape;
        // outras fitas sao inicializadas vazias

        return rtm;
    }
}
