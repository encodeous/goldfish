
using goldfish.Core.Game.FEN;
using goldfish.Engine;

// var game = FenConvert.Parse("rn1qkbr1/2pppppp/bp3n2/4P3/8/2N5/PPPPNPPP/R1BQK2R w KQq - 1 4");
var game = FenConvert.Parse("rn1qkb1r/2pppppp/bp3n2/4P3/8/2N5/PPPPNPPP/R1BQK2R b KQkq - 0 3");

var (eval, move) = GoldFishEngine.NextOptimalMove(game, 2, double.NegativeInfinity, double.PositiveInfinity);

Console.WriteLine($"{move.Value.NewPos}");

Console.WriteLine($"{eval} - {FenConvert.ToFen(move.Value.NewState)}");