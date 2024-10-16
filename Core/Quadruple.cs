public class Quadruple
{
    public string First { get; set; }
    public string Second { get; set; }
    public string Third { get; set; }
    public string Fourth { get; set; }

    public Quadruple(string first, string second, string third, string fourth)
    {
        First = first;
        Second = second;
        Third = third;
        Fourth = fourth;
    }

    public override string ToString()
    {
        return $"({First}, {Second}, {Third}, {Fourth})";
    }
}