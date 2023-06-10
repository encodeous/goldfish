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
    /// <returns></returns>
    public IEnumerable<ChessMove> GetMoves(ChessState state, int r, int c);
    /// <summary>
    /// Counts the valid moves and attacks that the piece can perform
    /// </summary>
    /// <param name="state"></param>
    /// <param name="r"></param>
    /// <param name="c"></param>
    /// <returns></returns>
    public int CountMoves(in ChessState state, int r, int c);

    /// <summary>
    /// Gets all the squares that the piece threatens
    /// </summary>
    /// <param name="state"></param>
    /// <param name="r"></param>
    /// <param name="c"></param>
    /// <param name="attacks"></param>
    /// <returns></returns>
    public int GetAttacks(ChessState state, int r, int c, Span<(int, int)> attacks);
    
    /// <summary>
    /// Counts all the squares that the piece threatens
    /// </summary>
    /// <param name="state"></param>
    /// <param name="r"></param>
    /// <param name="c"></param>
    /// <returns></returns>
    public int CountAttacks(in ChessState state, int r, int c);
}