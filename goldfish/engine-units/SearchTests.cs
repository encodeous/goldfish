using goldfish.Core.Data;
using goldfish.Core.Data.Optimization;
using goldfish.Core.Game;
using goldfish.Core.Game.FEN;

namespace engine_units;

public class SearchTests
{
    [Theory]
    [InlineData(1, 20)]
    [InlineData(2, 400)]
    [InlineData(3, 8902)]
    [InlineData(4, 197281)]
    [InlineData(5, 4865609)]
    [InlineData(6, 119060324)]
    public void CountMoves(int ply, int moves)
    {
        int mov = CountNextGames(ChessState.DefaultState(), ply);
        Assert.Equal(moves, mov);
    }
    [Theory]
    [InlineData("r3k2Q/pp3p1p/3qp3/2pp2N1/3P4/4PP2/PP1K2PP/nNB4R b k - 0 15", 1, 3)]
    [InlineData("r3k2Q/pp3p1p/3qp3/2pp2N1/3P4/4PP2/PP1K2PP/nNB4R b k - 0 15", 5, 2891505)]
    [InlineData("r3k2Q/pp3p1p/3qp3/2pp2N1/3P4/4PP2/PP1K2PP/nNB4R b k - 0 15", 4, 102724)]
    [InlineData("r3k2Q/pp3p1p/3qp3/2pp2N1/3P4/4PP2/PP1K2PP/nNB4R b k - 0 15", 3, 2938)]
    [InlineData("r6r/p3kp1p/4np2/1Bb5/3p4/P4N2/1P3PPP/R3K2R w KQ - 2 18", 1, 34)]
    [InlineData("1k6/1b6/8/8/7R/8/8/4K2R b K - 0 1", 5, 1063542)]
    [InlineData("K1k5/8/P7/8/8/8/8/8 w - - 0 1", 6, 2217)]
    [InlineData("8/5bk1/8/2Pp4/8/1K6/8/8 w - d6 0 1", 6, 824064)]
    [InlineData("8/8/1k6/8/2pP4/8/5BK1/8 b - d3 0 1", 6, 824064)]
    [InlineData("r3k2r/1b4bq/8/8/8/8/7B/R3K2R w KQkq - 0 1", 4, 1274206)]
    [InlineData("r3k2r/1b4bq/8/8/8/8/7B/2KR3R b kq - 1 1", 3, 32959)]
    [InlineData("r3k2r/8/3Q4/8/8/5q2/8/R3K2R b KQkq - 0 1", 4, 1720476)]
    [InlineData("8/8/2k5/5q2/5n2/8/5K2/8 b - - 0 1", 4, 23527)]
    [InlineData("8/k1P5/8/1K6/8/8/8/8 w - - 0 1", 7, 567584)]
    [InlineData("8/8/1P2K3/8/2n5/1q6/8/5k2 b - - 0 1", 5, 1004658)]
    [InlineData("8/8/1k6/2b5/2pP4/8/5K2/8 b - d3 0 1", 6, 1440467)]
    public void CountMovesCustom(string fen, int ply, int moves)
    {
        int mov = CountNextGames(FenConvert.Parse(fen), ply);
        Assert.Equal(moves, mov);
    }

    static int CountNextGames(ChessState state, int depth)
    {
        if (depth == 0)
        {
            return 1;
        }
        Span<ChessMove> tMoves = stackalloc ChessMove[32];
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
}