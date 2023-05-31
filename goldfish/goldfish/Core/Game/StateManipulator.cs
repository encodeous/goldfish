using goldfish.Core.Data;

namespace goldfish.Core.Game;

/// <summary>
/// A class that manipulates the ChessState
/// </summary>
public static class StateManipulator
{
    public static IEnumerable<ChessMove> GetValidMovesForSquare(this ref ChessState state, int r, int c)
    {
        var piece = state.GetPiece(r, c);
        throw new NotImplementedException();
    }
}