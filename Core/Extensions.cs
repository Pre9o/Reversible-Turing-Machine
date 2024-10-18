using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReversibleTuringMachine.Core; 
public static class Extensions {

    public static int AddState(this List<int> stages) {
        int newItem = stages[^1] + 1;
        stages.Add(newItem);
        return newItem;
    }
}
