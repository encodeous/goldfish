using System.Text.Json.Nodes;
using engine_test;
using goldfish.Core.Data;
using goldfish.Core.Game;
using goldfish.Core.Game.FEN;
using goldfish.Engine;
using goldfish.Engine.Analysis;
using goldfish.Engine.Searcher;

namespace engine_units;

public class NegamaxTest
{
    [Theory]
    [InlineData("castling")]
    [InlineData("famous")]
    [InlineData("pawns")]
    [InlineData("promotions")]
    [InlineData("standard")]
    [InlineData("taxing")]
    public void VerifyMovesBasicScenarios(string test)
    {
        var tData = JsonNode.Parse(File.ReadAllText($"../../../data/{test}.json"));
        foreach (var caseNode in tData["testCases"].AsArray())
        {
            var startState = FenConvert.Parse(caseNode["start"]["fen"].ToString());
            ulong pos = 0;
            Span<(ChessMove, double)> basicMoves = new (ChessMove, double)[5];
            Span<(ChessMove, double)> optMoves = new (ChessMove, double)[5];
            var basicEval = BasicNegamax(startState, 3, ref basicMoves);
            var optimizedEval = GoldFishEngine.NextOptimalMoves(startState, 3, ref optMoves);
            if (startState.ToMove == Side.Black)
            {
                Assert.True(basicEval >= optimizedEval);
            }
            else
            {
                Assert.True(basicEval <= optimizedEval);
            }
            // Assert.Equal(basicMoves[0], optMoves[0]);
        }
    }
    
    [Theory]
    [InlineData("castling")]
    [InlineData("famous")]
    [InlineData("pawns")]
    [InlineData("promotions")]
    [InlineData("standard")]
    [InlineData("taxing")]
    public void VerifyMovesAbScenarios(string test)
    {
        var tData = JsonNode.Parse(File.ReadAllText($"../../../data/{test}.json"));
        foreach (var caseNode in tData["testCases"].AsArray())
        {
            var startState = FenConvert.Parse(caseNode["start"]["fen"].ToString());
            ulong pos = 0;
            Span<(ChessMove, double)> referenceMoves = new (ChessMove, double)[5];
            Span<(ChessMove, double)> optMoves = new (ChessMove, double)[5];
            var basicEval = UncachedNegamax(startState, 5, ref referenceMoves, ref pos);
            var optimizedEval = GoldFishEngine.NextOptimalMoves(startState, 5, ref optMoves);
            if (startState.ToMove == Side.Black)
            {
                Assert.True(basicEval >= optimizedEval);
            }
            else
            {
                Assert.True(basicEval <= optimizedEval);
            }
            // Assert.Equal(basicMoves[0], optMoves[0]);
        }
    }
    
    [Theory]
    // [InlineData("r1bqkb1r/1ppppppp/5n2/p2P4/1n2P3/P4N2/1PP2PPP/RNBQKB1R b KQkq - 0 5", true)]
    [InlineData("r1bqkb1r/1ppppppp/5n2/p2P4/1n2P3/P4N2/1PP2PPP/RNBQKB1R b KQkq - 0 5")]
    public void VerifyGamesBasic(string fen)
    {
        var startState = FenConvert.Parse(fen);
        ulong pos = 0;
        Span<(ChessMove, double)> referenceMoves = new (ChessMove, double)[3];
        Span<(ChessMove, double)> optMoves = new (ChessMove, double)[3];
        double referenceEval = BasicNegamax(startState, 3, ref referenceMoves);
        var optimizedEval = GoldFishEngine.NextOptimalMoves(startState, 3, ref optMoves);
        if (startState.ToMove == Side.Black)
        {
            Assert.True(referenceEval >= optimizedEval);
        }
        else
        {
            Assert.True(referenceEval <= optimizedEval);
        }
    }
    
