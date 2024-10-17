public class QuintupleConverter
{
    public static (Quadruple, Quadruple) QuintupleToQuadruple(Quintuple quintuple)
    {
        var firstQuadruple = new Quadruple(
            quintuple.CurrentState.ToString(),
            quintuple.ReadSymbol.ToString(),
            quintuple.WriteSymbol.ToString(),
            quintuple.NextState.ToString()
        );

        var secondQuadruple = new Quadruple(
            quintuple.WriteSymbol.ToString(),
            "/",
            quintuple.MoveDirection == Direction.R ? "R" : "L",
            "-"
        );

        return (firstQuadruple, secondQuadruple);
    }

}
