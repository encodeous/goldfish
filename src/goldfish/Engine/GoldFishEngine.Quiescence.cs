using goldfish.Core.Data;
using goldfish.Core.Data.Optimization;
using goldfish.Core.Game;
using goldfish.Engine.Analysis;
using goldfish.Engine.Analysis.Analyzers;

namespace goldfish.Engine;

public static partial class GoldFishEngine
{
    // private static double QuiescenceSearch(in ChessState state, int depth, CancellationToken ct = default, double alpha = double.NegativeInfinity, double beta = double.PositiveInfinity)
    // {
    //     if (ct.IsCancellationRequested) return 0;
    //     ref var cache = ref Tst.Get(state);
    //     if (!double.IsNaN(cache.QuiesceEval) && cache.QuiesceDepth >= depth)
    //     {
    //         return cache.QuiesceEval;
    //     }
    //
    //     double staticEval = GameStateAnalyzer.Evaluate(state);
    //     if (depth == 0) return staticEval; // forcefully limit depth
    //     if (staticEval >= beta)
    //         return beta;
    //     if (alpha < staticEval)
    //         alpha = staticEval;
    //     Span<ChessMove> tMoves = stackalloc ChessMove[30];
    //     var toPlay = state.ToMove;
    //     for (var i = 0; i < 8; i++)
    //     for (var j = 0; j < 8; j++)
    //     {
    //         var piece = state.GetPiece(i, j);
    //         if (!piece.IsSide(toPlay)) continue;
    //         int moveCnt = state.GetValidMovesForSquare(i, j, tMoves);
    //         for(int m = 0; m < moveCnt; m++)
    //         {
    //             if (ct.IsCancellationRequested) return 0;
    //             var move = tMoves[m];
    //             if (move.Taken is not null)
    //             {
    //                 var (x, y) = move.Taken.Value;
    //                 int cScore = MaterialAnalyzer.ScorePiece(piece.GetPieceType());
    //                 int capScore = MaterialAnalyzer.ScorePiece(state.GetPiece(x, y).GetPieceType());
    //                 double score;
    //                 if (state.Pieces > EndGamePieces) // exclude endgames
    //                 {
    //                     if (capScore - cScore >= 4)
    //                     {
    //                         // most definitely a good capture
    //                         score = GameStateAnalyzer.Evaluate(move.NewState);
    //                         goto SKIP;
    //                     }
    //                     // if(GetDefenderScore(move.NewState, x, y) < cScore) continue;
    //                 }
    //                 score = -QuiescenceSearch(move.NewState, depth - 1, ct, -beta, -alpha);
    //                 SKIP:
    //                 if (score >= beta)
    //                     return beta;
    //                 if (score > alpha)
    //                     alpha = score;
    //             }
    //         }
    //     }
    //     cache = ref Tst.Get(state);
    //     cache.QuiesceEval = alpha;
    //     cache.QuiesceDepth = depth;
    //     return alpha;
    // }

    private static int GetDefenderScore(in ChessState state, int r, int c)
    {
        int minScore = 1000;
        Span<ChessMove> tMoves = stackalloc ChessMove[32];
        for (var i = 0; i < 8; i++)
        for (var j = 0; j < 8; j++)
        {
            var piece = state.GetPiece(i, j);
            if (!piece.IsSide(state.ToMove)) continue;
            int moveCnt = state.GetValidMovesForSquare(i, j, tMoves);
            for(int m = 0; m < moveCnt; m++)
            {
                var move = tMoves[m];
                if (move.Taken is not null && move.Taken == (r, c))
                {
                    minScore = Math.Min(minScore, MaterialAnalyzer.ScorePiece(piece.GetPieceType()));
                }
            }
        }

        return minScore;
    }
}