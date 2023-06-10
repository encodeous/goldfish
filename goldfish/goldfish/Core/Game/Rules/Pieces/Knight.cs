using goldfish.Core.Data;

namespace goldfish.Core.Game.Rules.Pieces;

public struct Knight : IPieceLogic
{
    private static readonly (int, int)[] _moves =
    {
        (2, 1),
        (1, 2),
        (-1, 2),
        (-2, 1),
        
        (2, -1),
        (1, -2),
        (-1, -2),
        (-2, -1)
    };
    public IEnumerable<ChessMove> GetMoves(ChessState state, int r, int c)
    {
        foreach (var move in _moves)
        {
            var (ox, oy) = move;
            var nr = r + ox;
            var nc = c + oy;
            if (!RuleUtils.MovePiece(state, r, c, nr, nc, out var cMove))
            {
                if(cMove.HasValue) 
                    yield return cMove.Value;
                continue;
            }

            yield return cMove.Value;
        }
    }

    public int CountMoves(in ChessState state, int r, int c)
    {
        return RuleUtils.CountMoves(state, r, c, _moves);
    }

    public int GetAttacks(ChessState state, int r, int c, Span<(int, int)> attacks)
    {
        int cnt = 0;
        foreach (var move in _moves)
        {
            var (ox, oy) = move;
            var nr = r + ox;
            var nc = c + oy;
            if (!(nr, nc).IsWithinBoard()) continue;
            attacks[cnt++] = (nr, nc);
        }

        return cnt;
    }

    public int CountAttacks(in ChessState state, int r, int c)
    {
        int cnt = 0;
        foreach (var move in _moves)
        {
            var (ox, oy) = move;
            var nr = r + ox;
            var nc = c + oy;
            if (!(nr, nc).IsWithinBoard()) continue;
            cnt++;
        }

        return cnt;
    }
}