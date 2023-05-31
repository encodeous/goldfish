namespace goldfish.Core.Data;

public struct ChessMove
{
    public (int, int) NewPos;
    public (int, int) OldPos;
    public PieceType? Taken;
}