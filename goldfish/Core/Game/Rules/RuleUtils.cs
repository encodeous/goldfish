using System.Runtime.CompilerServices;
using goldfish.Core.Data;
using goldfish.Core.Data.Optimization;

namespace goldfish.Core.Game.Rules;

public static class RuleUtils
{
    public static bool VerifyCheckPermits(in ChessState state, in ChessMove move)
    {
        var piece = state.GetPiece(move.OldPos.Item1, move.OldPos.Item2);
        var isChecked = state.IsChecked(piece.GetSide());
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

    /// <summary>
    /// Tries to move the piece
    /// </summary>
    /// <param name="state"></param>
    /// <param name="r"></param>
    /// <param name="c"></param>
    /// <param name="nr"></param>
    /// <param name="nc"></param>
    /// <param name="move"></param>
    /// <returns>false if the piece cannot move any further</returns>
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
            if (!VerifyCheckPermits(state, move.Value))
            {
                move = null;
            }
            return true;
        }

        if (p.GetSide() == side) return false;
        // only valid if its an opposing piece
        // capture
        var ns1 = state;
        ns1.FinalizeTurn();
        ns1.Move((r, c), (nr, nc));
        move = new ChessMove()
        {
            NewPos = (nr, nc),
            NewState = ns1,
            Taken = (nr, nc),
            OldPos = (r, c)
        };
        if (!VerifyCheckPermits(state, move.Value))
        {
            move = null;
        }

        return false;
    }

    public static int GetMoves(ChessState state, int r, int c, int dx, int dy, Span<ChessMove> moves, int multiplier = 1)
    {
        var cnt = 0;
        for (var i = 1; i <= multiplier; i++)
        {
            var res = MovePiece(state, r, c, r + i * dx, c + i * dy, out var move1);
            if (move1 is not null)
                moves[cnt++] = move1.Value;
            if (!res) break;
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
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEmptySquare(in ChessState state, int r, int c)
    {
        return (r, c).IsWithinBoard() && state.GetPiece(r, c).IsPieceType(PieceType.Space);
    }
}