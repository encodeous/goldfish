using goldfish.Core.Data;
using goldfish.Core.Game;

namespace goldfish.Engine.Analysis.Analyzers;

public class PiecePositionAnalyzer : IGameAnalyzer
{
	public static int Analyze (byte piece, int r, int c, bool isEndGame)
	{
		if (piece.IsSide(Side.White))
		{
			r = 7 - r;
		}

		return piece.GetPieceType() switch
		{
			PieceType.Pawn => pawns[r * 8 + c],
			PieceType.Rook => rooks[r * 8 + c],
			PieceType.Knight => knights[r * 8 + c],
			PieceType.Bishop => bishops[r * 8 + c],
			PieceType.Queen => queens[r * 8 + c],
			PieceType.King => isEndGame ? kingEnd[r * 8 + c] : kingMiddle[r * 8 + c],
			PieceType.Space => 0,
			_ => throw new ArgumentOutOfRangeException()
		};
	}

		public static readonly int[] pawns = {
			0,  0,  0,  0,  0,  0,  0,  0,
			50, 50, 50, 50, 50, 50, 50, 50,
			10, 10, 20, 30, 30, 20, 10, 10,
			5,  5, 10, 25, 25, 10,  5,  5,
			0,  0,  0, 20, 20,  0,  0,  0,
			5, -5,-10,  0,  0,-10, -5,  5,
			5, 10, 10,-20,-20, 10, 10,  5,
			0,  0,  0,  0,  0,  0,  0,  0
		};

		public static readonly int[] knights = {
			-50,-40,-30,-30,-30,-30,-40,-50,
			-40,-20,  0,  0,  0,  0,-20,-40,
			-30,  0, 10, 15, 15, 10,  0,-30,
			-30,  5, 15, 20, 20, 15,  5,-30,
			-30,  0, 15, 20, 20, 15,  0,-30,
			-30,  5, 10, 15, 15, 10,  5,-30,
			-40,-20,  0,  5,  5,  0,-20,-40,
			-50,-40,-30,-30,-30,-30,-40,-50,
		};

		public static readonly int[] bishops = {
			-20,-10,-10,-10,-10,-10,-10,-20,
			-10,  0,  0,  0,  0,  0,  0,-10,
			-10,  0,  5, 10, 10,  5,  0,-10,
			-10,  5,  5, 10, 10,  5,  5,-10,
			-10,  0, 10, 10, 10, 10,  0,-10,
			-10, 10, 10, 10, 10, 10, 10,-10,
			-10,  5,  0,  0,  0,  0,  5,-10,
			-20,-10,-10,-10,-10,-10,-10,-20,
		};

		public static readonly int[] rooks = {
			0,  0,  0,  0,  0,  0,  0,  0,
			5, 10, 10, 10, 10, 10, 10,  5,
			-5,  0,  0,  0,  0,  0,  0, -5,
			-5,  0,  0,  0,  0,  0,  0, -5,
			-5,  0,  0,  0,  0,  0,  0, -5,
			-5,  0,  0,  0,  0,  0,  0, -5,
			-5,  0,  0,  0,  0,  0,  0, -5,
			0,  0,  0,  5,  5,  0,  0,  0
		};

		public static readonly int[] queens = {
			-20,-10,-10, -5, -5,-10,-10,-20,
			-10,  0,  0,  0,  0,  0,  0,-10,
			-10,  0,  5,  5,  5,  5,  0,-10,
			-5,  0,  5,  5,  5,  5,  0, -5,
			0,  0,  5,  5,  5,  5,  0, -5,
			-10,  5,  5,  5,  5,  5,  0,-10,
			-10,  0,  5,  0,  0,  0,  0,-10,
			-20,-10,-10, -5, -5,-10,-10,-20
		};

		public static readonly int[] kingMiddle = {
			-30,-40,-40,-50,-50,-40,-40,-30,
			-30,-40,-40,-50,-50,-40,-40,-30,
			-30,-40,-40,-50,-50,-40,-40,-30,
			-30,-40,-40,-50,-50,-40,-40,-30,
			-20,-30,-30,-40,-40,-30,-30,-20,
			-10,-20,-20,-20,-20,-20,-20,-10,
			20, 20,  0,  0,  0,  0, 20, 20,
			20, 30, 10,  0,  0, 10, 30, 20
		};

		public static readonly int[] kingEnd = {
			-50,-40,-30,-20,-20,-30,-40,-50,
			-30,-20,-10,  0,  0,-10,-20,-30,
			-30,-10, 20, 30, 30, 20,-10,-30,
			-30,-10, 30, 40, 40, 30,-10,-30,
			-30,-10, 30, 40, 40, 30,-10,-30,
			-30,-10, 20, 30, 30, 20,-10,-30,
			-30,-30,  0,  0,  0,  0,-30,-30,
			-50,-30,-30,-30,-30,-30,-30,-50
		};

		public double Weighting => 15;
		public double GetScore(in ChessState state)
		{
			int pieceCount = 0;
			for (var i = 0; i < 8; i++)
			for (var j = 0; j < 8; j++)
			{
				if (!state.GetPiece(i, j).IsPieceType(PieceType.Space)) pieceCount++;
			}
			double ScoreSide(in ChessState nState, Side side)
			{
				double score = 0;
				for (var i = 0; i < 8; i++)
				for (var j = 0; j < 8; j++)
				{
					if(nState.GetPiece(i, j).IsSide(side))
						score += Analyze(nState.GetPiece(i, j), i, j, pieceCount <= 12);
				}

				return score;
			}
        

			return ScoreSide(in state, Side.White) - ScoreSide(in state, Side.Black);
		}
}