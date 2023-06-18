using goldfish.Core.Data;

namespace goldfish.Engine.Analysis;

public class Utils
{
    public static double DistFromCenter((int, int) pos)
    {
        return Math.Abs(pos.Item1 - 3.5) + Math.Abs(pos.Item2 - 3.5);
    }
    public static int DistFromPiece((int, int) pos, (int, int) piece)
    {
        return Math.Abs(pos.Item1 - piece.Item1) + Math.Abs(pos.Item2 - piece.Item2);
    }
    public static int DistFromPromotion((int, int) pawnPos, Side fromSide)
    {
        if (fromSide == Side.White)
        {
            return 7 - pawnPos.Item1;
        }
        return pawnPos.Item1;
    }
}