namespace goldfish.Core.Data.Optimization;

public class StateEvaluationCache
{
    public Side Checked = Side.None;
    public bool[][,] AttackCache = new bool[2][,];
    public IEnumerable<ChessMove>[,] CachedMoves = new IEnumerable<ChessMove>[8,8];
    public List<(int, int)>[,] CachedAttacks = new List<(int, int)>[8,8];
}