using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReversibleTuringMachine.Core; 
public class ReversibleTuringMachine : TuringMachine {

    protected List<string> historyTape = [];
    protected List<string> outputTape = [];
    public List<Quadruple> Transitions { get; init; } = [];
    
}
