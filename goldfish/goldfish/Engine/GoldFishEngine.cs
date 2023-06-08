using System.Diagnostics;
using goldfish.Core.Data;
using goldfish.Core.Game;
using goldfish.Engine.Analysis;

namespace goldfish.Engine;

public static class GoldFishEngine
{
    public static (double, ChessMove?) NextOptimalMove(ChessState state, int depth, double alpha, double beta, ChessMove? curMove = default)
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
        
        var flipEval = toPlay == Side.Black ? -1 : 1;
        var optimize = double.NegativeInfinity;
        for (var i = 0; i < 8; i++)
        for (var j = 0; j < 8; j++)
        {
            var piece = state.GetPiece(i, j);
            if (piece.GetSide() != toPlay || piece.GetLogic() is null) continue;
            foreach (var move in state.GetValidMovesForSquare(i, j, eval.Cache))
            {
                var curEval = NextOptimalMove(move.NewState, depth - 1, alpha, beta, move);
                if (curEval.Item1 * flipEval >= optimize)
                {
                    optimize = curEval.Item1 * flipEval;
                    if (curMove == null)
                    {
                        Debug.WriteLine($"{piece.GetPieceType()}>{(i, j)} to {(move.NewPos)} @ {optimize}");
                    }
                    topEvalMove = move;
                }
                // if (state.ToMove == Side.White)
                // {
                //     alpha = Math.Max(alpha, optimize);
                //     if (beta <= alpha)
                //     {
                //         return (flipEval * curEval.Item1, move);
                //     }
                // }
                // else
                // {
                //     beta = Math.Min(beta, -optimize);
                //     if (beta <= alpha)
                //     {
                //         return (flipEval * curEval.Item1, move);
                //     }
                // }
            }
        }
        
        return (flipEval * optimize, topEvalMove);
    }
}