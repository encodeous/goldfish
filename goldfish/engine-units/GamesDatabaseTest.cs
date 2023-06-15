using System.Globalization;
using Chess;
using CsvHelper;
using goldfish.Core.Data;
using goldfish.Core.Game;
using CastleType = Chess.CastleType;
using PieceType = goldfish.Core.Data.PieceType;
using PromotionType = goldfish.Core.Data.PromotionType;

namespace engine_units;

public class GamesDatabaseTest
{
    [Fact]
    public void VerifyMoves()
    {
        using var reader = new StreamReader("../../../data/games.csv");
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        var records = csv.GetRecords<dynamic>();
        Span<ChessMove> tMoves = stackalloc ChessMove[32];
        foreach (var rec in records)
        {
            try
            {
                string moves = rec.moves;
                var board = new ChessBoard();
                foreach (var s in moves.Split(' '))
                {
                    board.Move(s);
                }

                var parsedMoves = board.ExecutedMoves;
                var state = ChessState.DefaultState();
                foreach (var move in parsedMoves)
                {
                    var (x, y) = (move.OriginalPosition.X, move.OriginalPosition.Y);
                    int moveCnt = state.GetValidMovesForSquare(y, x, tMoves);
                    for(int m = 0; m < moveCnt; m++)
                    {
                        var mov = tMoves[m];
                        if (
                            mov.NewPos == (move.NewPosition.Y, move.NewPosition.X) 
                            && (mov.Taken is null == move.CapturedPiece is null)
                            || (move.Parameter is MoveCastle && mov.IsCastle)
                        )
                        {
                            if((move.Parameter is MovePromotion && ((PieceType)Convert(((MovePromotion)move.Parameter).PromotionType)) != mov.Type)) continue;
                            if(move.Parameter is MoveCastle cast)
                            {
                                if (cast.CastleType == CastleType.Queen)
                                {
                                    if(mov.NewPos.Item2 > mov.OldPos.Item2) continue;
                                }
                                else
                                {
                                    if(mov.NewPos.Item2 < mov.OldPos.Item2) continue;
                                }
                            }
                            state = mov.NewState;
                            goto OK;
                        }
                    }
                    Assert.Fail($"F: {move.San} - {moves}");
                    OK: ;
                }
            }
            catch(ChessSanNotFoundException)
            {
                // ignored
            }
        }
    }

    private static PromotionType Convert(Chess.PromotionType type)
    {
        return type switch
        {
            Chess.PromotionType.Default => new PromotionType(),
            Chess.PromotionType.ToQueen => PromotionType.Queen,
            Chess.PromotionType.ToRook => PromotionType.Rook,
            Chess.PromotionType.ToBishop => PromotionType.Bishop,
            Chess.PromotionType.ToKnight => PromotionType.Knight,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
}