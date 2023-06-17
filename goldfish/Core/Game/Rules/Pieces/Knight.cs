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
    public int GetMoves(in ChessState state, int r, int c, Span<ChessMove> moves, bool autoPromotion)
    {
        var cnt = 0;
        foreach (var move in _moves)
        {
            var (ox, oy) = move;
            var nr = r + ox;
            var nc = c + oy;
            RuleUtils.MovePiece(state, r, c, nr, nc, out var cMove);
            if (cMove is not null)
                moves[cnt++] = cMove.Value;
        }

        return cnt;
    }

    public int GetAttacks(in ChessState state, int r, int c, Span<(int, int)> attacks)
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
        return RuleUtils.CountAttacks(r, c, _moves);
    }
}