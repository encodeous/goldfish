using goldfish.Core.Game;

namespace goldfish.Core.Data;

public struct ChessMove
{
    public (int, int) NewPos;
    public (int, int) OldPos;
    public (int, int)? Taken;
    public ChessState NewState;
    internal bool WasPromotion;
    public PieceType Type => NewState.GetPiece(NewPos.Item1, NewPos.Item2).GetPieceType();
    public bool IsPromotion => Type == PieceType.Pawn && NewPos.Item1 is 0 or 7 || WasPromotion;
    public bool IsCastle => Type == PieceType.King && Math.Abs(NewPos.Item2 - OldPos.Item2) == 2;
}