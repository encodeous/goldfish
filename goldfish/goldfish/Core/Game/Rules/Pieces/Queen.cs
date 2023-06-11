using goldfish.Core.Data;

namespace goldfish.Core.Game.Rules.Pieces;

public struct Queen : IPieceLogic
{
    public int GetMoves(in ChessState state, int r, int c, Span<ChessMove> moves, bool autoPromotion)
    {
        int cnt = new Rook().GetMoves(state, r, c, moves, autoPromotion);
        cnt += new Bishop().GetMoves(state, r, c, moves[cnt..], autoPromotion);
        return cnt;
    }

    public int GetAttacks(in ChessState state, int r, int c, Span<(int, int)> attacks)
    {
        int cnt = new Rook().GetAttacks(state, r, c, attacks);
        cnt += new Bishop().GetAttacks(state, r, c, attacks[cnt..]);
        return cnt;
    }

    public int CountAttacks(in ChessState state, int r, int c)
    {
        int cnt = new Rook().CountAttacks(state, r, c);
        cnt += new Bishop().CountAttacks(state, r, c);
        return cnt;
    }
}