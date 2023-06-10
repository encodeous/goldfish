using goldfish.Core.Data;

namespace goldfish.Core.Game.Rules.Pieces;

public struct Pawn : IPieceLogic
{
    public IEnumerable<ChessMove> GetMoves(ChessState state, int r, int c)
    {
        var piece = state.GetPiece(r, c);
        var side = piece.GetSide();
        var dir = side == Side.White ? 1 : -1;
        
        // move forward
        // check for obstruction
        var fwd = dir + r;
        if (fwd is < 0 or >= 8)
        {
            yield break;
        }
        
        if (state.GetPiece(fwd, c).GetPieceType() == PieceType.Space)
        {
            // move 1x
            var ns = state;
            ns.FinalizeTurn();
            ns.Move((r, c), (fwd, c));
            yield return new ChessMove()
            {
                NewPos = (fwd, c),
                NewState = ns,
                OldPos = (r, c)
            };
            
            // move 2x
            fwd += dir;
            if (fwd is >= 0 and < 8 && state.GetPiece(fwd, c).GetPieceType() == PieceType.Space
                && r is 1 or 6 // only initial position, once the pawn reaches the other side this case is also not considered since there is only 1 square
                )
            {
                ns = state;
                ns.FinalizeTurnWithEnPassant(c, side);
                ns.Move((r, c), (fwd, c));
                yield return new ChessMove()
                {
                    NewPos = (fwd, c),
                    NewState = ns,
                    OldPos = (r, c)
                };
            }

            fwd -= dir;
        }

        // captures
        
        // left capture
        if (CanCapture(fwd, c - 1, out var resL, ref state, side, r, c, dir))
        {
            yield return resL.Value;
        }
        
        // right capture
        if (CanCapture(fwd, c + 1, out var resR, ref state, side, r, c, dir))
        {
            yield return resR.Value;
        }
    }

    public int CountMoves(in ChessState state, int r, int c)
    {
        var cnt = 0;
        var piece = state.GetPiece(r, c);
        var side = piece.GetSide();
        var dir = side == Side.White ? 1 : -1;
        
        // move forward
        // check for obstruction
        var fwd = dir + r;
        if (fwd is < 0 or >= 8)
        {
            return 0;
        }
        
        if (state.GetPiece(fwd, c).GetPieceType() == PieceType.Space)
        {
            // move 1x
            cnt++;
            
            // move 2x
            fwd += dir;
            if (fwd is >= 0 and < 8 && state.GetPiece(fwd, c).GetPieceType() == PieceType.Space
                                    && r is 1 or 6 // only initial position, once the pawn reaches the other side this case is also not considered since there is only 1 square
               )
            {
                cnt++;
            }
            fwd -= dir;
        }

        // captures
        
        // left capture
        if (CanCapture(fwd, c - 1, state, side))
        {
            cnt++;
        }
        
        // right capture
        if (CanCapture(fwd, c + 1, state, side))
        {
            cnt++;
        }

        return cnt;
    }

    private bool CanCapture(int nr, int nc, in ChessState state, Side side)
    {
        var capturePiece = state.GetPiece(nr, nc);
        if (nc is >= 0 and < 8 && capturePiece.GetSide() != side) // only valid if its an opposing piece
        {
            if (capturePiece.GetPieceType() != PieceType.Space)
            {
                return true;
            }
            if(state.Additional.CheckEnPassant(nc, side.GetOpposing())
               && ((side == Side.White && nr == 5) || (side == Side.Black && nr == 2))
              ) // check en passant
            {
                return true;
            }
        }
        return false;
    }
    private bool CanCapture(int nr, int nc, out ChessMove? move, ref ChessState state, Side side, int r, int c, int dir)
    {
        var capturePiece = state.GetPiece(nr, nc);
        if (nc is >= 0 and < 8 && capturePiece.GetSide() != side) // only valid if its an opposing piece
        {
            var nState = state;
            if (capturePiece.GetPieceType() != PieceType.Space)
            {
                nState.FinalizeTurn();
                nState.Move((r, c), (nr, nc));
                move = new ChessMove()
                {
                    Taken = (nr, nc),
                    NewPos = (nr, nc),
                    NewState = nState,
                    OldPos = (r, c)
                };
                return true;
            }
            if(state.Additional.CheckEnPassant(nc, side.GetOpposing())
               && ((side == Side.White && nr == 5) || (side == Side.Black && nr == 2))
              ) // check en passant
            {
                // take with en passant
                nState.FinalizeTurn();
                nState.Move((r, c), (nr, nc));
                nState.SetPiece(nr - dir, nc, PieceType.Space.ToPiece(Side.None));
                move = new ChessMove()
                {
                    Taken = (nr - dir, nc),
                    NewPos = (nr, nc),
                    NewState = nState,
                    OldPos = (r, c)
                };
                return true;
            }
        }

        move = default;
        return false;
    }

    public int GetAttacks(ChessState state, int r, int c, Span<(int, int)> attacks)
    {
        var piece = state.GetPiece(r, c);
        var side = piece.GetSide();
        int dir = side == Side.White ? 1 : -1;
        
        // move forward
        // check for obstruction
        var fwd = dir + r;
        if (fwd is < 0 or >= 8)
        {
            return 0;
        }

        var cnt = 0;

        if ((fwd, c - 1).IsWithinBoard())
        {
            // left
            attacks[cnt++] = (fwd, c - 1);
        }
        if ((fwd, c + 1).IsWithinBoard())
        {
            // right
            attacks[cnt++] = (fwd, c + 1);
        }

        return cnt;
    }

    public int CountAttacks(in ChessState state, int r, int c)
    {
        var cnt = 0;
        var piece = state.GetPiece(r, c);
        var side = piece.GetSide();
        int dir = side == Side.White ? 1 : -1;
        
        // move forward
        // check for obstruction
        var fwd = dir + r;
        if (fwd is < 0 or >= 8)
        {
            return 0;
        }

        if ((fwd, c - 1).IsWithinBoard())
        {
            // left
            cnt++;
        }
        if ((fwd, c + 1).IsWithinBoard())
        {
            // right
            cnt++;
        }

        return cnt;
    }
}