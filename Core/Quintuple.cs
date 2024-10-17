namespace ReversibleTuringMachine.Core;

public class Quintuple
{
    public int CurrentState { get; set; }
    public string ReadSymbol { get; set; }
    public string WriteSymbol { get; set; }
    public int NextState { get; set; }
    public Direction MoveDirection { get; set; }

    public Quintuple(int currentState, string readSymbol, string writeSymbol, int nextState, Direction moveDirection)
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