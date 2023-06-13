using goldfish.Core.Data;

namespace goldfish.Core.Game.Rules.Pieces;

public struct Pawn : IPieceLogic
{
    public readonly int GetMoves(in ChessState state, int r, int c, Span<ChessMove> moves, bool autoPromotion)
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
            var ns = state;
            ns.FinalizeTurn();
            ns.Move((r, c), (fwd, c));
            var mov = new ChessMove()
            {
                NewPos = (fwd, c),
                NewState = ns,
                OldPos = (r, c)
            };
            if (RuleUtils.VerifyCheckPermits(state, mov))
            {
                if (autoPromotion && mov.IsPromotion)
                {
                    AddPromotionVariants(ref cnt, mov, moves);
                }
                else
                {
                    moves[cnt++] = mov;
                }
            }
            
            // move 2x
            fwd += dir;
            if (fwd is >= 0 and < 8 && state.GetPiece(fwd, c).GetPieceType() == PieceType.Space
                && r is 1 or 6 // only initial position, once the pawn reaches the other side this case is also not considered since there is only 1 square
                )
            {
                ns = state;
                ns.FinalizeTurnWithEnPassant(c, side);
                ns.Move((r, c), (fwd, c));
                mov = new ChessMove()
                {
                    NewPos = (fwd, c),
                    NewState = ns,
                    OldPos = (r, c)
                };
                if(RuleUtils.VerifyCheckPermits(state, mov)) moves[cnt++] = mov;
            }

            fwd -= dir;
        }

        // captures
        
        // left capture
        if (CanCapture(fwd, c - 1, out var resL, state, side, r, c, dir))
        {
            if (autoPromotion && resL.Value.IsPromotion)
            {
                AddPromotionVariants(ref cnt, resL.Value, moves);
            }
            else
            {
                moves[cnt++] = resL.Value;
            }
        }
        
        // right capture
        if (CanCapture(fwd, c + 1, out var resR, state, side, r, c, dir))
        {
            if (autoPromotion && resR.Value.IsPromotion)
            {
                AddPromotionVariants(ref cnt, resR.Value, moves);
            }
            else
            {
                moves[cnt++] = resR.Value;
            }
        }

        return cnt;
    }

    private void AddPromotionVariants(ref int cnt, in ChessMove move, Span<ChessMove> moves)
    {
        moves[cnt++] = PromotionVariant(move, PromotionType.Bishop);
        moves[cnt++] = PromotionVariant(move, PromotionType.Knight);
        moves[cnt++] = PromotionVariant(move, PromotionType.Queen);
        moves[cnt++] = PromotionVariant(move, PromotionType.Rook);
    }
    
    private static ChessMove PromotionVariant(ChessMove move, PromotionType type)
    {
        var ns = move.NewState;
        ns.Promote(move.NewPos, type);
        return move with { NewState = ns };
    }
    
    private bool CanCapture(int nr, int nc, out ChessMove? move, in ChessState state, Side side, int r, int c, int dir)
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
                if (!RuleUtils.VerifyCheckPermits(state, move.Value))
                {
                    return false;
                }
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
                if (!RuleUtils.VerifyCheckPermits(state, move.Value))
                {
                    return false;
                }
                return true;
            }
        }

        move = default;
        return false;
    }

    public readonly int GetAttacks(in ChessState state, int r, int c, Span<(int, int)> attacks)
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