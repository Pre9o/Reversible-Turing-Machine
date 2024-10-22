using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReversibleTuringMachine.Core;
public class Tape {
    public List<string> Cells { get; set; }

    public int HeadPosition { get; private set; } = 0;

    public int TotalSize => Cells.Count;
    //    {
    //    get {
    //        int lastBlank = Cells.IndexOf(BlankSymbol);
    //        if(lastBlank == -1) {
    //            return Cells.Count;
    //        } else {
    //            return lastBlank;
    //        }
    //    }
    //}

    public const string BlankSymbol = "B";
    public const string LimitSymbol = "^";

    public Tape(List<string>? cells = null) {
        cells ??= [];
        Cells = cells;
        Cells.Insert(0, LimitSymbol);
    }

    public void Move(Direction direction) {
        switch (direction) {
            case Direction.L:
                HeadPosition--;
                break;
            case Direction.R:
                HeadPosition++;
                break;
            case Direction.None:
                break;
            default:
                throw new TuringException("Invalid direction");
        }
        if (HeadPosition < 0) {
            throw new TuringException("Head moved out of bounds");
        }
        if (HeadPosition >= Cells.Count) {
            Cells.Add(BlankSymbol);
        }
    }

    public string SymbolAtHead() {
        if(HeadPosition < 0) {
            throw new TuringException("Head out of bounds. Negative index!");
        }

        if (HeadPosition > Cells.Count-1) {
            Cells.AddRange(Enumerable.Repeat(BlankSymbol, Math.Abs(HeadPosition - (Cells.Count-1))));
        }
        return Cells[HeadPosition];
    }

    public void WriteSymbol(string symbol) {
        if (HeadPosition < 0) {
            throw new TuringException("Head out of bounds. Negative index!");
        }

        if (HeadPosition > Cells.Count-1) {
            Cells.AddRange(Enumerable.Repeat(BlankSymbol, HeadPosition - Cells.Count));
        }
        Cells[HeadPosition] = symbol;
    }
}
