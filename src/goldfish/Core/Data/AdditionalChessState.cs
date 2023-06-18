using goldfish.Core.Data.Optimization;

namespace goldfish.Core.Data;

public struct AdditionalChessState
{
    private byte _enPassant;
    private byte _castleState;
    public ulong Hash;

    public byte EnPassant
    {
        get => _enPassant;
        set
        {
            Hash ^= Tst.EnPassant[_enPassant];
            _enPassant = value;
            Hash ^= Tst.EnPassant[_enPassant];
        }
    }

    public byte CastleState
    {
        get => _castleState;
        set
        {
            Hash ^= Tst.CastleState[_castleState];
            _castleState = value;
            Hash ^= Tst.CastleState[_castleState];
        }
    }

    /// <summary>
    /// Marks a pawn as possible to capture via En Passant
    /// </summary>
    /// <param name="col"></param>
    /// <param name="side"></param>
    public void MarkEnPassant(int col, Side side)
    {
        Hash ^= Tst.EnPassant[_enPassant];
        _enPassant = (byte)(col << 2 | (int)side << 1 | 1);
        Hash ^= Tst.EnPassant[_enPassant];
    }

    /// <summary>
    /// Checks if a pawn in the column is able to be captured En Passant
    /// </summary>
    /// <returns>true if possible</returns>
    public bool CheckEnPassant(int col, Side side)
    {
        if ((_enPassant & 1) != 1) return false;
        var cSide = (Side)(_enPassant >> 1 & 1);
        if (cSide != side) return false;
        return _enPassant >> 2 == col;
    }

    /// <summary>
    /// Marks the type of castle so that it is blocked
    /// </summary>
    /// <param name="type"></param>
    public void MarkCastle(CastleType type)
    {
        Hash ^= Tst.CastleState[_castleState];
        _castleState |= (byte)type;
        Hash ^= Tst.CastleState[_castleState];
    }

    /// <summary>
    /// Checks if a type of castling is blocked
    /// </summary>
    /// <param name="type"></param>
    /// <returns>true if blocked</returns>
    public bool CheckCastle(CastleType type)
    {
        return (_castleState & (int)type) == (int)type;
    }
}