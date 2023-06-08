using System.Diagnostics;
using goldfish.Core.Data;
using goldfish.Core.Game;
using goldfish.Engine.Analysis;
using goldfish.Core.Game.FEN;

namespace goldfish.Engine;

public static class GoldFishEngine
{
    public static (double, ChessMove?) NextOptimalMove(ChessState state, int depth, double alpha, double beta, int dbgHash = 0, ChessMove? curMove = default)
    {
        // alpha is white, beta is black
        var eval = new GameStateAnalyzer(state);
        var evalV = eval.Evaluate();
        ChessMove? topEvalMove = null;
        if (depth == 0 || double.IsPositiveInfinity(Math.Abs(evalV)))
        {
            return (evalV, curMove);
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
        
        for (var i = 0; i < 8; i++)
        for (var j = 0; j < 8; j++)
        {
            var piece = state.GetPiece(i, j);
            if (piece.GetSide() != toPlay || piece.GetLogic() is null) continue;
            foreach (var move in state.GetValidMovesForSquare(i, j, eval.Cache))
            {
                var (nEval, nMove) = NextOptimalMove(move.NewState, depth - 1, alpha, beta, dbgHash, move);
                Debug.WriteLine($"{nEval} - {FenConvert.ToFen(move.NewState)}");
                optimalVal = optimizer(nEval, optimalVal);
                if (optimalVal == nEval)
                {
                    if (curMove == null)
                    {
                        Debug.WriteLine($"{piece.GetPieceType()}>{(i, j)} to {(move.NewPos)} @ {optimalVal}");
                    }
                    topEvalMove = nMove;
                }

                if (state.ToMove == Side.White)
                    alpha = Math.Max(alpha, nEval);
                else
                    beta = Math.Min(beta, nEval);
                if (beta <= alpha)
                    return (optimalVal, topEvalMove);
            }
        }

        return (optimalVal, topEvalMove);
    }
}