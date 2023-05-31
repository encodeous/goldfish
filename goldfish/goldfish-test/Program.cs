using goldfish.Core.Data;
using goldfish.Core.Game;

var state = ChessState.DefaultState();
state.SetPiece(0, 0, PieceType.Queen.GetPiece(Side.White));
for (int i = 0; i < 8; i++)
{
    for (int j = 0; j < 8; j++)
    {
        Console.Write($"{Piece.GetPieceType(state.GetPiece(i, j))} ");
    }
    Console.WriteLine();
}
state.Additional.MarkCastle(CastleType.BlackKs);
Console.WriteLine(state.Additional.CheckCastle(CastleType.BlackKs));