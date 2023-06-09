
using goldfish.Core.Game.FEN;
using goldfish.Engine;

// var game = FenConvert.Parse("rn1qkbr1/2pppppp/bp3n2/4P3/8/2N5/PPPPNPPP/R1BQK2R w KQq - 1 4");
var game = FenConvert.Parse("N1bqkb2/pp1pp1Q1/1P4p1/2P1n1p1/8/4B3/PP3PPP/R3KB1R b KQ - 1 19");

var eval = GoldFishEngine.NextMoveSeq(game, 5, out var move);

foreach (var cMove in move)
{
    Console.WriteLine($"{cMove.NewPos}");

    Console.WriteLine($"{eval} - {FenConvert.ToFen(cMove.NewState)}");
}