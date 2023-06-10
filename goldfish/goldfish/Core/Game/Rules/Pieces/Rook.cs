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
            if (!RuleUtils.MovePiece(state, r, c, i, c, out var move))
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
            if (!RuleUtils.MovePiece(state, r, c, i, c, out var move))
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
            if (!RuleUtils.MovePiece(state, r, c, r, i, out var move))
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
            if (!RuleUtils.MovePiece(state, r, c, r, i, out var move))
            {
                if(move.HasValue) 
                    yield return move.Value;
                break;
            }
            yield return move.Value;
        }
    }

    public int CountMoves(in ChessState state, int r, int c)
    {
        var cnt = 0;
        cnt += RuleUtils.CountMoves(state, r, c, -1, 0, 7);
        cnt += RuleUtils.CountMoves(state, r, c, 1, 0, 7);
        cnt += RuleUtils.CountMoves(state, r, c, 0, -1, 7);
        cnt += RuleUtils.CountMoves(state, r, c, 0, 1, 7);
        return cnt;
    }

    public int GetAttacks(ChessState state, int r, int c, Span<(int, int)> attacks)
    {
        int cnt = RuleUtils.GetAttacks(state, r, c, -1, 0, attacks, 7);
        cnt += RuleUtils.GetAttacks(state, r, c, 1, 0, attacks[cnt..], 7);
        cnt += RuleUtils.GetAttacks(state, r, c, 0, -1, attacks[cnt..], 7);
        cnt += RuleUtils.GetAttacks(state, r, c, 0, 1, attacks[cnt..], 7);
        return cnt;
    }

    public int CountAttacks(in ChessState state, int r, int c)
    {
        var cnt = 0;
        cnt += RuleUtils.CountAttacks(state, r, c, -1, 0, 7);
        cnt += RuleUtils.CountAttacks(state, r, c, 1, 0, 7);
        cnt += RuleUtils.CountAttacks(state, r, c, 0, -1, 7);
        cnt += RuleUtils.CountAttacks(state, r, c, 0, 1, 7);
        return cnt;
    }
}