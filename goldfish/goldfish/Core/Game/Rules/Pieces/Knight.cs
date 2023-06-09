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
            if (!PieceUtils.MovePiece(state, r, c, nr, nc, out var cMove))
            {
                if(cMove.HasValue) 
                    yield return cMove.Value;
                continue;
            }

            yield return cMove.Value;
        }
    }

    public void GetAttacks(ChessState state, int r, int c, List<(int, int)> attacks)
    {
        foreach (var move in _moves)
        {
            var (ox, oy) = move;
            var nr = r + ox;
            var nc = c + oy;
            if (!(nr, nc).IsWithinBoard()) continue;
            attacks.Add((nr, nc));
        }
    }
}