using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        int goingBackState = States.AddState();
        int goingForwardState = States.AddState();
        foreach (var symbol in tm.TapeAlphabet)
        {
            CopyTransitions.Add(new()
            {
                StartState = FinalState,
                EndState = goingBackState,
                ActionIn = [
                    new(){
                        Read = true,
                        SymbolRead = symbol
                    },
                    new(){
                        Read = false
                    },
                    new(){
                        Read = false
                    }
                ],
                ActionOut = [
                    new(){
                        Write = false,
                        SymbolWritten = symbol,
                        Move = true,
                        MoveDirection = Direction.L
                    },
                    new(){
                        Write = false,
                        Move = false
                    },
                    new(){
                        Write = false,
                        Move = false
                    }
                ]
            });

            CopyTransitions.Add(new()
            {
                StartState = goingBackState,
                EndState = goingBackState,
                ActionIn = [
                    new(){
                        Read = true,
                        SymbolRead = symbol
                    },
                    new(){
                        Read = false
                    },
                    new(){
                        Read = false
                    }
                ],
                ActionOut = [
                    new(){
                        Write = false,
                        SymbolWritten = symbol,
                        Move = true,
                        MoveDirection = Direction.L
                    },
                    new(){
                        Write = false,
                        Move = false
                    },
                    new(){
                        Write = false,
                        Move = false
                    }
                ]
            });
            CopyTransitions.Add(new()
            {
                StartState = goingBackState,
                EndState = goingForwardState,
                ActionIn = [
                    new(){
                        Read = true,
                        SymbolRead = Tape.LimitSymbol 
                    },
                    new(){
                        Read = false
                    },
                    new(){
                        Read = false
                    }
                ],
                ActionOut = [
                    new(){
                        Move = true,
                        MoveDirection= Direction.R
                    },
                    new(){
                        Write = false,
                    },
                    new(){
                        Write = false,
                    }
                ]
            });
            if (symbol == Tape.BlankSymbol)
            {
                continue;
            }

            int intermediateState = States.AddState();

            CopyTransitions.Add(new()
            {
                StartState = goingForwardState,
                EndState = intermediateState,
                ActionIn = [
                    new(){
                        Read = true,
                        SymbolRead = symbol 
                    },
                    new(){
                        Read = false
                    },
                    new(){
                        Read = false
                    }
                ],
                ActionOut = [
                    new(){
                        Move = true,
                        MoveDirection = Direction.R
                    },
                    new(){
                        Write = false,
                    },
                    new(){
                        Write = true,
                        SymbolWritten = symbol
                    }
                ]
            });

            CopyTransitions.Add(new()
            {
                StartState = intermediateState,
                EndState = goingForwardState,
                ActionIn = [
                    new(){
                        Read = false
                    },
                    new(){
                        Read = false
                    },
                    new(){
                        Read = false
                    }
                ],
                ActionOut = [
                    new(){
                    },
                    new(){
                        Write = false,
                    },
                    new(){
                        Move = true,
                        MoveDirection = Direction.R
                    }
                ]
            });
        }
        
        int c = States.AddState();
        int copyIntermed = States.AddState();
        CopyFinalState = c;

        // Adiciona transição final para o estado final
        CopyTransitions.Add(new() {
            StartState = goingForwardState,
            EndState = copyIntermed,
            ActionIn = [
                new(){
                    Read = false,
                },
                default,
                default
            ],
            ActionOut = [
                new(){
                    Move = true,
                    MoveDirection = Direction.R
                },
                default,
                default
            ]
        });
        CopyTransitions.Add(new()
        {
            StartState = copyIntermed,
            EndState = c,
            ActionIn = [
                new(){
                    Read = true,
                    SymbolRead = Tape.BlankSymbol
                },
                new(){
                    Read = false
                },
                new(){
                    Read = false
                }
            ],
            ActionOut = [
                new(){
                    Write = true,
                    SymbolWritten = Tape.BlankSymbol, 
                    Move = false
                },
                new(){
                    Write = false,
                    Move = false
                },
                new(){
                    Write = false,
                    Move = false
                }
            ]
        });


        RetraceTransitions = [];
        // esse CF eh o ultimo estado do copy transitions
        if (ComputeTransitions.Count % 2 != 0)
        {
            throw new TuringException("Number of compute transitions is not even");
        }
        for (int i = ComputeTransitions.Count - 1; i > 0; i-=2)
        {
            var t2 = ComputeTransitions[i];
            var t1 = ComputeTransitions[i - 1];

            int cTemp = States.AddState();
            int cNew = States.AddState();

            var g1 = new Quadruple();
            g1.StartState = c;
            g1.EndState = cTemp;
            g1.ActionIn = [default, default, default];
            g1.ActionOut = [default, default, default];

            var g2 = new Quadruple();
            g2.StartState = cTemp;
            g2.EndState = cNew;
            g2.ActionIn = [default, default, default];
            g2.ActionOut = [default, default, default];

            // magia negra, inverte t1 e t2 em g1 e g2
            g1.ActionIn[0] = new Quadruple.TapeActionIn()
            {
                Read = false
            };
            g1.ActionIn[1] = new Quadruple.TapeActionIn()
            {
                Read = true,
                SymbolRead = t2.ActionOut[1].SymbolWritten
            };
            g1.ActionIn[2] = new Quadruple.TapeActionIn()
            {
                Read = false
            };
            g1.ActionOut[0] = new Quadruple.TapeActionOut()
            {
                Write = false,
                Move = true,
                MoveDirection = t2.ActionOut[0].MoveDirection.InverseDirection()
            };
            g1.ActionOut[1] = new Quadruple.TapeActionOut()
            {
                Write = true,
                Move = false,
                SymbolWritten = t2.ActionIn[1].SymbolRead
            };
            g1.ActionOut[2] = new Quadruple.TapeActionOut()
            {
                Write = false,
                Move = true,
                MoveDirection = t2.ActionOut[2].MoveDirection
            };

            g2.ActionIn[0] = new Quadruple.TapeActionIn() 
            {
                Read = true,
                SymbolRead = t1.ActionOut[0].SymbolWritten
            };
            g2.ActionIn[1] = new Quadruple.TapeActionIn() 
            {
                Read = false
            };
            g2.ActionIn[2] = new Quadruple.TapeActionIn() 
            {
                Read = true,
                SymbolRead = t1.ActionOut[2].SymbolWritten
            };
            g2.ActionOut[0] = new Quadruple.TapeActionOut() 
            {
                Write = true,
                SymbolWritten = t1.ActionIn[0].SymbolRead,
                Move = false
            };
            g2.ActionOut[1] = new Quadruple.TapeActionOut()
            {
                Write = false,
                Move = true,
                MoveDirection = t1.ActionOut[1].MoveDirection.InverseDirection()
            };
            g2.ActionOut[2] = new Quadruple.TapeActionOut() 
            {
                Write = true,
                SymbolWritten = t1.ActionIn[2].SymbolRead,
                Move = false
            };  


            RetraceTransitions.Add(g1);
            RetraceTransitions.Add(g2);
            c = cNew;
        }
    }
}   


