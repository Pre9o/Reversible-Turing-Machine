using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReversibleTuringMachine.Core; 
public class ReversibleTuringMachine : TuringMachine {

    private List<char> historyTape = [];
    private List<char> outputTape = [];

    private List<char> inputAlphabet = [];
    private List<char> tapeAlphabet = [];
    private new List<Quadruple> transitions = [];
    private List<int> states = [];
    private int currentState = -1;
    private int finalState = -1;
}
