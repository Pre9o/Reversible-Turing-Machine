using System.Diagnostics;

namespace ReversibleTuringMachine.Core;

public class Quadruple {
    public int StartState { get; set; }

    public int EndState { get; set; }

    public List<TapeActionIn> ActionIn { get; set; } = [];

    public List<TapeActionOut> ActionOut { get; set; } = [];

    /// <summary>
    /// Usado para mostrar qual foi a transicao usada
    /// na interface. Nao interfere no funcionamento
    /// </summary>
    public bool IsActive { get; set; } = false;

    public struct TapeActionIn {
        public bool Read { get; set; }
        public string SymbolRead { get; set; }
    }

    public struct TapeActionOut {
        public bool Write { get; set; }
        public bool Move { get; set; }
        public Direction MoveDirection { get; set; }
        public string SymbolWritten { get; set; }
    }

    public override string ToString() {
        return $"({StartState}, [{string.Join(", ", ActionIn.Select(x=> !x.Read ? "/" : x.SymbolRead))}])" +
            $"->" +
            $"([{string.Join(", ", ActionOut.Select(x => 
                x.Move ? 
                    (x.MoveDirection switch {
                        Direction.None => "0",
                        Direction.R => "+1",
                        Direction.L => "-1",
                        _ => throw new TuringException("Invalid direction to convert to string")
                    }) : 
                    (x.Write ? x.SymbolWritten : "?")
                ))}], {EndState})";
    }
}