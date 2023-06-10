
using System.Security.Cryptography;
using engine_test;
using goldfish.Core.Data;
using goldfish.Core.Data.Optimization;
using goldfish.Core.Game;
using goldfish.Core.Game.FEN;
using goldfish.Engine;
using Spectre.Console;

var game = FenConvert.Parse("r4b1r/p2pkppp/b2Np3/3q4/4N3/1Qp1P3/PP3PPP/R2R2K1 b - - 1 1");

// var game = FenConvert.Parse(Console.ReadLine());

// Console.WriteLine(game.GetGameState());
//
// return;

Span<(ChessMove, double)> moves = new (ChessMove, double)[6];
long cnt = 0;
var tEval = GoldFishEngine.NextOptimalMoves(game, 6, ref moves, ref cnt);

foreach (var (move, eval) in moves)
{
    Console.WriteLine($"Step -- E: {eval} --");
    
    Console.WriteLine($"{move.Type} to {move.NewPos}");

    AnsiConsole.Write(BoardPrinter.PrintBoard(in move.NewState, move));
    
    Console.WriteLine($"{FenConvert.ToFen(move.NewState)}");
}

Console.Read();