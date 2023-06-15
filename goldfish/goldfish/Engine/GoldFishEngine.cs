using goldfish.Core.Data;
using goldfish.Core.Data.Optimization;
using goldfish.Core.Game;
using goldfish.Engine.Analysis;

namespace goldfish.Engine;

public static partial class GoldFishEngine
{
    public static double NextOptimalMoves(ChessState state, int depth, ref Span<(ChessMove, double)> bestMoves, CancellationToken ct = default, double alpha = double.NegativeInfinity, double beta = double.PositiveInfinity, double staticEval = double.NaN)

    {
        if (ct.IsCancellationRequested) return 0;
        // alpha is white, beta is black
        if (depth == 0 || double.IsPositiveInfinity(Math.Abs(staticEval)))
        {
            return GameStateAnalyzer.Evaluate(state);
        }

        ref var cache = ref Tst.Get(state);
        if (!double.IsNaN(cache.EngineEval) && cache.EvalDepth > depth)
        {
            return cache.EngineEval;
        }

        var toPlay = state.ToMove;
        (ChessMove, double)? lastMove = null;

        double optimalVal;

        
        optimalVal = toPlay == Side.White ? 
            // maximize
            double.NegativeInfinity :
            // minimize
            double.PositiveInfinity;

        Span<(ChessMove, double)> evalMoves = stackalloc (ChessMove, double)[state.Pieces * 32]; // should be plenty... i think
        Span<ChessMove> tMoves = stackalloc ChessMove[33];
        int cnt = 0;
        for (var i = 0; i < 8; i++)
        for (var j = 0; j < 8; j++)
        {
            var piece = state.GetPiece(i, j);
            if (!piece.IsSide(toPlay)) continue;
            int moveCnt = state.GetValidMovesForSquare(i, j, tMoves);
            for(int m = 0; m < moveCnt; m++)
            {
                if (ct.IsCancellationRequested) return 0;
                var move = tMoves[m];
                double eval;
                ref var nCache = ref Tst.Get(move.NewState);
                if (!double.IsNaN(nCache.EngineEval))
                {
                    eval = nCache.EngineEval;
                }else if (!double.IsNaN(nCache.StaticEval))
                {
                    eval = nCache.StaticEval;
                }
                else
                {
                    eval = GameStateAnalyzer.Evaluate(move.NewState);
                }

                if (nCache.PV)
                {
                    evalMoves[cnt++] = (move, -optimalVal);
                }
                else
                {
                    evalMoves[cnt++] = (move, eval);
                }
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
        for (int i = 0; i < cnt; i++)
        {
            var (move, mEval) = evalMoves[i];
            var nEval = NextOptimalMoves(move.NewState, depth - 1, ref optimalMoves, ct, alpha, beta, mEval);
            lastMove = (move, mEval);

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
                if (double.IsInfinity(optimalVal))
                {
                    bestMoves[0] = lastMove.Value;
                }
                return optimalVal;
            }
        }
        if (double.IsInfinity(optimalVal) && lastMove is not null)
        {
            bestMoves[0] = lastMove.Value;
        }
        cache = ref Tst.Get(state);
        cache.EngineEval = optimalVal;
        cache.EvalDepth = depth;
        ref var pvCache = ref Tst.Get(bestMoves[0].Item1.NewState);
        pvCache.PV = true;
        return optimalVal;
    }
}