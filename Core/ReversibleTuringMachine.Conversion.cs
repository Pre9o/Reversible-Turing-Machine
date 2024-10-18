using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;
using static ReversibleTuringMachine.Core.ReversibleTuringMachine;

namespace ReversibleTuringMachine.Core; 
public partial class ReversibleTuringMachine {

    private new static List<Quadruple> ConvertQuintuplesToQuadruples(List<Quintuple> quintuples, List<int> states) {
        var quadruples = new List<Quadruple>();

        foreach (var quintuple in quintuples) {
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

    public ReversibleTuringMachine(TuringMachine tm) {
        States = new(tm.States);
        ComputeTransitions = ConvertQuintuplesToQuadruples(tm.Transitions, States);

        // verifica se tem o numero certo de transicoes
        if (ComputeTransitions.Count != 2 * tm.Transitions.Count) {
            throw new TuringException("Number of quadruples is not twice the number of quintuples");
        }
        if (tm.States.Count + tm.Transitions.Count != States.Count) {
            throw new TuringException($"Expected one new state for each quintuple. Expected: {tm.States.Count + tm.Transitions.Count}.\n" +
                $"Got: {States.Count}");
        }

        CurrentState = tm.CurrentState;
        FinalState = tm.FinalState;
        InputTape = tm.InputTape;

        // move eles para comecarem depois do limitador de fim esquerdo
        InputTape.Move(Direction.R);
        HistoryTape.Move(Direction.R);
        OutputTape.Move(Direction.R);

        // usa os alfabetos para criar transicoes de copia
        CopyTransitions = [];
        foreach(var symbol in tm.TapeAlphabet) {
            if(symbol == "$") {
                continue;
            }
        }
    }
}
