using System.Diagnostics;
using goldfish.Core.Data;
using goldfish.Core.Game;

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
        if (depth == 1)
        {
            cnt += moveCnt;
        }
        else
        {
            for(int m = 0; m < moveCnt; m++)
            {
                cnt += CountNextGames(tMoves[m].NewState, depth - 1);
            }
        }
    }

    return cnt;
}

for (int i = 0; i < 10; i++)
{
    var start = Stopwatch.GetTimestamp();
    Console.WriteLine(CountNextGames(ChessState.DefaultState(), 5));
    Console.WriteLine(Stopwatch.GetElapsedTime(start));
}

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