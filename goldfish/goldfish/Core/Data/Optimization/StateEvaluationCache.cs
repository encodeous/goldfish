namespace goldfish.Core.Data.Optimization;

public class StateEvaluationCache
{
    public Side Checked = Side.None;
    public Grid8x8?[] AttackCache = new Grid8x8?[2];
    public IEnumerable<ChessMove>[,] CachedMoves = new IEnumerable<ChessMove>[8,8];
    public (int, int)[,][] CachedAttacks = new (int, int)[8,8][];
}