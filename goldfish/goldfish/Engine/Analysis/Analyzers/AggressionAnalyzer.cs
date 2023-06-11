using goldfish.Core.Data;
using goldfish.Core.Game;

namespace goldfish.Engine.Analysis.Analyzers;

public class AggressionAnalyzer : IGameAnalyzer
{
    public double Weighting => 5;
    public double GetScore(in ChessState state, GameStateAnalyzer analyzer)
    {
        double ScoreSide(in ChessState nState, Side side)
        {
            double score = 0;
            for (var i = 0; i < 8; i++)
            for (var j = 0; j < 8; j++)
            {
                var piece = nState.GetPiece(i, j);
                if (piece.GetSide() == side)
                {
                    score += 8 - Utils.DistFromPiece((i, j), nState.GetKing(side.GetOpposing()));
                }
            }

            return score;
        }
        return ScoreSide(in state, Side.White) - ScoreSide(in state, Side.Black);
    }
}