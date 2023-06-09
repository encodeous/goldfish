using System.Diagnostics;
using System.Runtime.InteropServices.ComTypes;
using goldfish.Core.Data;
using goldfish.Core.Data.Optimization;
using goldfish.Core.Game;
using goldfish.Engine.Analysis;
using goldfish.Core.Game.FEN;

namespace goldfish.Engine;

public static class GoldFishEngine
{
    public static double NextOptimalMove(ChessState state, int depth, out ChessMove bestMove, double alpha = double.NegativeInfinity, double beta = double.PositiveInfinity, double eval = double.NaN, StateEvaluationCache cache = null)
    {
        bestMove = new ChessMove();
        // alpha is white, beta is black
        ChessMove? topEvalMove = null;
        if (depth == 0 || double.IsPositiveInfinity(Math.Abs(eval)))
        {
            return eval;
        }

        var toPlay = state.ToMove;

        Func<double, double, double> optimizer;
        double optimalVal;
        
        if (toPlay == Side.White)
        {
            // maximize
            optimalVal = double.NegativeInfinity;
            optimizer = Math.Max;
        }
        else
        {
            // minimize
            optimalVal = double.PositiveInfinity;
            optimizer = Math.Min;
        }

        var evalMoves = new List<(ChessMove, double, StateEvaluationCache)>();
        for (var i = 0; i < 8; i++)
        for (var j = 0; j < 8; j++)
        {
            var piece = state.GetPiece(i, j);
            if (piece.GetSide() != toPlay || piece.GetLogic() is null) continue;
            foreach (var move in state.GetValidMovesForSquare(i, j, cache))
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
        foreach (var (move, mEval, mCache) in evalMoves)
        {
            var nEval = NextOptimalMove(move.NewState, depth - 1, out _, alpha, beta, mEval, mCache);
            // Debug.WriteLine($"{nEval} - {FenConvert.ToFen(move.NewState)}");
            optimalVal = optimizer(nEval, optimalVal);
            if (optimalVal == nEval)
            {
                // if (curMove == null)
                // {
                //     Debug.WriteLine($"{piece.GetPieceType()}>{(i, j)} to {(move.NewPos)} @ {optimalVal}");
                // }
                topEvalMove = move;
                if (double.IsNaN(eval))
                {
                    bestMove = topEvalMove.Value;
                }
            }

            if (state.ToMove == Side.White)
                alpha = Math.Max(alpha, nEval);
            else
                beta = Math.Min(beta, nEval);
            if (beta <= alpha)
                return optimalVal;
        }
        return optimalVal;
    }
}