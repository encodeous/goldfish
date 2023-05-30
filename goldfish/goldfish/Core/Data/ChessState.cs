namespace goldfish.Core.Data;

/// <summary>
/// The current chess state, with black at the top and white at the bottom
/// </summary>
public unsafe struct ChessState
{
    /// <summary>
    /// Compressed chess state, 8 rows, 4 cols.
    /// Even numbered columns occupy the upper 4 bits, while odd numbered columns occupy the lower 4 bits.
    /// </summary>
    public fixed byte Pieces[8 * 4];

    /// <summary>
    /// Gets the 4-bit piece from the compressed data
    /// </summary>
    /// <param name="r">row, 0 based index</param>
    /// <param name="c">col, 0 based index</param>
    /// <returns></returns>
    public byte GetPiece(int r, int c)
    {
        return (byte)(Pieces[r * 4 + c / 2] >> (c % 2 * 4));
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
        Pieces[r * 4 + c / 2] = (byte)(piece << (c % 2 * 4));
    }
}