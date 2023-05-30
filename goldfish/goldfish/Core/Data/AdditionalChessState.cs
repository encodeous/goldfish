namespace goldfish.Core.Data;

public struct AdditionalChessState
{
    public ushort EnPassantPawns;
    public byte CastleState;

    /// <summary>
    /// Marks a pawn as possible to capture via En Passant
    /// </summary>
    /// <param name="col"></param>
    /// <param name="white"></param>
    public void MarkEnPassant(int col, bool white)
    {
        if (white)
        {
            EnPassantPawns = (ushort)(EnPassantPawns | (1 << (col + 8)));
        }
        else
        {
            EnPassantPawns = (ushort)(EnPassantPawns | (1 << col));
        }
    }

    /// <summary>
    /// Checks if a pawn in the column is able to be captured En Passant
    /// </summary>
    /// <returns>true if possible</returns>
    public bool CheckEnPassant(int col, bool white)
    {
        if (white)
        {
            return ((EnPassantPawns >> (col + 8)) & 1) == 1;
        }
        return ((EnPassantPawns >> col) & 1) == 1;
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
        return (CastleState & (int)type) == 1;
    }
}