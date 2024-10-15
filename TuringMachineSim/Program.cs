public class Program
{
    public static void Main(string[] args)
    {
        Quintuple quintuple = new Quintuple(1, '0', '$', 2, Direction.R);
        var (quad1, quad2) = QuintupleConverter.QuintupleToQuadruple(quintuple);
        Console.WriteLine(quintuple);
        Console.WriteLine(quad1);
        Console.WriteLine(quad2);
    }
}