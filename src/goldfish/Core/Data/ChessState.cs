using System.Runtime.CompilerServices;
using goldfish.Core.Data.Optimization;
using goldfish.Core.Game;

namespace goldfish.Core.Data;

/// <summary>
/// The current chess state, with white at the top and black at the bottom
/// </summary>
public unsafe struct ChessState
{
    /// <summary>
    /// Compressed chess state, 8 rows, 4 cols.
    /// Even numbered columns occupy the upper 4 bits, while odd numbered columns occupy the lower 4 bits.
    /// </summary>
    private fixed byte _pieces[8 * 4];

    private ushort KingPos;
    /// <summary>
    /// Additional chess state to store information such as En Passant and Castling
    /// </summary>
    public AdditionalChessState Additional;

    public Side _toMove;
    public int Pieces;

    public Side ToMove
    {
        get => _toMove;
        set
        {
            Additional.Hash ^= (_toMove == Side.White ? Tst.WhiteToMove : 0);
            _toMove = value;
            Additional.Hash ^= (value == Side.White ? Tst.WhiteToMove : 0);
        }
    }

    /// <summary>
    /// Gets the 4-bit piece from the compressed data
    /// </summary>
    /// <param name="r">row, 0 based index</param>
    /// <param name="c">col, 0 based index</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly byte GetPiece(int r, int c)
    {
        return (byte)((_pieces[r * 4 + c / 2] >> ((c + 1) % 2 * 4)) & 0b1111);
    }

    /// <summary>
    /// Sets the 4-bit piece to the compressed data
    /// </summary>
    /// <param name="r">row, 0 based index</param>
    /// <param name="c">col, 0 based index</param>
    /// <param name="piece">The 4-bit representation of the piece</param>
    /// <returns></returns>
    public void SetPiece(int r, int c, byte piece)
    {
        var dat = _pieces[r * 4 + c / 2];
        Additional.Hash ^= Tst.ZobristCache[r * 4 + c / 2, dat];
        if (piece.IsPieceType(PieceType.Space)) Pieces--;
        byte nDat;
        if (piece.IsPieceType(PieceType.King))
        {
            if (piece.IsSide(Side.Black))
            {
                KingPos = (ushort)(KingPos & ~0b111111 | r | (c << 3));
            }
            else
            {
                KingPos = (ushort)(KingPos & 0b111111 | (r << 6) | (c << 9));
            }
        }
        if (c % 2 == 0)
        {
            // upper bits
            nDat = _pieces[r * 4 + c / 2] = (byte)(dat & (0b1111) | (piece << 4));
            if (((byte)((dat & 0b11110000) >> 4)).IsPieceType(PieceType.Space)) Pieces++;
        }
        else
        {
            // lower bits
            nDat = _pieces[r * 4 + c / 2] = (byte)(dat & (0b11110000) | piece);
            if (((byte)(dat & 0b1111)).IsPieceType(PieceType.Space)) Pieces++;
        }
        Additional.Hash ^= Tst.ZobristCache[r * 4 + c / 2, nDat];
    }
    private void _SetPieceNoHash(int r, int c, byte piece)
    {
        var dat = _pieces[r * 4 + c / 2];
        //if (dat.IsPieceType(PieceType.Space)) Pieces++;
        if (piece.IsPieceType(PieceType.Space)) Pieces--;
        if (piece.IsPieceType(PieceType.King))
        {
            if (piece.IsSide(Side.Black))
            {
                KingPos = (ushort)(KingPos & ~0b111111 | r | (c << 3));
            }
            else
            {
                KingPos = (ushort)(KingPos & 0b111111 | (r << 6) | (c << 9));
            }
        }
        if (c % 2 == 0)
        {
            // upper bits
            _pieces[r * 4 + c / 2] = (byte)(dat & (0b1111) | (piece << 4));
            if (((byte)((dat & 0b11110000) >> 4)).IsPieceType(PieceType.Space)) Pieces++;
        }
        else
        {
            // lower bits
            _pieces[r * 4 + c / 2] = (byte)(dat & (0b11110000) | piece);
            if (((byte)(dat & 0b1111)).IsPieceType(PieceType.Space)) Pieces++;
        }
    }

    private static readonly PieceType[] DefaultPieces = {
        PieceType.Rook,
        PieceType.Knight,
        PieceType.Bishop,
        PieceType.Queen,
        PieceType.King,
        PieceType.Bishop,
        PieceType.Knight,
        PieceType.Rook,
    };

    private static ChessState _state;
    
    /// <summary>
    /// Initializes a regular chess board.
    /// </summary>
    /// <returns></returns>
    public static ChessState DefaultState()
    {
        if (_state._pieces[0] != 0) return _state;
        var cur = new ChessState();
        for (var i = 0; i < 32; i++)
        {
            cur._pieces[i] = (byte)PieceType.Space | (byte)PieceType.Space << 4;
        }

        for (var i = 0; i < 8; i++)
        {
            cur._SetPieceNoHash(0, i, DefaultPieces[i].ToPiece(Side.White));
            cur._SetPieceNoHash(7, i, DefaultPieces[i].ToPiece(Side.Black));
            cur._SetPieceNoHash(1, i, PieceType.Pawn.ToPiece(Side.White));
            cur._SetPieceNoHash(6, i, PieceType.Pawn.ToPiece(Side.Black));
        }

        cur.ToMove = Side.White;
        ulong hash = 0;
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                hash ^= Tst.ZobristCache[i * 4 + j, cur._pieces[i * 4 + j]];
            }
        }

        hash ^= Tst.EnPassant[cur.Additional.EnPassant];
        hash ^= Tst.CastleState[cur.Additional.CastleState];
        cur.Additional.Hash = hash;
        _state = cur;
        return cur;
    }
    
    public static ChessState EmptyState()
    {
        if (_state._pieces[0] != 0) return _state;
        var cur = new ChessState();
        for (var i = 0; i < 32; i++)
        {
            cur._pieces[i] = (byte)PieceType.Space | (byte)PieceType.Space << 4;
        }

        cur.ToMove = Side.White;
        ulong hash = 0;
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                hash ^= Tst.ZobristCache[i * 4 + j, cur._pieces[i * 4 + j]];
            }
        }

        hash ^= Tst.EnPassant[cur.Additional.EnPassant];
        hash ^= Tst.CastleState[cur.Additional.CastleState];
        cur.Additional.Hash = hash;
        _state = cur;
        return cur;
    }

    /// <summary>
    /// Locates the coordinates of the specified king
    /// </summary>
    /// <param name="side"></param>
    public readonly (sbyte, sbyte) GetKing(Side side)
    {
        if (side == Side.Black)
        {
            return ((sbyte, sbyte))(KingPos & 0b111, (KingPos >> 3) & 0b111);
        }

        return ((sbyte, sbyte))((KingPos >> 6) & 0b111, (KingPos >> 9) & 0b111);
    }
}