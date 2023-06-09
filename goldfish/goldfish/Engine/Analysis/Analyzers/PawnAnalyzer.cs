using goldfish.Core.Data;
using goldfish.Core.Game;

namespace goldfish.Engine.Analysis.Analyzers;

public class PawnAnalyzer : IGameAnalyzer
{
    public double Weighting => 1;
    public double GetScore(in ChessState state, GameStateAnalyzer analyzer)
    {
        double ScoreSide(in ChessState nState, Side side)
        {
            var cSquares = nState.GetAttackMatrix(side, analyzer.Cache);
            var aSquares = nState.GetAttackMatrix(side.GetOpposing(), analyzer.Cache);
            double score = 0;
            for (var i = 0; i < 8; i++)
            for (var j = 0; j < 8; j++)
            {
                var piece = nState.GetPiece(i, j);
                if (piece.GetSide() == side && piece.GetPieceType() == PieceType.Pawn)
                {
                    double worth = (8 - Utils.DistFromPromotion((i, j), side)) * 0.6 +
                                   (8 - Utils.DistFromCenter((i, j))) * 0.4;
                    if (cSquares[i, j]) worth *= 2;
                    if (aSquares[i, j]) worth /= 2;
                    score += worth;
                }
            }

            return score;
        }
        

        return ScoreSide(in state, Side.White) - ScoreSide(in state, Side.Black);
    }
}