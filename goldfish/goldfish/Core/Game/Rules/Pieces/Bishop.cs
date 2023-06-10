using goldfish.Core.Data;

namespace goldfish.Core.Game.Rules.Pieces;

public struct Bishop : IPieceLogic
{
    public IEnumerable<ChessMove> GetMoves(ChessState state, int r, int c)
    {
        for (int i = 1; i < 8; i++)
        {
            if (!RuleUtils.MovePiece(state, r, c, r - i, c - i, out var move1))
            {
                if(move1.HasValue) 
                    yield return move1.Value;
                break;
            }
            yield return move1.Value;
        }

        for (int i = 1; i < 8; i++)
        {
            if (!RuleUtils.MovePiece(state, r, c, r - i, c + i, out var move2))
            {
                if (move2.HasValue)
                    yield return move2.Value;
                break;
            }

            yield return move2.Value;
        }

        for (int i = 1; i < 8; i++)
        {
            if (!RuleUtils.MovePiece(state, r, c, r + i, c - i, out var move3))
            {
                if (move3.HasValue)
                    yield return move3.Value;
                break;
            }

            yield return move3.Value;
        }
        for (int i = 1; i < 8; i++)
        {
            if (!RuleUtils.MovePiece(state, r, c, r + i, c + i, out var move4))
            {
                if(move4.HasValue) 
                    yield return move4.Value;
                break;
            }
            yield return move4.Value;
        }
    }

    public int CountMoves(in ChessState state, int r, int c)
    {
        var cnt = 0;
        cnt += RuleUtils.CountMoves(state, r, c, -1, -1, 7);
        cnt += RuleUtils.CountMoves(state, r, c, -1, 1, 7);
        cnt += RuleUtils.CountMoves(state, r, c, 1, -1, 7);
        cnt += RuleUtils.CountMoves(state, r, c, 1, 1, 7);
        return cnt;
    }

    public int GetAttacks(ChessState state, int r, int c, Span<(int, int)> attacks)
    {
        int cnt = RuleUtils.GetAttacks(state, r, c, -1, -1, attacks, 7);
        cnt += RuleUtils.GetAttacks(state, r, c, -1, 1, attacks[cnt..], 7);
        cnt += RuleUtils.GetAttacks(state, r, c, 1, -1, attacks[cnt..], 7);
        cnt += RuleUtils.GetAttacks(state, r, c, 1, 1, attacks[cnt..], 7);
        return cnt;
    }

    public int CountAttacks(in ChessState state, int r, int c)
    {
        var cnt = 0;
        cnt += RuleUtils.CountAttacks(state, r, c, -1, -1, 7);
        cnt += RuleUtils.CountAttacks(state, r, c, -1, 1, 7);
        cnt += RuleUtils.CountAttacks(state, r, c, 1, -1, 7);
        cnt += RuleUtils.CountAttacks(state, r, c, 1, 1, 7);
        return cnt;
    }
}