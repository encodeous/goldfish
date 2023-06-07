using goldfish.Core.Data;
using goldfish.Core.Game;
using goldfish.Engine.Analysis;

namespace goldfish.Engine;

public static class GoldFishEngine
{
    public static ChessMove SearchOptimalMove(this ChessState state, Side toPlay)
    {
        return new ChessMove();
    }

    private static double NegaMaxEvaluator(ChessState state, Side toPlay, int depth)
    {
        var eval = new GameStateAnalyzer(state);
        var evalV = eval.Evaluate();
        if (depth == 0 || double.IsPositiveInfinity(Math.Abs(evalV)))
        {
            return evalV;
        }

        var flipEval = toPlay == Side.Black ? -1 : 1;
        var optimize = double.NegativeInfinity;
        for (var i = 0; i < 8; i++)
        for (var j = 0; j < 8; j++)
        {
            var piece = state.GetPiece(i, j);
            if (piece.GetSide() != toPlay || piece.GetLogic() is null) continue;
            foreach (var move in state.GetValidMovesForSquare(i, j))
            {
                optimize = Math.Max(optimize, flipEval * NegaMaxEvaluator(move.NewState, toPlay.GetOpposing(), depth - 1));
            }
        }
        
        return flipEval * optimize;
    }
}