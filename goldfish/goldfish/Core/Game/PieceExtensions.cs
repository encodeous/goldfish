using System.Runtime.CompilerServices;
using goldfish.Core.Data;
using goldfish.Core.Game.Rules.Pieces;

namespace goldfish.Core.Game;

/// <summary>
/// Each chess piece is represented as a 4-bit number, white pieces are from 0-5 and black pieces are from 6-11. Spaces are represented as 15
/// </summary>
public static class PieceExtensions
{
    /// <summary>
    /// Checks if a piece is white
    /// </summary>
    /// <param name="piece"></param>
    /// <returns></returns>
    public static bool IsWhite(this byte piece)
    {
        if (piece == (int)PieceType.Space) return false;
        return piece > 5;
    }
    /// <summary>
    /// Checks if a piece is black
    /// </summary>
    /// <param name="piece"></param>
    /// <returns></returns>
    public static bool IsBlack(this byte piece)
    {
        if (piece == (int)PieceType.Space) return false;
        return piece <= 5;
    }

    /// <summary>
    /// Gets the side that a piece is on
    /// </summary>
    /// <param name="piece"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Side GetSide(this byte piece)
    {
        if (piece == (int)PieceType.Space) return Side.None;
        return IsWhite(piece) ? Side.White : Side.Black;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsSide(this byte piece, Side side)
    {
        if (piece == (int)PieceType.Space) return side == Side.None;
        return IsWhite(piece) ? side == Side.White : side == Side.Black;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PieceType GetPieceType(this byte piece)
    {
        if (piece == (int)PieceType.Space) return PieceType.Space;
        return (PieceType)(piece % 6);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsPieceType(this byte piece, PieceType type)
    {
        if (piece == (int)PieceType.Space) return type == PieceType.Space;
        return (piece % 6) == (int)type;
    }

    /// <summary>
    /// Gets the piece as a 4-bit representation
    /// </summary>
    /// <param name="type"></param>
    /// <param name="side"></param>
    /// <returns></returns>
    public static byte ToPiece(this PieceType type, Side side)
    {
        if (type == PieceType.Space) return (byte)PieceType.Space;
        return (byte)((int)type + (side == Side.White ? 6 : 0));
    }

    /// <summary>
    /// Gets the opposing side to the game
    /// </summary>
    /// <param name="side"></param>
    /// <returns></returns>
    public static Side GetOpposing(this Side side)
    {
        if (side == Side.None) return side;
        if (side == Side.White)
            return Side.Black;
        return Side.White;
    }

    /// <summary>
    /// Check whether the point is within the chess board
    /// </summary>
    /// <param name="coord"></param>
    /// <returns></returns>
    public static bool IsWithinBoard(this (int, int) coord)
    {
        return 0 <= coord.Item1 && coord.Item1 < 8
                                && 0 <= coord.Item2 && coord.Item2 < 8;
    }

    /// <summary>
    /// Determines the castle type of the rook based on its initial position
    /// </summary>
    /// <param name="coord"></param>
    /// <returns></returns>
    public static CastleType? GetRookType(this (int, int) coord)
    {
        var (r, c) = coord;
        if ((r is not (0 or 7)) || (c is not (0 or 7))) return default;
        if (r == 0 && c == 0)
            return CastleType.WhiteQs;

        if (r == 0 && c == 7)
            return CastleType.WhiteKs;

        if (r == 7 && c == 0)
            return CastleType.BlackQs;

        return CastleType.BlackKs;

    }

    /// <summary>
    /// Gets the initial position of the rooks based on the type of castle
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static (int, int) GetCastleRookPos(this CastleType type)
    {
        return type switch
        {
            CastleType.BlackKs => (7, 7),
            CastleType.BlackQs => (7, 0),
            CastleType.WhiteKs => (0, 7),
            CastleType.WhiteQs => (0, 0),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    private static readonly Pawn _pawn;
    private static readonly Rook _rook;
    private static readonly Knight _knight;
    private static readonly Bishop _bishop;
    private static readonly Queen _queen;
    private static readonly King _king;

    public static IPieceLogic? GetLogic(this byte piece)
    {
        return (piece.GetPieceType() switch
        {
            PieceType.Pawn => _pawn,
            PieceType.Rook => _rook,
            PieceType.Knight => _knight,
            PieceType.Bishop => _bishop,
            PieceType.Queen => _queen,
            PieceType.King => _king,
            PieceType.Space => default,
            _ => throw new ArgumentOutOfRangeException()
        });
    }
    public static int GetLogicAttacks(this byte piece, in ChessState state, int r, int c, Span<(int, int)> attacks)
    {
        if (piece.GetPieceType() == PieceType.Pawn)
            return _pawn.GetAttacks(state, r, c, attacks);
        else if (piece.GetPieceType() == PieceType.Rook)
            return _rook.GetAttacks(state, r, c, attacks);
        else if (piece.GetPieceType() == PieceType.Knight)
            return _knight.GetAttacks(state, r, c, attacks);
        else if (piece.GetPieceType() == PieceType.Bishop)
            return _bishop.GetAttacks(state, r, c, attacks);
        else if (piece.GetPieceType() == PieceType.Queen)
            return _queen.GetAttacks(state, r, c, attacks);
        else if (piece.GetPieceType() == PieceType.King)
            return _king.GetAttacks(state, r, c, attacks);
        else throw new ArgumentOutOfRangeException();
    }
    public static int GetLogicMoves(this byte piece, in ChessState state, int r, int c, Span<ChessMove> moves, bool autoPromotion)
    {
        if (piece.GetPieceType() == PieceType.Pawn)
            return _pawn.GetMoves(state, r, c, moves, autoPromotion);
        if (piece.GetPieceType() == PieceType.Rook)
            return _rook.GetMoves(state, r, c, moves, autoPromotion);
        if (piece.GetPieceType() == PieceType.Knight)
            return _knight.GetMoves(state, r, c, moves, autoPromotion);
        if (piece.GetPieceType() == PieceType.Bishop)
            return _bishop.GetMoves(state, r, c, moves, autoPromotion);
        if (piece.GetPieceType() == PieceType.Queen)
            return _queen.GetMoves(state, r, c, moves, autoPromotion);
        if (piece.GetPieceType() == PieceType.King)
            return _king.GetMoves(state, r, c, moves, autoPromotion);
        return 0;
    } 
}