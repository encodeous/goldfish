
using System.Diagnostics;
using System.Security.Cryptography;
using engine_test;
using goldfish.Core.Data;
using goldfish.Core.Data.Optimization;
using goldfish.Core.Game;
using goldfish.Core.Game.FEN;
using goldfish.Engine;
using Spectre.Console;

var game = FenConvert.Parse("rnb1kbnr/pppp1ppp/4p3/8/4P2q/2N3P1/PPPP1P1P/R1BQKBNR b KQkq - 0 1");

// var game = FenConvert.Parse(Console.ReadLine());

// Console.WriteLine(game.GetGameState());
//
// return;

Span<(ChessMove, double)> moves = new (ChessMove, double)[6];
ulong positions = 0;
Console.Write(Tst.Get(game).Hash);
var start = Stopwatch.GetTimestamp();
var tEval = GoldFishEngine.NextOptimalMoves(game, 6, ref moves, ref positions);
var end = Stopwatch.GetElapsedTime(start);

Console.WriteLine($"Calculated - {tEval} {positions} positions in {end}");

foreach (var (move, eval) in moves)
{
    Console.WriteLine($"Step -- E: {eval} --");
    
    Console.WriteLine($"{move.Type} to {move.NewPos}");

    AnsiConsole.Write(BoardPrinter.PrintBoard(in move.NewState, move));
    
    Console.WriteLine($"{FenConvert.ToFen(move.NewState)}");
}

Console.Read();