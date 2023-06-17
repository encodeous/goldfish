using goldfish.Core.Data;
using goldfish.Core.Game;

namespace goldfish.Engine.Analysis.Analyzers;

public class ControlAnalyzer : IGameAnalyzer
{
    public double Weighting => 5;
    public double GetScore(in ChessState state)
    {
        double ScoreSide(in ChessState nState, Side side)
        {
            var cSquares = nState.GetAttackMatrix(side);
            double score = 0;
            for (var i = 0; i < 8; i++)
            for (var j = 0; j < 8; j++)
            {
                if (cSquares[i, j])
                {
                    var piece = nState.GetPiece(i, j);
                    double worth = MaterialAnalyzer.ScorePiece(piece.GetPieceType());
                    if (piece.GetSide() != side) worth *= 1.5;
                    worth *= Math.Pow(8 - Utils.DistFromCenter((i, j)), 0.2); // encourage pieces to be in the center
                    if (worth == 0) worth = 0.5;
                    score += worth;
                }
            }

            return score;
        }
        

        return ScoreSide(in state, Side.White) - ScoreSide(in state, Side.Black);
    }
}