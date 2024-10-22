using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;

namespace ReversibleTuringMachine.Core; 

// esse partial indica que a classe esta dividida
// entre varios arquivos. melhor para organizacao
public partial class ReversibleTuringMachine : TuringMachine {

    public Tape HistoryTape { get; protected set; } = new();
    public Tape OutputTape { get; protected set; }  = new();
    public List<Quadruple> ComputeTransitions { get; set; } = [];
    public List<Quadruple> CopyTransitions { get; set; } = [];
    public List<Quadruple> RetraceTransitions { get; set; }  = [];

    public int CopyFinalState { get; protected set; } = -1;
    public int RetraceFinalState { get; protected set; } = -1;

    private readonly Dictionary<Stage, bool> executionState = new() {
        { Stage.Compute, false},
        {Stage.Copy, false},
        {Stage.Retrace, false},
    };

    /// <summary>
    /// Ultima transicao tomada. Usado para
    /// deselecionar o highlight na lista
    /// de transicoes.
    /// </summary>
    private Quadruple? lastTransitionTaken = null;

    public ReversibleTuringMachine() {}

    public bool IsFinished(Stage stage) {
        return executionState[stage];
    }

    private Quadruple? FindTransition(List<Quadruple> transitions) {
        var candidateTransitions = transitions.Where(x => x.StartState == CurrentState);

        foreach (var candidate in candidateTransitions) {
            // input action
            if (candidate.ActionIn[0].Read && candidate.ActionIn[0].SymbolRead != InputTape.SymbolAtHead()) {
                continue;
            }
            // history action
            if (candidate.ActionIn[1].Read && candidate.ActionIn[1].SymbolRead != HistoryTape.SymbolAtHead()) {
                continue;
            }
            // output action
            if (candidate.ActionIn[2].Read && candidate.ActionIn[2].SymbolRead != OutputTape.SymbolAtHead()) {
                continue;
            }

            // se chegou aqui eh que nao se opoe pelos inputs, deve ser a certa
            return candidate;
        }
        return null;
    }

    private void ApplyTransition(Quadruple transition) {
        // fita input
        if (transition.ActionOut[0].Write) {
            InputTape.WriteSymbol(transition.ActionOut[0].SymbolWritten);
        } else if (transition.ActionOut[0].Move) {
            InputTape.Move(transition.ActionOut[0].MoveDirection);
        } else {
            //throw new TuringException("Invalid action. Did not move nor write on input Tape");
        }

        // fita history
        if (transition.ActionOut[1].Write) {
            HistoryTape.WriteSymbol(transition.ActionOut[1].SymbolWritten);
        } else if (transition.ActionOut[1].Move) {
            HistoryTape.Move(transition.ActionOut[1].MoveDirection);
        } else {
            //throw new TuringException("Invalid action. Did not move nor write on history Tape");
        }

        // fita output
        if (transition.ActionOut[2].Write) {
            OutputTape.WriteSymbol(transition.ActionOut[2].SymbolWritten);
        } else if (transition.ActionOut[2].Move) {
            OutputTape.Move(transition.ActionOut[2].MoveDirection);
        } else {
            //throw new TuringException("Invalid action. Did not move nor write on output Tape");
        }

        CurrentState = transition.EndState;
    }

    /// <summary>
    /// Executa um tick da maquina no 1o estagio de
    /// execucao da maquina.
    /// </summary>
    /// <exception cref="TuringException"></exception>
    public void ExecuteStep() {
        // procura a transicao certa
        Quadruple transition = FindTransition(ComputeTransitions) ?? throw new TuringException("No transition found");

        // ok, agora aplicamos a transicao
        ApplyTransition(transition);

        if(lastTransitionTaken is not null) {
            lastTransitionTaken.IsActive = false;
        }
        transition.IsActive = true;
        lastTransitionTaken = transition;

        // verifica se acabou
        if (CurrentState == FinalState) {
            executionState[Stage.Compute] = true;
        }
    }

    /// <summary>
    /// Executa um tick da maquina para copiar 
    /// </summary>
    public void CopyStep() {
        Quadruple transition = FindTransition(CopyTransitions) ?? throw new TuringException("No transition found");
        ApplyTransition(transition);
        // verifica se acabou. como?

        if (lastTransitionTaken is not null)
        {
            lastTransitionTaken.IsActive = false;
        }
        transition.IsActive = true;
        lastTransitionTaken = transition;

        if (CurrentState == CopyFinalState)
        {
            executionState[Stage.Copy] = true;
        }
    }

    public void RetraceStep() {
        Quadruple transition = FindTransition(RetraceTransitions) ?? throw new TuringException("No transition found");
        ApplyTransition(transition);

        if (lastTransitionTaken is not null)
        {
            lastTransitionTaken.IsActive = false;
        }
        transition.IsActive = true;
        lastTransitionTaken = transition;

        if (CurrentState == RetraceFinalState)
        {
            executionState[Stage.Retrace] = true;
        }
    }

    public enum Stage {
        Compute,
        Copy,
        Retrace
    }
}
