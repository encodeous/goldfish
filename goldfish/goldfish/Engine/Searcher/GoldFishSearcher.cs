using System.Collections.Concurrent;
using goldfish.Core.Data;
using goldfish.Core.Data.Optimization;
using goldfish.Core.Game;
using goldfish.Engine.Analysis;

namespace goldfish.Engine.Searcher;

public class GoldFishSearcher
{
    private int _threads;
    private TimeSpan _allottedTime;

    public GoldFishSearcher(TimeSpan allottedTime, int threads = -1)
    {
        if (threads == -1)
        {
            threads = 4;
        }
        _threads = threads;
        _allottedTime = allottedTime;
    }

    public delegate void SearchUpdateDelegate(SearchResult result);

    public event SearchUpdateDelegate? SearchUpdate;

    public SearchResult StartSearch(ChessState state, CancellationToken ct = default)
    {
        var optimalVal = state.ToMove == Side.White ?
            // maximize
            double.NegativeInfinity :
            // minimize
            double.PositiveInfinity;
        ChessMove optimalMove = default;
        // iterative deepening
        var cts = new CancellationTokenSource(_allottedTime);
        ct.Register(() => cts.Cancel());
        int maxDepth = 0;
        for (int depth = 2; depth < 100; depth++)
        {
            var result = ParallelSearch(state, depth, cts.Token);
            if (!cts.IsCancellationRequested)
            {
                var (mEval, move, _) = result;

                bool isMoreOptimal = false;

                if (state.ToMove == Side.White)
                {
                    // maximize
                    if (optimalVal < mEval) isMoreOptimal = true;
                }
                else
                {
                    // minimize
                    if (optimalVal > mEval) isMoreOptimal = true;
                }

                if (isMoreOptimal)
                {
                    optimalVal = mEval;
                    optimalMove = move;
                }

                SearchUpdate?.Invoke(new SearchResult(optimalVal, optimalMove, depth));
                maxDepth = depth;
            }
            else break;
        }

        return new SearchResult(optimalVal, optimalMove, maxDepth);
    }

    public SearchResult ParallelSearch(ChessState state, int depth, CancellationToken ct)
    {
        var toPlay = state.ToMove;

        var optimalVal = toPlay == Side.White
            ?
            // maximize
            double.NegativeInfinity
            :
            // minimize
            double.PositiveInfinity;
        ChessMove optimalMove = default;

        var jobs = new List<SearchJob>();
        var tMoves = new ChessMove[32];
        for (var i = 0; i < 8; i++)
        for (var j = 0; j < 8; j++)
        {
            var piece = state.GetPiece(i, j);
            if (!piece.IsSide(toPlay)) continue;
            int moveCnt = state.GetValidMovesForSquare(i, j, tMoves);
            for (int m = 0; m < moveCnt; m++)
            {
                var move = tMoves[m];
                jobs.Add(new SearchJob(move, depth - 1, GameStateAnalyzer.Evaluate(move.NewState)));
            }
        }

        var bag = new ConcurrentBag<SearchResult>();
        try
        {
            Parallel.ForEach(jobs, new ParallelOptions()
            {
                MaxDegreeOfParallelism = _threads,
                CancellationToken = ct
            }, (job, token) =>
            {
                if (token.ShouldExitCurrentIteration || ct.IsCancellationRequested) return;
                var arr = new (ChessMove, double)[job.Depth];
                Span<(ChessMove, double)> optMoves = arr;
                var optimizedEval = GoldFishEngine.NextOptimalMoves(job.Move.NewState, job.Depth, ref optMoves, ct,
                    staticEval: job.Eval);
                bag.Add(new SearchResult(optimizedEval, job.Move, job.Depth));
            });
        }catch(OperationCanceledException){}
        (ChessMove, double)? lastMove = null;
        foreach (var res in bag)
        {
            var (mEval, move, _) = res;
            lastMove = (move, mEval);
            bool isMoreOptimal = false;

            if (toPlay == Side.White)
            {
                // maximize
                if (optimalVal < mEval) isMoreOptimal = true;
            }
            else
            {
                // minimize
                if (optimalVal > mEval) isMoreOptimal = true;
            }

            if (isMoreOptimal)
            {
                optimalVal = mEval;
                optimalMove = move;
            }
        }
        if (double.IsInfinity(optimalVal) && lastMove is not null)
        {
            return new SearchResult(lastMove.Value.Item2, lastMove.Value.Item1, depth);
        }
        return new(optimalVal, optimalMove, depth);
    }

    private record SearchJob(ChessMove Move, int Depth, double Eval);

    public record SearchResult(double EngineEval, ChessMove BestMove, int Depth);
}