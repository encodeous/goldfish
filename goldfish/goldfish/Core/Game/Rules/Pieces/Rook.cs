using goldfish.Core.Data;

namespace goldfish.Core.Game.Rules.Pieces;

public struct Rook : IPieceLogic
{
    public IEnumerable<ChessMove> GetMoves(ChessState state, int r, int c)
    {
        var piece = state.GetPiece(r, c);
        var side = piece.GetSide();
        // mark the castle state
        if ((r, c).GetRookType() is not null)
        {
            state.Additional.MarkCastle((r, c).GetRookType()!.Value);
        }

        bool MoveRook(int nr, int nc, out ChessMove? move)
        {
            move = default;
            if (!(nr, nc).IsWithinBoard()) return false;
            var p = state.GetPiece(nr, nc);
            if (p.GetPieceType() == PieceType.Space)
            {
                // free to move
                var ns = state;
                ns.FinalizeTurn();
                ns.Move((r, c), (nr, nc));
                move = new ChessMove()
                {
                    NewPos = (nr, nc),
                    NewState = ns,
                    OldPos = (r, c)
                };
                return true;
            }

            if (p.GetSide() == side) return false;
            {
                // only valid if its an opposing piece
                // capture
                var ns = state;
                ns.FinalizeTurn();
                ns.Move((r, c), (nr, nc));
                move = new ChessMove()
                {
                    NewPos = (nr, nc),
                    NewState = ns,
                    Taken = (nr, nc),
                    OldPos = (r, c)
                };
                return false;
            }

        }
        
        // up
        for (int i = r - 1; i >= 0; i--)
        {
            if (!MoveRook(i, c, out var move))
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
            if (!MoveRook(i, c, out var move))
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
            if (!MoveRook(r, i, out var move))
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
            if (!MoveRook(r, i, out var move))
            {
                if(move.HasValue) 
                    yield return move.Value;
                break;
            }
            yield return move.Value;
        }
    }

    public IEnumerable<(int, int)> GetAttacks(ChessState state, int r, int c)
    {
        bool MoveRook(int nr, int nc)
        {
            if (!(nr, nc).IsWithinBoard()) return false;
            var p = state.GetPiece(nr, nc);
            return p.GetPieceType() == PieceType.Space;
        }
        // up
        for (int i = r - 1; i >= 0; i--)
        {
            if (!MoveRook(i, c))
            {
                if((i, c).IsWithinBoard())
                    yield return (i, c);
                break;
            }
            yield return (i, c);
        }
        // down
        for (int i = r + 1; i < 8; i++)
        {
            if (!MoveRook(i, c))
            {
                if((i, c).IsWithinBoard())
                    yield return (i, c);
                break;
            }
            yield return (i, c);
        }
        // left
        for (int i = c - 1; i >= 0; i--)
        {
            if (!MoveRook(r, i))
            {
                if((r, i).IsWithinBoard())
                    yield return (r, i);
                break;
            }
            yield return (r, i);
        }
        // right
        for (int i = c + 1; i < 8; i++)
        {
            if (!MoveRook(r, i))
            {
                if((r, i).IsWithinBoard())
                    yield return (r, i);
                break;
            }
            yield return (r, i);
        }
    }
}