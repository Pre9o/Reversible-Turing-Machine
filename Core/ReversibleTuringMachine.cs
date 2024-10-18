using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReversibleTuringMachine.Core; 
public class ReversibleTuringMachine : TuringMachine {

    public Tape HistoryTape { get; protected set; } = new();
    public Tape OutputTape { get; protected set; }  = new();
    public List<Quadruple> Transitions { get; init; } = [];
    
    public bool IsFinished() {
        return CurrentState == finalState;
    }

    public void Step() {
        var candidateTransitions = Transitions.Where(x => x.StartState == CurrentState);

        Quadruple? transition = null;
        foreach(var candidate in candidateTransitions) {
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
            transition = candidate;
            break;
        }

        if (transition == null) {
            throw new TuringException("No transition found");
        }

        // ok, agora aplicamos a transicao

        // fita input
        if (transition.ActionOut[0].Write) {
            InputTape.WriteSymbol(transition.ActionOut[0].SymbolWritten);
        }else if (transition.ActionOut[0].Move) {
            InputTape.Move(transition.ActionOut[0].MoveDirection);
        } else {
            throw new TuringException("Invalid action. Did not move nor write on input Tape");
        }

        // fita history
        if (transition.ActionOut[1].Write) {
            HistoryTape.WriteSymbol(transition.ActionOut[1].SymbolWritten);
        } else if (transition.ActionOut[1].Move) {
            HistoryTape.Move(transition.ActionOut[1].MoveDirection);
        } else {
            throw new TuringException("Invalid action. Did not move nor write on history Tape");
        }

        // fita output
        if (transition.ActionOut[2].Write) {
            OutputTape.WriteSymbol(transition.ActionOut[2].SymbolWritten);
        } else if (transition.ActionOut[2].Move) {
            OutputTape.Move(transition.ActionOut[2].MoveDirection);
        } else {
            throw new TuringException("Invalid action. Did not move nor write on output Tape");
        }

        CurrentState = transition.EndState;
    }
}
