using goldfish.Core.Data;

namespace goldfish.Core.Game.Rules.Pieces;

public static class PieceUtils
{
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

    public static bool IsEmptySquare(in ChessState state, int r, int c)
    {
        if (!(r, c).IsWithinBoard()) return false;
        var p = state.GetPiece(r, c);
        return p.GetPieceType() == PieceType.Space;
    }
}