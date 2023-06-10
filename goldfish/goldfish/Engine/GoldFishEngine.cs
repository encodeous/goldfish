using System.Diagnostics;
using System.Runtime.InteropServices.ComTypes;
using goldfish.Core.Data;
using goldfish.Core.Data.Optimization;
using goldfish.Core.Game;
using goldfish.Engine.Analysis;
using goldfish.Core.Game.FEN;
using goldfish;

namespace goldfish.Engine;

public static class GoldFishEngine
{
    public static double NextOptimalMoves(ChessState state, int depth, ref Span<(ChessMove, double)> bestMoves, ref long moves, double alpha = double.NegativeInfinity, double beta = double.PositiveInfinity, double eval = double.NaN, StateEvaluationCache cache = null)

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

        var evalMoves = new List<(ChessMove, double, StateEvaluationCache)>();
        for (var i = 0; i < 8; i++)
        for (var j = 0; j < 8; j++)
        {
            var piece = state.GetPiece(i, j);
            if (piece.GetSide() != toPlay || piece.GetLogic() is null) continue;
            foreach (var move in state.GetValidMovesForSquare(i, j, true, cache))
            {
                var analyzer = new GameStateAnalyzer(move.NewState);
                evalMoves.Add((move, analyzer.Evaluate(), analyzer.Cache));
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
        foreach (var (move, mEval, mCache) in evalMoves)
        {
            var nEval = NextOptimalMoves(move.NewState, depth - 1, ref optimalMoves, ref moves, alpha, beta, mEval, mCache);
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