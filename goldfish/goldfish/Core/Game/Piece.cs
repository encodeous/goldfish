using goldfish.Core.Data;

namespace goldfish.Core.Game;

/// <summary>
/// Each chess piece is represented as a 4-bit number, white pieces are from 0-5 and black pieces are from 6-11. Spaces are represented as 15
/// </summary>
public static class Piece
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
        return IsWhite(piece) ? Side.White : Side.Black;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="piece"></param>
    /// <returns></returns>
    public static PieceType GetPieceType(byte piece)
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
    public static byte GetPiece(this PieceType type, Side side)
    {
        if (type == PieceType.Space) return (byte)PieceType.Space;
        return (byte)((int)type + (side == Side.White ? 6 : 0));
    }
}