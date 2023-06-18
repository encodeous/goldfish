using goldfish.Core.Data;
using goldfish.Core.Data.Optimization;
using goldfish.Engine.Analysis.Analyzers;

namespace goldfish.Engine.Analysis;

public class GameStateAnalyzer
{
    private static readonly IGameAnalyzer[] _analyzers;

    static GameStateAnalyzer()
    {
        _analyzers = new IGameAnalyzer[]
        {
            new MaterialAnalyzer(),
            new WinAnalyzer(),
            new ControlAnalyzer(),
            new AggressionAnalyzer(),
            new PawnAnalyzer(),
            new PiecePositionAnalyzer()
        };
    }

    /// <summary>
    /// Evaluate the current game state with + favouring white and - favouring black
    /// </summary>
    /// <returns></returns>
    public static double Evaluate(in ChessState state)
    {
        ref var cache = ref Tst.Get(state);
        if (!double.IsNaN(cache.StaticEval)) return cache.StaticEval;
        double score = 0;
        foreach (var analyzer in _analyzers)
        {
            var cScore = analyzer.GetScore(state);
            if (double.IsNaN(cScore))
            {
                return cache.StaticEval = 0;
            }
            score += cScore * analyzer.Weighting;
        }

        return cache.StaticEval = score;
    }
}