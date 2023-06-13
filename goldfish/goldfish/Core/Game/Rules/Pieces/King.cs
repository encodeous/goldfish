using goldfish.Core.Data;

namespace goldfish.Core.Game.Rules.Pieces;

public struct King : IPieceLogic
{
    private static readonly (int, int)[] _moves = {
        (0, 1),
        (1, 1),
        (1, 0),
        (0, -1),
        (-1, 1),
        (-1, 0),
        (-1, -1),
        (1, -1),
    };
    public int GetMoves(in ChessState state, int r, int c, Span<ChessMove> moves, bool autoPromotion)
    {
        var cnt = 0;
        var piece = state.GetPiece(r, c);
        var side = piece.GetSide();
        var attackMtx = state.GetAttackMatrix(side.GetOpposing());
        foreach (var move in _moves)
        {
            var (a, b) = move;
            var nr = a + r;
            var nc = b + c;
            if(!(nr, nc).IsWithinBoard()) continue;
            if(attackMtx[nr, nc] || state.GetPiece(nr, nc).GetSide() == side) continue;
            CastleType? castleType = default;
            (int, int)? newCap = default;
            var ns = state;
            ns.FinalizeTurn();
            if (state.GetPiece(nr, nc).GetPieceType() != PieceType.Space)
            {
                newCap = (nr, nc);
            }
            // check for castle
            if (side == Side.Black)
            {
                if (!state.Additional.CheckCastle(CastleType.BlackKs) && a == 0 && b == 1)
                    castleType = CastleType.BlackKs;
                if (!state.Additional.CheckCastle(CastleType.BlackQs) && a == 0 && b == -1)
                    castleType = CastleType.BlackQs;
                ns.Additional.MarkCastle(CastleType.BlackKs);
                ns.Additional.MarkCastle(CastleType.BlackQs);
            }
            else
            {
                if (!state.Additional.CheckCastle(CastleType.WhiteKs) && a == 0 && b == 1)
                    castleType = CastleType.WhiteKs;
                if (!state.Additional.CheckCastle(CastleType.WhiteQs) && a == 0 && b == -1)
                    castleType = CastleType.WhiteQs;
                ns.Additional.MarkCastle(CastleType.WhiteKs);
                ns.Additional.MarkCastle(CastleType.WhiteQs);
            }
            ns.Move((r, c), (nr, nc));
            if (castleType is not null)
            {
                ChessState castleState = ns;
                // the king will castle
                // check preconditions
                var rPos = castleType.Value.GetCastleRookPos();
                var dir = Math.Sign(rPos.Item2 - c);
                var valid = true;
                var rPiece = state.GetPiece(rPos.Item1, rPos.Item2);
                if(rPiece.GetPieceType() != PieceType.Rook || !rPiece.IsSide(side)) valid = false;
                for (int i = Math.Min(rPos.Item2 - dir, c + dir); i <= Math.Max(rPos.Item2 - dir, c + dir); i++)
                {
                    if (state.GetPiece(r, i).GetPieceType() == PieceType.Space) continue;
                    valid = false;
                    break;
                }

                if (attackMtx[r, c]) valid = false;
                if (attackMtx[nr, nc]) valid = false;

                if (valid)
                {
                    // continue with castle
                    castleState.Move((nr, nc), (nr, nc + dir));
                    castleState.Move(rPos, (nr, nc));
                    var mov1 = new ChessMove()
                    {
                        NewPos = (nr, nc + dir),
                        NewState = castleState,
                        Taken = newCap,
                        OldPos = (r, c)
                    };
                    if (RuleUtils.VerifyCheckPermits(state, mov1))
                    {
                        moves[cnt++] = mov1;
                    }
                }
            }
            var mov2 = new ChessMove()
            {
                NewPos = (nr, nc),
                NewState = ns,
                Taken = newCap,
                OldPos = (r, c)
            };
            if (RuleUtils.VerifyCheckPermits(state, mov2))
            {
                moves[cnt++] = mov2;
            }
        }

        return cnt;
    }

    public int GetAttacks(in ChessState state, int r, int c, Span<(int, int)> attacks)
    {
        int cnt = 0;
        foreach (var mv in _moves)
        {
            if((r + mv.Item1, c + mv.Item2).IsWithinBoard())
                attacks[cnt++] = (r + mv.Item1, c + mv.Item2);
        }

        return cnt;
    }

    public int CountAttacks(in ChessState state, int r, int c)
    {
        return RuleUtils.CountAttacks(r, c, _moves);
    }
}