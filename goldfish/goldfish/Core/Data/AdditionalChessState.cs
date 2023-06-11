namespace goldfish.Core.Data;

public struct AdditionalChessState
{
    public byte EnPassant;
    public byte CastleState;

    /// <summary>
    /// Marks a pawn as possible to capture via En Passant
    /// </summary>
    /// <param name="col"></param>
    /// <param name="side"></param>
    public void MarkEnPassant(int col, Side side)
    {
        EnPassant = (byte)(col << 2 | (int)side << 1 | 1);
    }

    /// <summary>
    /// Checks if a pawn in the column is able to be captured En Passant
    /// </summary>
    /// <returns>true if possible</returns>
    public bool CheckEnPassant(int col, Side side)
    {
        if ((EnPassant & 1) != 1) return false;
        var cSide = (Side)(EnPassant >> 1 & 1);
        if (cSide != side) return false;
        return EnPassant >> 2 == col;
    }

    /// <summary>
    /// Marks the type of castle so that it is blocked
    /// </summary>
    /// <param name="type"></param>
    public void MarkCastle(CastleType type)
    {
        CastleState |= (byte)type;
    }

    /// <summary>
    /// Checks if a type of castling is blocked
    /// </summary>
    /// <param name="type"></param>
    /// <returns>true if blocked</returns>
    public bool CheckCastle(CastleType type)
    {
        return (CastleState & (int)type) == (int)type;
    }
}