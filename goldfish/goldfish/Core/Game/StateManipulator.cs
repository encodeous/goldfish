using goldfish.Core.Data;
using goldfish.Core.Data.Optimization;
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
    /// <param name="cache"></param>
    /// <returns></returns>
    public static bool[,] GetAttackMatrix(this in ChessState state, Side side, StateEvaluationCache? cache = null)
    {
        if (cache is not null && cache.AttackCache[(int)side] is not null) return cache.AttackCache[(int)side];
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

        if (cache is not null) cache.AttackCache[(int)side] = atk;
        return atk;
    }

    /// <summary>
    /// Determines whether a side has won
    /// </summary>
    /// <returns>returns None if it is a draw and null if there is no Checkmate or Stalemate</returns>
    public static Side? GetGameState(this in ChessState state, StateEvaluationCache? cache = null)
    {
        if (state.ToMove == Side.Black)
        {
            for (var i = 0; i < 8; i++)
            for (var j = 0; j < 8; j++)
            {
                var piece = state.GetPiece(i, j);
                if (piece.GetSide() != Side.Black || piece.GetLogic() is null) continue;
                if (state.GetValidMovesForSquare(i, j, cache).Any())
                {
                    return null;
                }
            }
            return state.IsChecked(Side.Black, cache) ? Side.White : Side.None;
        }
        if (state.ToMove == Side.White)
        {
            for (var i = 0; i < 8; i++)
            for (var j = 0; j < 8; j++)
            {
                var piece = state.GetPiece(i, j);
                if (piece.GetSide() != Side.White || piece.GetLogic() is null) continue;
                if (state.GetValidMovesForSquare(i, j, cache).Any())
                {
                    return null;
                }
            }
            return state.IsChecked(Side.White, cache) ? Side.Black : Side.None;
        }

        return Side.None;
    }

    /// <summary>
    /// Determines whether the specified king is being checked
    /// </summary>
    /// <param name="state"></param>
    /// <param name="side"></param>
    /// <param name="cache"></param>
    /// <returns></returns>
    public static bool IsChecked(this in ChessState state, Side side, StateEvaluationCache? cache = null)
    {
        if (cache is not null && cache.Checked != Side.None) return side == cache.Checked;
        var king = state.GetKing(side);
        var mtx = GetAttackMatrix(state, side.GetOpposing(), cache);
        var res = mtx[king.Item1, king.Item2];
        if (cache is not null && res) cache.Checked = side;
        return res;
    }

    /// <summary>
    /// Gets all of the valid moves of a piece
    /// </summary>
    /// <param name="state"></param>
    /// <param name="r"></param>
    /// <param name="c"></param>
    /// <param name="cache"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static IEnumerable<ChessMove> GetValidMovesForSquare(this ChessState state, int r, int c, StateEvaluationCache? cache = null)
    {
        var piece = state.GetPiece(r, c);
        var type = piece.GetPieceType();
        if(type == PieceType.Space) return Enumerable.Empty<ChessMove>();
        var isChecked = state.IsChecked(piece.GetSide(), cache);
        var logic = piece.GetLogic();
        var moves = logic.GetMoves(state, r, c);

        IEnumerable<ChessMove> ValidMoves()
        {
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
        if (cache != null)
        {
            if (cache.CachedMoves[r, c] is not null)
            {
                return cache.CachedMoves[r, c];
            }
            return cache.CachedMoves[r, c] = ValidMoves().ToArray();
        }

        return ValidMoves();
    }

    public static void FinalizeTurn(ref this ChessState state)
    {
        state.Additional.EnPassant = 0; // reset en passant
        state.ToMove = state.ToMove.GetOpposing();
    }
    
    public static void FinalizeTurnWithEnPassant(ref this ChessState state, int col, Side side)
    {
        FinalizeTurn(ref state);
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