    [Theory]
    // [InlineData("r1bqkb1r/1ppppppp/5n2/p2P4/1n2P3/P4N2/1PP2PPP/RNBQKB1R b KQkq - 0 5", true)]
    [InlineData("r1bqkb1r/1ppppppp/5n2/p2P4/1n2P3/P4N2/1PP2PPP/RNBQKB1R b KQkq - 0 5")]
    [InlineData("r3k2Q/pp3p1p/3qp3/2pp2N1/3P4/4PP2/PP1K2PP/nNB4R b k - 0 15")]
    [InlineData("r6r/p3kp1p/4np2/1Bb5/3p4/P4N2/1P3PPP/R3K2R w KQ - 2 18")]
    [InlineData("1k6/1b6/8/8/7R/8/8/4K2R b K - 0 1")]
    [InlineData("K1k5/8/P7/8/8/8/8/8 w - - 0 1")]
    [InlineData("8/5bk1/8/2Pp4/8/1K6/8/8 w - d6 0 1")]
    [InlineData("8/8/1k6/8/2pP4/8/5BK1/8 b - d3 0 1")]
    [InlineData("r3k2r/1b4bq/8/8/8/8/7B/R3K2R w KQkq - 0 1")]
    [InlineData("r3k2r/1b4bq/8/8/8/8/7B/2KR3R b kq - 1 1")]
    [InlineData("r3k2r/8/3Q4/8/8/5q2/8/R3K2R b KQkq - 0 1")]
    [InlineData("8/8/2k5/5q2/5n2/8/5K2/8 b - - 0 1")]
    [InlineData("8/k1P5/8/1K6/8/8/8/8 w - - 0 1")]
    [InlineData("8/8/1P2K3/8/2n5/1q6/8/5k2 b - - 0 1")]
    [InlineData("8/8/1k6/2b5/2pP4/8/5K2/8 b - d3 0 1")]
    public void VerifyGamesAbPruning(string fen)
    {
        var startState = FenConvert.Parse(fen);
        ulong pos = 0;
        Span<(ChessMove, double)> referenceMoves = new (ChessMove, double)[5];
        Span<(ChessMove, double)> optMoves = new (ChessMove, double)[5];
        double referenceEval = UncachedNegamax(startState, 5, ref referenceMoves, ref pos);
        var optimizedEval = GoldFishEngine.NextOptimalMoves(startState, 5, ref optMoves);
        Assert.Equal(referenceEval, optimizedEval);
        Assert.Equal(referenceMoves[0], optMoves[0]);
    }
    
    [Theory]
    // [InlineData("r1bqkb1r/1ppppppp/5n2/p2P4/1n2P3/P4N2/1PP2PPP/RNBQKB1R b KQkq - 0 5", true)]
    [InlineData("r1bqkb1r/1ppppppp/5n2/p2P4/1n2P3/P4N2/1PP2PPP/RNBQKB1R b KQkq - 0 5")]
    [InlineData("r3k2Q/pp3p1p/3qp3/2pp2N1/3P4/4PP2/PP1K2PP/nNB4R b k - 0 15")]
    [InlineData("r6r/p3kp1p/4np2/1Bb5/3p4/P4N2/1P3PPP/R3K2R w KQ - 2 18")]
    [InlineData("1k6/1b6/8/8/7R/8/8/4K2R b K - 0 1")]
    [InlineData("K1k5/8/P7/8/8/8/8/8 w - - 0 1")]
    [InlineData("8/5bk1/8/2Pp4/8/1K6/8/8 w - d6 0 1")]
    [InlineData("8/8/1k6/8/2pP4/8/5BK1/8 b - d3 0 1")]
    [InlineData("r3k2r/1b4bq/8/8/8/8/7B/R3K2R w KQkq - 0 1")]
    [InlineData("r3k2r/8/3Q4/8/8/5q2/8/R3K2R b KQkq - 0 1")]
    [InlineData("8/8/2k5/5q2/5n2/8/5K2/8 b - - 0 1")]
    [InlineData("8/k1P5/8/1K6/8/8/8/8 w - - 0 1")]
    [InlineData("8/8/1P2K3/8/2n5/1q6/8/5k2 b - - 0 1")]
    [InlineData("8/8/1k6/2b5/2pP4/8/5K2/8 b - d3 0 1")]
    public void VerifyGamesAbPruningParallel(string fen)
    {
        var startState = FenConvert.Parse(fen);
        ulong pos = 0;
        Span<(ChessMove, double)> referenceMoves = new (ChessMove, double)[5];
        double referenceEval = GoldFishEngine.NextOptimalMoves(startState, 5, ref referenceMoves);
        var searcher = new GoldFishSearcher(TimeSpan.FromSeconds(5));
        var optimizedEval = searcher.ParallelSearch(startState, 5, CancellationToken.None);
        Assert.Equal(referenceEval, optimizedEval.EngineEval);
        Assert.Equal(referenceMoves[0].Item1, optimizedEval.BestMove);
    }
    
