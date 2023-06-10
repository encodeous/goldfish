using goldfish.Core.Data;
using goldfish.Core.Game;

namespace goldfish.Engine.Analysis.Analyzers;

public class WinAnalyzer : IGameAnalyzer
{
    public double Weighting => 1;
    public double GetScore(in ChessState state, GameStateAnalyzer analyzer)
    {
        var win = state.GetGameState(analyzer.Cache);
        if (win.HasValue)
        {
            if (win == Side.White) return 1000000000;
            if (win == Side.Black) return -1000000000;
            if (win == Side.None) return double.NaN;
        }
        return 0;
    }
}