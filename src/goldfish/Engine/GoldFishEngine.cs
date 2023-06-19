using goldfish.Core.Data;
using goldfish.Core.Data.Optimization;
using goldfish.Core.Game;
using goldfish.Engine.Analysis;
using goldfish.Engine.Analysis.Analyzers;

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

        // ref var cache = ref Tst.Get(state);
        // if (!double.IsNaN(cache.EngineEval) && cache.EvalDepth > depth)
        // {
        //     return cache.EngineEval;
        // }

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
        ref var n1Cache = ref Tst.Get(state);
        n1Cache.EngineEval = optimalVal;
        n1Cache.EvalDepth = depth;
        ref var pvCache = ref Tst.Get(bestMoves[0].Item1.NewState);
        pvCache.PV = true;
        return optimalVal;
    }
    
    
    public static (double, int) EngineEval(ChessState state, int depth, CancellationToken ct = default, double alpha = double.NegativeInfinity, double beta = double.PositiveInfinity)

    {
        if (ct.IsCancellationRequested) return (0, 0);
        var staticEval = GameStateAnalyzer.Evaluate(state);
        if (Math.Abs(staticEval) >= WinAnalyzer.CheckmateWeighting)
        {
            return (staticEval, 0);
        }
        // alpha is white, beta is black
        if (depth == 0 || double.IsPositiveInfinity(Math.Abs(staticEval)))
        {
            return (staticEval, 1000);
        }

        ref var cache = ref Tst.Get(state);
        if (!double.IsNaN(cache.EngineEval) && cache.EvalDepth > depth)
        {
            return (cache.EngineEval, cache.Moves);
        }

        var toPlay = state.ToMove;
        (ChessMove, double)? lastMove = null;
        ChessMove? bestMove = null;

        double optimalVal;

        optimalVal = toPlay == Side.White ? 
            // maximize
            double.NegativeInfinity :
            // minimize
            double.PositiveInfinity;

        int moves = 10000;
        
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
                if (ct.IsCancellationRequested) return (0, 10000);
                var move = tMoves[m];
                ref var nCache = ref Tst.Get(move.NewState);
                var eval = GameStateAnalyzer.Evaluate(move.NewState);

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
        
        for (int i = 0; i < cnt; i++)
        {
            var (move, mEval) = evalMoves[i];
            var (nEval, mov) = EngineEval(move.NewState, depth - 1, ct, alpha, beta);
            mov++;
            lastMove = (move, nEval);

            var isMoreOptimal = false;
            
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

            bool isWin = (state.ToMove == Side.White && nEval >= WinAnalyzer.CheckmateWeighting)
                         || (state.ToMove == Side.Black && nEval <= -WinAnalyzer.CheckmateWeighting);

            if (isWin)
            {
                if (moves >= mov)
                {
                    optimalVal = nEval;
                    bestMove = move;
                    moves = mov;
                }
            }
            else if (isMoreOptimal)
            {
                optimalVal = nEval;
                bestMove = move;
                moves = mov;
            }

            if (state.ToMove == Side.White)
                alpha = Math.Max(alpha, nEval);
            else
                beta = Math.Min(beta, nEval);
            if (beta <= alpha)
            {
                if (double.IsInfinity(optimalVal))
                {
                    optimalVal = lastMove.Value.Item2;
                }
                return (optimalVal, moves);
            }
        }
        if (double.IsInfinity(optimalVal))
        {
            if(lastMove is not null) 
                optimalVal = lastMove.Value.Item2;
            else
                optimalVal = GameStateAnalyzer.Evaluate(state);
        }
        ref var n1Cache = ref Tst.Get(state);
        n1Cache.EngineEval = optimalVal;
        n1Cache.EvalDepth = depth;
        n1Cache.Moves = moves;
        if (bestMove is not null)
        {
            ref var pvCache = ref Tst.Get(bestMove.Value.NewState);
            pvCache.PV = true;
        }
        return (optimalVal, moves);
    }
}