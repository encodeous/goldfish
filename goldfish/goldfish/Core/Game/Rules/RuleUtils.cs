using goldfish.Core.Data;
using goldfish.Core.Data.Optimization;

namespace goldfish.Core.Game.Rules;

public static class RuleUtils
{
    public static bool VerifyCheckPermits(in ChessState state, in ChessMove move, StateEvaluationCache? cache = null)
    {
        var piece = state.GetPiece(move.OldPos.Item1, move.OldPos.Item2);
        var isChecked = state.IsChecked(piece.GetSide(), cache);
        var isCheckedAgain = move.NewState.IsChecked(piece.GetSide());
        if (isChecked)
        {
            // only permit blocking moves and do not allow castle out of check
            if (!isCheckedAgain && !move.IsCastle)
            {
                return true;
            }
        }
        else
        {
            return !isCheckedAgain;
        }

        return false;
    }
    public static bool? CanMovePiece(in ChessState state, int r, int c, int nr, int nc)
    {
        var piece = state.GetPiece(r, c);
        var side = piece.GetSide();
        if (!(nr, nc).IsWithinBoard()) return default;
        var p = state.GetPiece(nr, nc);
        if (p.GetPieceType() == PieceType.Space)
        {
            // free to move
            return true;
        }

        if (p.GetSide() == side) return false;
        {
            // only valid if its an opposing piece
            // capture
            return false;
        }
    }
    
    public static bool MovePiece(in ChessState state, int r, int c, int nr, int nc, out ChessMove? move)
    {
        var piece = state.GetPiece(r, c);
        var side = piece.GetSide();
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

    public static int CountMoves(in ChessState state, int r, int c, int dx, int dy, int multiplier = 1)
    {
        var moves = 0;
        for (var i = 1; i <= multiplier; i++)
        {
            var res = CanMovePiece(state, r, c, r + dx * i, c + dy * i);
            if (!res.HasValue || !res.Value)
            {
                if(res.HasValue) 
                    moves++;
                break;
            }
            moves++;
        }

        return moves;
    }
    public static int CountMoves(in ChessState state, int r, int c, IEnumerable<(int, int)> moves)
    {
        int cnt = 0;
        foreach (var move in moves)
        {
            var (ox, oy) = move;
            var nr = r + ox;
            var nc = c + oy;
            var res = CanMovePiece(state, r, c, nr, nc);
            if (!res.HasValue || !res.Value)
            {
                if(res.HasValue) 
                    cnt++;
                break;
            }
            cnt++;
        }

        return cnt;
    }
    
    
    public static int GetMoves(ChessState state, int r, int c, int dx, int dy, Span<ChessMove> moves, int multiplier = 1)
    {
        var cnt = 0;
        for (var i = 1; i <= multiplier; i++)
        {
            if (!MovePiece(state, r, c, r + i * dx, c + i * dy, out var move1))
            {
                if (move1.HasValue)
                    moves[cnt] = move1.Value;
                break;
            }
            moves[cnt++] = move1.Value;
        }

        return cnt;
    }

    public static int CountAttacks(int r, int c, IEnumerable<(int, int)> moves)
    {
        int attacks = 0;
        foreach (var move in moves)
        {
            var (ox, oy) = move;
            var nr = r + ox;
            var nc = c + oy;
            if((nr, nc).IsWithinBoard()) 
                attacks++;
        }

        return attacks;
    }
    
    public static int CountAttacks(in ChessState state, int r, int c, int dx, int dy, int multiplier = 1)
    {
        var attacks = 0;
        for (var i = 1; i <= multiplier; i++)
        {
            if (!IsEmptySquare(state, r + dx * i, c + dy * i))
            {
                if((r + dx * i, c + dy * i).IsWithinBoard()) 
                    attacks++;
                break;
            }
            attacks++;
        }

        return attacks;
    }
    
    public static int GetAttacks(in ChessState state, int r, int c, int dx, int dy, Span<(int, int)> moves, int multiplier = 1)
    {
        var attacks = 0;
        for (var i = 1; i <= multiplier; i++)
        {
            var pos = (r + dx * i, c + dy * i);
            if (!IsEmptySquare(state, r + dx * i, c + dy * i))
            {
                if(pos.IsWithinBoard()) 
                    moves[attacks++] = pos;
                break;
            }
            moves[attacks++] = pos;
        }

        return attacks;
    }
    
    public static bool IsEmptySquare(in ChessState state, int r, int c)
    {
        if (!(r, c).IsWithinBoard()) return false;
        var p = state.GetPiece(r, c);
        return p.GetPieceType() == PieceType.Space;
    }
}