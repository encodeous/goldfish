using goldfish.Core.Data;

namespace goldfish.Core.Game.Rules.Pieces;

public struct Bishop : IPieceLogic
{
    public int GetMoves(in ChessState state, int r, int c, Span<ChessMove> moves, bool autoPromotion)
    {
        var cnt = RuleUtils.GetMoves(state, r, c, -1, -1, moves, 7);
        cnt += RuleUtils.GetMoves(state, r, c, -1, 1, moves[cnt..], 7);
        cnt += RuleUtils.GetMoves(state, r, c, 1, -1, moves[cnt..], 7);
        cnt += RuleUtils.GetMoves(state, r, c, 1, 1, moves[cnt..], 7);
        return cnt;
    }

    public int GetAttacks(in ChessState state, int r, int c, Span<(int, int)> attacks)
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