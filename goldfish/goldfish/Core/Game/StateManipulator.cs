using goldfish.Core.Data;
using goldfish.Core.Game.Rules.Pieces;

namespace goldfish.Core.Game;

/// <summary>
/// A class that manipulates the ChessState and queries extensive information about it
/// </summary>
public static class StateManipulator
{
    /// <summary>
    /// Gets a matrix of squares attacked by a side
    /// </summary>
    /// <param name="state"></param>
    /// <param name="side"></param>
    /// <returns></returns>
    public static bool[,] GetAttackMatrix(this in ChessState state, Side side)
    {
        var atk = new bool[8, 8];
        for (var i = 0; i < 8; i++)
        for (var j = 0; j < 8; j++)
        {
            var piece = state.GetPiece(i, j);
            if (piece.GetSide() != side || piece.GetLogic() is null) continue;
            foreach (var pos in piece.GetLogic().GetAttacks(state, i, j))
            {
                atk[pos.Item1, pos.Item2] = true;
            }
        }

        return atk;
    }

    /// <summary>
    /// Determines whether a side has won, returns None if neither side has won
    /// </summary>
    /// <returns></returns>
    public static Side GetWinner(this in ChessState state)
    {
        if (state.IsChecked(Side.Black))
        {
            for (var i = 0; i < 8; i++)
            for (var j = 0; j < 8; j++)
            {
                var piece = state.GetPiece(i, j);
                if (piece.GetSide() != Side.Black || piece.GetLogic() is null) continue;
                if (state.GetValidMovesForSquare(i, j).Any())
                {
                    return Side.None;
                }
            }

            return Side.White;
        }
        if (state.IsChecked(Side.White))
        {
            for (var i = 0; i < 8; i++)
            for (var j = 0; j < 8; j++)
            {
                var piece = state.GetPiece(i, j);
                if (piece.GetSide() != Side.White || piece.GetLogic() is null) continue;
                if (state.GetValidMovesForSquare(i, j).Any())
                {
                    return Side.None;
                }
            }

            return Side.Black;
        }

        return Side.None;
    }

    /// <summary>
    /// Determines whether the specified king is being checked
    /// </summary>
    /// <param name="state"></param>
    /// <param name="side"></param>
    /// <returns></returns>
    public static bool IsChecked(this in ChessState state, Side side)
    {
        var king = state.GetKing(side);
        var mtx = GetAttackMatrix(state, side.GetOpposing());
        return mtx[king.Item1, king.Item2];
    }
    /// <summary>
    /// Gets all of the valid moves of a piece
    /// </summary>
    /// <param name="state"></param>
    /// <param name="r"></param>
    /// <param name="c"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static IEnumerable<ChessMove> GetValidMovesForSquare(this ChessState state, int r, int c)
    {
        var piece = state.GetPiece(r, c);
        var type = piece.GetPieceType();
        if(type == PieceType.Space) yield break;
        var isChecked = state.IsChecked(piece.GetSide());
        var logic = piece.GetLogic();
        var moves = logic.GetMoves(state, r, c);
        // check the moves
        foreach (var move in moves)
        {
            var newCheckStatus = move.NewState.IsChecked(piece.GetSide());
            if (isChecked)
            {
                // only permit blocking moves and do not allow castle out of check
                if (!newCheckStatus && !move.IsCastle)
                    yield return move;
            }
            else
            {
                if(!newCheckStatus) 
                    yield return move;
            }
        }
    }

    public static void FinalizeTurn(ref this ChessState state)
    {
        state.Additional.EnPassantPawns = 0; // reset en passant
    }
    
    public static void FinalizeTurnWithEnPassant(ref this ChessState state, int col, Side side)
    {
        state.Additional.EnPassantPawns = 0; // reset en passant
        state.Additional.MarkEnPassant(col, side);
    }

    public static void Move(ref this ChessState state, (int, int) oldP, (int, int) newP)
    {
        var piece = state.GetPiece(oldP.Item1, oldP.Item2);
        state.SetPiece(oldP.Item1, oldP.Item2, PieceType.Space.ToPiece(Side.None));
        state.SetPiece(newP.Item1, newP.Item2, piece);
    }

    public static void Promote(this ref ChessState state, (int, int) pawnPos, PromotionType choice)
    {
        state.SetPiece(pawnPos.Item1, pawnPos.Item2, ((PieceType)choice).ToPiece(state.GetPiece(pawnPos.Item1, pawnPos.Item2).GetSide()));
    }
}