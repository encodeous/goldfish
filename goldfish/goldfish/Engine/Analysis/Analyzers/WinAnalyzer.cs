using goldfish.Core.Data;
using goldfish.Core.Game;

namespace goldfish.Engine.Analysis.Analyzers;

public class WinAnalyzer : IGameAnalyzer
{
    public double Weighting => 1;
    public double GetScore(in ChessState state)
    {
        var win = state.GetGameState();
        if (win is not null)
        {
            if (win == Side.White) return 1000000;
            if (win == Side.Black) return -1000000;
            if (win == Side.None) return double.NaN;
        }
        return 0;
    }
}