using goldfish.Core.Data;
using goldfish.Core.Data.Optimization;
using goldfish.Core.Game;
using goldfish.Engine.Analysis;

namespace goldfish.Engine;

public static class GoldFishEngine
{
    public static double NextOptimalMoves(ChessState state, int depth, ref Span<(ChessMove, double)> bestMoves, ref ulong positions, double alpha = double.NegativeInfinity, double beta = double.PositiveInfinity, double staticEval = double.NaN)

    {
        // alpha is white, beta is black
        if (depth == 0 || double.IsPositiveInfinity(Math.Abs(staticEval)))
        {
            return staticEval;
        }

        ref var cache = ref Tst.Get(state);
        if (!double.IsNaN(staticEval) && !double.IsNaN(cache.EngineEval) && cache.EvalDepth <= depth)
        {
            positions += cache.Positions;
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

        Span<(ChessMove, double)> evalMoves = stackalloc (ChessMove, double)[32 * 30]; // should be plenty... i think
        Span<ChessMove> tMoves = stackalloc ChessMove[30];
        int cnt = 0;
        for (var i = 0; i < 8; i++)
        for (var j = 0; j < 8; j++)
        {
            var piece = state.GetPiece(i, j);
            if (!piece.IsSide(toPlay) || piece.GetLogic() is null) continue;
            int moveCnt = state.GetValidMovesForSquare(i, j, tMoves);
            for(int m = 0; m < moveCnt; m++)
            {
                var move = tMoves[m];
                evalMoves[cnt++] = (move, GameStateAnalyzer.Evaluate(move.NewState));
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
            var nEval = NextOptimalMoves(move.NewState, depth - 1, ref optimalMoves, ref positions, alpha, beta, mEval);
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
                positions += curMoves;
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
        positions += curMoves;
        return optimalVal;
    }
}