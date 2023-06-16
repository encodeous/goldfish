using goldfish.Core.Data;
using goldfish.Core.Game;

namespace goldfish.Engine.Analysis.Analyzers;

public class MaterialAnalyzer : IGameAnalyzer
{
    public double Weighting => 1000;

    internal static int ScorePiece(PieceType type)
    {
        return type switch
        {
            PieceType.Pawn => 1,
            PieceType.Rook => 5,
            PieceType.Knight => 3,
            PieceType.Bishop => 3,
            PieceType.Queen => 9,
            PieceType.King => 0,
            PieceType.Space => 0,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
    public double GetScore(in ChessState state)
    {
        int score = 0;
        for (var i = 0; i < 8; i++)
        for (var j = 0; j < 8; j++)
        {
            var piece = state.GetPiece(i, j);
            score += ScorePiece(piece.GetPieceType()) * (piece.GetSide() == Side.Black ? -1 : 1);
        }
        return score;
    }
}