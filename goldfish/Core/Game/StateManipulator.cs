﻿using System.Buffers;
using goldfish.Core.Data;
using goldfish.Core.Data.Optimization;
using goldfish.Core.Game.Rules;
using goldfish.Core.Game.Rules.Pieces;

namespace goldfish.Core.Game;

/// <summary>
/// A class that manipulates the ChessState and queries extensive information about it
/// </summary>
public static class StateManipulator
{
    private static readonly ChessMove[] _moveDump = new ChessMove[40];
    /// <summary>
    /// Gets a matrix of squares attacked by a side
    /// </summary>
    /// <param name="state"></param>
    /// <param name="side"></param>
    /// <returns></returns>
    public static Grid8x8 GetAttackMatrix(this in ChessState state, Side side)
    {
        var cache = Tst.Get(state);
        if (side == Side.White)
        {
            if (cache.WhiteCache is not null) return cache.WhiteCache.Value;
        }
        else
        {
            if (cache.BlackCache is not null) return cache.BlackCache.Value;
        }
        var atk = new Grid8x8();
        Span<(int, int)> attackBuf = stackalloc (int, int)[32]; // assume any given piece will have less than 30 possible attacks, this is very generous
        for (var i = 0; i < 8; i++)
        for (var j = 0; j < 8; j++)
        {
            var piece = state.GetPiece(i, j);
            if (!piece.IsSide(side)) continue;
            int attacks = piece.GetLogicAttacks(state, i, j, attackBuf);
            for (int a = 0; a < attacks; a++)
            {
                var pos = attackBuf[a];
                atk[pos.Item1, pos.Item2] = true;
            }
        }
        ref var aCache = ref Tst.Get(state);
        if (side == Side.White)
        {
            aCache.WhiteCache = atk;
        }
        else
        {
            aCache.BlackCache = atk;
        }
        return atk;
    }

    /// <summary>
    /// Determines whether a side has won
    /// </summary>
    /// <returns>returns None if it is a draw and null if there is no Checkmate or Stalemate</returns>
    public static Side? GetGameState(this in ChessState state)
    {
        for (var i = 0; i < 8; i++)
        for (var j = 0; j < 8; j++)
        {
            var piece = state.GetPiece(i, j);
            if (!piece.IsSide(state.ToMove)) continue;
            if (state.GetValidMovesForSquare(i, j, _moveDump) != 0)
            {
                return null;
            }
        }
        return state.IsChecked(state.ToMove) ? state.ToMove.GetOpposing() : Side.None;
    }

    /// <summary>
    /// Determines whether the specified king is being checked
    /// </summary>
    /// <param name="state"></param>
    /// <param name="side"></param>
    /// <returns></returns>
    public static bool IsChecked(this in ChessState state, Side side)
    {
        ref var cache = ref Tst.Get(state);
        if (cache.Checked is not null) return side == cache.Checked;
        var king = state.GetKing(side);
        var mtx = GetAttackMatrix(state, side.GetOpposing());
        var res = mtx[king.Item1, king.Item2];
        cache = ref Tst.Get(state);
        if (res) cache.Checked = side;
        return res;
    }

    /// <summary>
    /// Gets all of the valid moves of a piece
    /// </summary>
    /// <param name="state"></param>
    /// <param name="r"></param>
    /// <param name="c"></param>
    /// <param name="moves"></param>
    /// <param name="autoPromotion">determines whether the promotion options are returned as moves</param>
    /// <returns>the number of moves for the square</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static int GetValidMovesForSquare(this ChessState state, int r, int c, Span<ChessMove> moves, bool autoPromotion = true)
    {
        var piece = state.GetPiece(r, c);
        var type = piece.GetPieceType();
        if(type == PieceType.Space) return 0;
        var moveCnt = piece.GetLogicMoves(state, r, c, moves, autoPromotion);
        return moveCnt;
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