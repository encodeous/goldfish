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

    public int CountMoves(in ChessState state, int r, int c)
    {
        int cnt = new Rook().CountMoves(state, r, c);
        cnt += new Bishop().CountMoves(state, r, c);
        return cnt;
    }

    public int GetAttacks(ChessState state, int r, int c, Span<(int, int)> attacks)
    {
        int cnt = new Rook().GetAttacks(state, r, c, attacks);
        cnt += new Bishop().GetAttacks(state, r, c, attacks[cnt..]);
        return cnt;
    }

    public int CountAttacks(in ChessState state, int r, int c)
    {
        int cnt = new Rook().CountAttacks(state, r, c);
        cnt += new Bishop().CountAttacks(state, r, c);
        return cnt;
    }
}