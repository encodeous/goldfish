using goldfish.Core.Data;

namespace goldfish.Core.Game.Rules.Pieces;

public struct Rook : IPieceLogic
{
    public IEnumerable<ChessMove> GetMoves(ChessState state, int r, int c)
    {
        // mark the castle state
        if ((r, c).GetRookType() is not null)
        {
            state.Additional.MarkCastle((r, c).GetRookType()!.Value);
        }
        // up
        for (int i = r - 1; i >= 0; i--)
        {
            if (!PieceUtils.MovePiece(state, r, c, i, c, out var move))
            {
                if(move.HasValue) 
                    yield return move.Value;
                break;
            }
            yield return move.Value;
        }
        // down
        for (int i = r + 1; i < 8; i++)
        {
            if (!PieceUtils.MovePiece(state, r, c, i, c, out var move))
            {
                if(move.HasValue) 
                    yield return move.Value;
                break;
            }
            yield return move.Value;
        }
        // left
        for (int i = c - 1; i >= 0; i--)
        {
            if (!PieceUtils.MovePiece(state, r, c, r, i, out var move))
            {
                if(move.HasValue) 
                    yield return move.Value;
                break;
            }
            yield return move.Value;
        }
        // right
        for (int i = c + 1; i < 8; i++)
        {
            if (!PieceUtils.MovePiece(state, r, c, r, i, out var move))
            {
                if(move.HasValue) 
                    yield return move.Value;
                break;
            }
            yield return move.Value;
        }
    }

    public void GetAttacks(ChessState state, int r, int c, List<(int, int)> attacks)
    {
        // up
        for (int i = r - 1; i >= 0; i--)
        {
            if (!PieceUtils.IsEmptySquare(state, i, c))
            {
                if((i, c).IsWithinBoard())
                    attacks.Add((i, c));
                break;
            }
            attacks.Add((i, c));
        }
        // down
        for (int i = r + 1; i < 8; i++)
        {
            if (!PieceUtils.IsEmptySquare(state, i, c))
            {
                if((i, c).IsWithinBoard())
                    attacks.Add((i, c));
                break;
            }
            attacks.Add((i, c));
        }
        // left
        for (int i = c - 1; i >= 0; i--)
        {
            if (!PieceUtils.IsEmptySquare(state, r, i))
            {
                if((r, i).IsWithinBoard())
                    attacks.Add((r, i));
                break;
            }
            attacks.Add((r, i));
        }
        // right
        for (int i = c + 1; i < 8; i++)
        {
            if (!PieceUtils.IsEmptySquare(state, r, i))
            {
                if((r, i).IsWithinBoard())
                    attacks.Add((r, i));
                break;
            }
            attacks.Add((r, i));
        }
    }
}