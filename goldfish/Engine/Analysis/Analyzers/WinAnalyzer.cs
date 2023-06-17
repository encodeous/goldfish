using goldfish.Core.Data;
using goldfish.Core.Game;

namespace goldfish.Engine.Analysis.Analyzers;

public class WinAnalyzer : IGameAnalyzer
{
    public double Weighting => 1;
    public const int CheckmateWeighting = 1000000;
    public double GetScore(in ChessState state)
    {
        var win = state.GetGameState();
        if (win is not null)
        {
            if (win == Side.White) return 2 * CheckmateWeighting;
            if (win == Side.Black) return -2 * CheckmateWeighting;
            if (win == Side.None) return double.NaN;
        }
        return 0;
    }
}