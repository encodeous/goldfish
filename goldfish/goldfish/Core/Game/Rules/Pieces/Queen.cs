using goldfish.Core.Data;

namespace goldfish.Core.Game.Rules.Pieces;

public struct Queen : IPieceLogic
{
    public IEnumerable<ChessMove> GetMoves(ChessState state, int r, int c)
    {
        foreach (var move in new Rook().GetMoves(state, r, c))
        {
            yield return move;
        }
        foreach (var move in new Bishop().GetMoves(state, r, c))
        {
            yield return move;
        }
    }

    public void GetAttacks(ChessState state, int r, int c, List<(int, int)> attacks)
    {
        new Rook().GetAttacks(state, r, c, attacks);
        new Bishop().GetAttacks(state, r, c, attacks);
    }
}