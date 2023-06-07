using goldfish.Core.Data;
using goldfish.Engine.Analysis.Analyzers;

namespace goldfish.Engine.Analysis;

public class GameStateAnalyzer
{
    private ChessState _state;
    private readonly IGameAnalyzer[] _analyzers;

    public GameStateAnalyzer(ChessState state)
    {
        _state = state;
        _analyzers = new IGameAnalyzer[]
        {
            new MaterialAnalyzer(),
            new WinAnalyzer()
        };
    }

    /// <summary>
    /// Evaluate the current game state with + favouring white and - favouring black
    /// </summary>
    /// <returns></returns>
    public double Evaluate()
    {
        double score = 0;
        double weighting = 0;
        foreach (var analyzer in _analyzers)
        {
            var cScore = analyzer.GetScore(_state);
            if (double.IsNaN(cScore))
            {
                return 0;
            }
            score += cScore * analyzer.Weighting;
            weighting += analyzer.Weighting;
        }

        return score / weighting;
    }
}