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

        bool CanCapture(int nr, int nc, out ChessMove? move)
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
                if(state.Additional.CheckEnPassant(nc, side.GetOpposing())) // check en passant
                {
                    // take with en passant
                    state.FinalizeTurn();
                    nState.Move((r, c), (nr, nc));
                    nState.SetPiece(nr - dir, nc, PieceType.Space.GetPiece(Side.None));
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
        
        // captures
        
        // left capture
        if (CanCapture(fwd, c - 1, out var resL))
        {
            yield return resL.Value;
        }
        
        // right capture
        if (CanCapture(fwd, c + 1, out var resR))
        {
            yield return resR.Value;
        }
    }

    public IEnumerable<(int, int)> GetAttacks(ChessState state, int r, int c)
    {
        var piece = state.GetPiece(r, c);
        var side = piece.GetSide();
        int dir = side == Side.White ? 1 : -1;
        
        // move forward
        // check for obstruction
        var fwd = dir + r;
        if (fwd is < 0 or >= 8)
        {
            yield break;
        }

        if ((fwd, c - 1).IsWithinBoard())
        {
            // left
            yield return (fwd, c - 1);
        }
        if ((fwd, c + 1).IsWithinBoard())
        {
            // right
            yield return (fwd, c + 1);
        }
    }
}