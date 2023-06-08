namespace goldfish.Core.Data.Optimization;

public class StateEvaluationCache
{
    public Side Checked = Side.None;
    public bool[][,] AttackCache = new bool[2][,];
    public IEnumerable<ChessMove>[,] CachedMoves = new IEnumerable<ChessMove>[8,8];
}