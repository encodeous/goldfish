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
        foreach (var analyzer in _analyzers)
        {
            score += analyzer.GetScore(_state) * analyzer.Weighting;
        }

        return score;
    }
}