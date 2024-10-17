using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReversibleTuringMachine.Core;
public class TuringException : Exception {
    public TuringException() {
    }

    public TuringException(string? message) : base(message) {
    }
}
