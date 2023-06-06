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
    public fixed byte Pieces[8 * 4];

    /// <summary>
    /// Additional chess state to store information such as En Passant and Castling
    /// </summary>
    public AdditionalChessState Additional;

    /// <summary>
    /// Gets the 4-bit piece from the compressed data
    /// </summary>
    /// <param name="r">row, 0 based index</param>
    /// <param name="c">col, 0 based index</param>
    /// <returns></returns>
    public readonly byte GetPiece(int r, int c)
    {
        return (byte)((Pieces[r * 4 + c / 2] >> ((c + 1) % 2 * 4)) & 0b1111);
    }

    /// <summary>
    /// Sets the 4-bit piece to the compressed data
    /// </summary>
    /// <param name="r">row, 0 based index</param>
    /// <param name="c">col, 0 based index</param>
    /// <param name="piece">The 4-bit representation of the pieve</param>
    /// <returns></returns>
    public void SetPiece(int r, int c, byte piece)
    {
        if (c % 2 == 0)
        {
            // upper bits
            Pieces[r * 4 + c / 2] = (byte)(Pieces[r * 4 + c / 2] & (0b1111) | (piece << 4));
        }
        else
        {
            // lower bits
            Pieces[r * 4 + c / 2] = (byte)(Pieces[r * 4 + c / 2] & (0b11110000) | piece);
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
        if (_state.Pieces[0] != 0) return _state;
        var cur = new ChessState();
        for (var i = 0; i < 32; i++)
        {
            cur.Pieces[i] = (byte)PieceType.Space | (byte)PieceType.Space << 4;
        }

        for (var i = 0; i < 8; i++)
        {
            cur.SetPiece(0, i, DefaultPieces[i].GetPiece(Side.White));
            cur.SetPiece(7, i, DefaultPieces[i].GetPiece(Side.Black));
            cur.SetPiece(1, i, PieceType.Pawn.GetPiece(Side.White));
            cur.SetPiece(6, i, PieceType.Pawn.GetPiece(Side.Black));
        }

        _state = cur;
        return cur;
    }

    /// <summary>
    /// Locates the coordinates of the specified king
    /// </summary>
    /// <param name="side"></param>
    /// <returns>(-1, -1) if not found</returns>
    public readonly (int, int) GetKing(Side side)
    {
        for (var i = 0; i < 8; i++)
        for (var j = 0; j < 8; j++)
        {
            var p = GetPiece(i, j);
            if (p.GetPieceType() == PieceType.King && p.GetSide() == side)
                return (i, j);
        }

        return (-1, -1);
    }
}