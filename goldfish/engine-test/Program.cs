
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json.Nodes;
using engine_test;
using goldfish.Core.Data;
using goldfish.Core.Data.Optimization;
using goldfish.Core.Game;
using goldfish.Core.Game.FEN;
using goldfish.Engine;
using Spectre.Console;

static int CountNextGames(ChessState state, int depth)
{
    if (depth == 0)
    {
        return 1;
    }
    Span<ChessMove> tMoves = stackalloc ChessMove[30];
    int cnt = 0;
    for (var i = 0; i < 8; i++)
    for (var j = 0; j < 8; j++)
    {
        var piece = state.GetPiece(i, j);
        if (!piece.IsSide(state.ToMove) || piece.GetLogic() is null) continue;
        int moveCnt = state.GetValidMovesForSquare(i, j, tMoves);
        for(int m = 0; m < moveCnt; m++)
        {
            cnt += CountNextGames(tMoves[m].NewState, depth - 1);
        }
    }

    return cnt;
}

while (true)
{
    Console.WriteLine(CountNextGames(FenConvert.Parse(Console.ReadLine()), 1));
}

void VerifyMoves(string test)
{
    var tData = JsonNode.Parse(File.ReadAllText($"../../../data/{test}.json"));
    foreach (var caseNode in tData["testCases"].AsArray())
    {
        var startState = FenConvert.Parse(caseNode["start"]["fen"].ToString());
        var endStates = new HashSet<(ulong, ChessState)>(caseNode["expected"].AsArray().Select(x 
            =>
        {
            var state = FenConvert.Parse(x["fen"].ToString());
            return (state.HashState(), state);
        }));
        var thingStates = GetAllMoves(startState);
        thingStates.ExceptWith(endStates);
        foreach (var state in thingStates)
        {
            BoardPrinter.PrintBoard(state.Item2, null);
            Console.WriteLine(FenConvert.ToFen(state.Item2));
        }

        break;
    }
}

static HashSet<(ulong, ChessState)> GetAllMoves(in ChessState state)
{
    int cnt = 0;
    Span<ChessMove> tMoves = stackalloc ChessMove[30];
    var states = new HashSet<(ulong, ChessState)>();
    for (var i = 0; i < 8; i++)
    for (var j = 0; j < 8; j++)
    {
        var piece = state.GetPiece(i, j);
        if (!piece.IsSide(state.ToMove) || piece.GetLogic() is null) continue;
        int moveCnt = state.GetValidMovesForSquare(i, j, tMoves);
        for(int m = 0; m < moveCnt; m++)
        {
            BoardPrinter.PrintBoard(tMoves[m].NewState, null);
            states.Add((tMoves[m].NewState.HashState(), tMoves[m].NewState));
        }
    }

    return states;
}

VerifyMoves("castling");

Console.ReadLine();


// var game = ChessState.DefaultState();
//
// // var game = FenConvert.Parse(Console.ReadLine());
//
// // Console.WriteLine(game.GetGameState());
// //
// // return;
//
// Span<(ChessMove, double)> moves = new (ChessMove, double)[6];
// ulong positions = 0;
// Console.Write(Tst.Get(game).Hash);
// var start = Stopwatch.GetTimestamp();
// var tEval = GoldFishEngine.NextOptimalMoves(game, 6, ref moves, ref positions);
// var end = Stopwatch.GetElapsedTime(start);
//
// Console.WriteLine($"Calculated - {tEval} {positions} positions in {end}");
//
// foreach (var (move, eval) in moves)
// {
//     Console.WriteLine($"Step -- E: {eval} --");
//     
//     Console.WriteLine($"{move.Type} to {move.NewPos}");
//
//     AnsiConsole.Write(BoardPrinter.PrintBoard(in move.NewState, move));
//     
//     Console.WriteLine($"{FenConvert.ToFen(move.NewState)}");
// }
//
// Console.Read();