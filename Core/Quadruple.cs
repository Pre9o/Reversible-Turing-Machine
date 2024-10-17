namespace ReversibleTuringMachine.Core;

public class Quadruple {
    public int StartState { get; set; }

    public int EndState { get; set; }

    public List<TapeActionIn> ActionIn { get; set; } = [];

    public List<TapeActionOut> ActionOut { get; set; } = [];

    public struct TapeActionIn {
        public bool Read { get; set; }
        public char SymbolRead { get; set; }
    }

    public struct TapeActionOut {
        public bool Write { get; set; }
        public bool Move { get; set; }
        public int MoveDirection { get; set; }
        public char SymbolWritten { get; set; }
    }
}