    public static double BasicNegamax(ChessState state, int depth, ref Span<(ChessMove, double)> bestMoves, double staticEval = double.NaN)
    {
        // alpha is white, beta is black
        if (depth == 0 || double.IsPositiveInfinity(Math.Abs(staticEval)))
        {
            return staticEval;
        }
        var toPlay = state.ToMove;
        (ChessMove, double)? lastMove = null;
        double optimalVal;

        optimalVal = toPlay == Side.White ? 
            // maximize
            double.NegativeInfinity :
            // minimize
            double.PositiveInfinity;
        
        Span<(ChessMove, double)> evalMoves = stackalloc (ChessMove, double)[32 * 30]; // should be plenty... i think
        Span<ChessMove> tMoves = stackalloc ChessMove[32];
        int cnt = 0;
        for (var i = 0; i < 8; i++)
        for (var j = 0; j < 8; j++)
        {
            var piece = state.GetPiece(i, j);
            if (!piece.IsSide(toPlay) || piece.GetLogic() is null) continue;
            int moveCnt = state.GetValidMovesForSquare(i, j, tMoves);
            for(int m = 0; m < moveCnt; m++)
            {
                evalMoves[cnt++] = (tMoves[m], GameStateAnalyzer.Evaluate(state));
            }
        }

        evalMoves = evalMoves[..cnt];
        
        if (state.ToMove == Side.Black)
        {
            evalMoves.Sort((tuple, valueTuple) => tuple.Item2.CompareTo(valueTuple.Item2));
        }
        else
        {
            evalMoves.Sort((tuple, valueTuple) => valueTuple.Item2.CompareTo(tuple.Item2));
        }
        
        Span<(ChessMove, double)> optimalMoves = stackalloc (ChessMove, double)[depth - 1];
        ulong curMoves = 0;
        for (int i = 0; i < cnt; i++)
        {
            var (move, mEval) = evalMoves[i];
            var nEval = BasicNegamax(move.NewState, depth - 1, ref optimalMoves, mEval);
            lastMove = (move, mEval);
            curMoves++;

            bool isMoreOptimal = false;
            
            if (toPlay == Side.White)
            {
                // maximize
                if (optimalVal < nEval) isMoreOptimal = true;
            }
            else
            {
                // minimize
                if (optimalVal > nEval) isMoreOptimal = true;
            }

            if (isMoreOptimal)
            {
                optimalVal = nEval;
                bestMoves[0] = (move, mEval);
                optimalMoves.CopyTo(bestMoves[1..]);
            }
        }
        if (double.IsInfinity(optimalVal) && lastMove is not null)
        {
            bestMoves[0] = lastMove.Value;
        }
        return optimalVal;
    }
    
    public static double UncachedNegamax(ChessState state, int depth, ref Span<(ChessMove, double)> bestMoves, ref ulong moves, double alpha = double.NegativeInfinity, double beta = double.PositiveInfinity, double eval = double.NaN)

    {
        // alpha is white, beta is black
        if (depth == 0 || double.IsPositiveInfinity(Math.Abs(eval)))
        {
            return eval;
        }

        var toPlay = state.ToMove;

        double optimalVal;

        
        optimalVal = toPlay == Side.White ? 
            // maximize
            double.NegativeInfinity :
            // minimize
            double.PositiveInfinity;

        var evalMoves = new List<(ChessMove, double)>();
        Span<ChessMove> tMoves = stackalloc ChessMove[32];
        for (var i = 0; i < 8; i++)
        for (var j = 0; j < 8; j++)
        {
            var piece = state.GetPiece(i, j);
            if (piece.GetSide() != toPlay || piece.GetLogic() is null) continue;
            int moveCnt = state.GetValidMovesForSquare(i, j, tMoves, true);
            for (int m = 0; m < moveCnt; m++)
            {
                var move = tMoves[m];
                evalMoves.Add((move, GameStateAnalyzer.Evaluate(move.NewState)));
            }
        }

        if (state.ToMove == Side.Black)
        {
            evalMoves.Sort((tuple, valueTuple) => tuple.Item2.CompareTo(valueTuple.Item2));
        }
        else
        {
            evalMoves.Sort((tuple, valueTuple) => valueTuple.Item2.CompareTo(tuple.Item2));
        }
        
        Span<(ChessMove, double)> optimalMoves = stackalloc (ChessMove, double)[depth - 1];
        foreach (var (move, mEval) in evalMoves)
        {
            var nEval = UncachedNegamax(move.NewState, depth - 1, ref optimalMoves, ref moves, alpha, beta, mEval);
            moves++;

            bool isMoreOptimal = false;
            
            if (toPlay == Side.White)
            {
                // maximize
                if (optimalVal < nEval) isMoreOptimal = true;
            }
            else
            {
                // minimize
                if (optimalVal > nEval) isMoreOptimal = true;
            }

            if (isMoreOptimal)
            {
                optimalVal = nEval;
                bestMoves[0] = (move, mEval);
                optimalMoves.CopyTo(bestMoves[1..]);
            }

            if (state.ToMove == Side.White)
                alpha = Math.Max(alpha, nEval);
            else
                beta = Math.Min(beta, nEval);
            if (beta <= alpha)
            {
                return optimalVal;
            }
        }
        return optimalVal;
    }
}