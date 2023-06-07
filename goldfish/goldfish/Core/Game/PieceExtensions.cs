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
    public static Side GetSide(this byte piece)
    {
        if (piece.GetPieceType() == PieceType.Space) return Side.None;
        return IsWhite(piece) ? Side.White : Side.Black;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="piece"></param>
    /// <returns></returns>
    public static PieceType GetPieceType(this byte piece)
    {
        if (piece == (int)PieceType.Space) return PieceType.Space;
        return (PieceType)(piece % 6);
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

        return CastleType.BlackQs;

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
    public static IPieceLogic? GetLogic(this byte piece)
    {
        return (piece.GetPieceType() switch
        {
            PieceType.Pawn => new Pawn(),
            PieceType.Rook => new Rook(),
            PieceType.Knight => new Knight(),
            PieceType.Bishop => new Bishop(),
            PieceType.Queen => new Queen(),
            PieceType.King => new King(),
            PieceType.Space => default,
            _ => throw new ArgumentOutOfRangeException()
        });
    }
}