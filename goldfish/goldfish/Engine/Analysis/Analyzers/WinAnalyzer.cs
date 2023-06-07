using goldfish.Core.Data;
using goldfish.Core.Game;

namespace goldfish.Engine.Analysis.Analyzers;

public class WinAnalyzer : IGameAnalyzer
{
    public double Weighting => 1;
    public double GetScore(in ChessState state)
    {
        var win = state.GetGameState();
        if (win.HasValue)
        {
            if (win == Side.White) return double.PositiveInfinity;
            if (win == Side.Black) return double.NegativeInfinity;
            if (win == Side.None) return double.NaN;
        }
        return 0;
    }
}