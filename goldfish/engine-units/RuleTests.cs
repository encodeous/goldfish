using System.Text.Json.Nodes;
using engine_test;
using goldfish.Core.Data;
using goldfish.Core.Data.Optimization;
using goldfish.Core.Game;
using goldfish.Core.Game.FEN;

namespace engine_units;

public class RuleTests
{
    [Theory]
    [InlineData("castling")]
    [InlineData("famous")]
    [InlineData("pawns")]
    [InlineData("promotions")]
    [InlineData("standard")]
    [InlineData("taxing")]
    public void VerifyMoves(string test)
    {
        var tData = JsonNode.Parse(File.ReadAllText($"../../../data/{test}.json"));
        foreach (var caseNode in tData["testCases"].AsArray())
        {
            var startState = FenConvert.Parse(caseNode["start"]["fen"].ToString());
            var endStates = new HashSet<ulong>(caseNode["expected"].AsArray().Select(x 
                =>
            {
                BoardPrinter.PrintBoard(FenConvert.Parse(x["fen"].ToString()), null);
                return FenConvert.Parse(x["fen"].ToString()).Additional.Hash;
            }));
            Assert.Equal(endStates, GetAllMoves(startState));
        }
    }

    static HashSet<ulong> GetAllMoves(in ChessState state)
    {
        int cnt = 0;
        Span<ChessMove> tMoves = stackalloc ChessMove[30];
        var states = new HashSet<ulong>();
        for (var i = 0; i < 8; i++)
        for (var j = 0; j < 8; j++)
        {
            var piece = state.GetPiece(i, j);
            if (!piece.IsSide(state.ToMove) || piece.GetLogic() is null) continue;
            int moveCnt = state.GetValidMovesForSquare(i, j, tMoves);
            for(int m = 0; m < moveCnt; m++)
            {
                BoardPrinter.PrintBoard(tMoves[m].NewState, null);
                states.Add(tMoves[m].NewState.Additional.Hash);
            }
        }

        return states;
    }
}