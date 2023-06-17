using goldfish.Core.Data;

namespace goldfish.Core.Game.Rules.Pieces;

public struct Rook : IPieceLogic
{
    public int GetMoves(in ChessState state, int r, int c, Span<ChessMove> moves, bool autoPromotion)
    {
        var cRookState = state;
        // mark the castle state
        if ((r, c).GetRookType() is not null)
        {
            cRookState.Additional.MarkCastle((r, c).GetRookType()!.Value);
        }

        var cnt = RuleUtils.GetMoves(cRookState, r, c, -1, 0, moves, 7);
        cnt += RuleUtils.GetMoves(cRookState, r, c, 1, 0, moves[cnt..], 7);
        cnt += RuleUtils.GetMoves(cRookState, r, c, 0, -1, moves[cnt..], 7);
        cnt += RuleUtils.GetMoves(cRookState, r, c, 0, 1, moves[cnt..], 7);
        return cnt;
    }
    public int GetAttacks(in ChessState state, int r, int c, Span<(int, int)> attacks)
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