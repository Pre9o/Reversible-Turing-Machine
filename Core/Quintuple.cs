namespace ReversibleTuringMachine.Core;

public class Quintuple
{
    public int CurrentState { get; set; }
    public char ReadSymbol { get; set; }
    public char WriteSymbol { get; set; }
    public int NextState { get; set; }
    public Direction MoveDirection { get; set; }

    public Quintuple(int currentState, char readSymbol, char writeSymbol, int nextState, Direction moveDirection)
    {
        CurrentState = currentState;
        ReadSymbol = readSymbol;
        WriteSymbol = writeSymbol;
        NextState = nextState;
        MoveDirection = moveDirection;
    }

    public override string ToString()
    {
        return $"({CurrentState}, {ReadSymbol}) -> ({NextState}, {WriteSymbol}, {MoveDirection})";
    }
}