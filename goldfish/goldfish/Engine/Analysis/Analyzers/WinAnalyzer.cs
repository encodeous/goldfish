using goldfish.Core.Data;
using goldfish.Core.Game;

namespace goldfish.Engine.Analysis.Analyzers;

public class WinAnalyzer : IGameAnalyzer
{
    public double Weighting => double.PositiveInfinity;
    public double GetScore(in ChessState state)
    {
        var win = state.GetWinner();
        if (win == Side.White) return 1;
        else if (win == Side.Black) return -1;
        return 0;
    }
}