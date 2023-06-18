using goldfish.Core.Data;

namespace goldfish.Core.Game.Rules.Pieces;

public interface IPieceLogic
{
    /// <summary>
    /// Gets the valid moves and attacks that the piece can perform
    /// </summary>
    /// <param name="state"></param>
    /// <param name="r"></param>
    /// <param name="c"></param>
    /// <param name="moves"></param>
    /// <param name="autoPromotion"></param>
    /// <returns>the number of valid moves</returns>
    public int GetMoves(in ChessState state, int r, int c, Span<ChessMove> moves, bool autoPromotion);

    /// <summary>
    /// Gets all the squares that the piece threatens
    /// </summary>
    /// <param name="state"></param>
    /// <param name="r"></param>
    /// <param name="c"></param>
    /// <param name="attacks"></param>
    /// <returns></returns>
    public int GetAttacks(in ChessState state, int r, int c, Span<(int, int)> attacks);
    
    /// <summary>
    /// Counts all the squares that the piece threatens
    /// </summary>
    /// <param name="state"></param>
    /// <param name="r"></param>
    /// <param name="c"></param>
    /// <returns></returns>
    public int CountAttacks(in ChessState state, int r, int c);
}