namespace goldfish.Engine.Analysis;

public class Utils
{
    public static double DistFromCenter((int, int) pos)
    {
        return Math.Abs(pos.Item1 - 3.5) + Math.Abs(pos.Item2 - 3.5);
    }
}