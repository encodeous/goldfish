using goldfish.Core.Data;
using goldfish.Core.Game;
using goldfish.Core.Game.FEN;
using Spectre.Console;

namespace engine_test;

public class BoardPrinter
{
    public static Grid PrintBoard(in ChessState state, ChessMove? prevMove)
    {
        var grid = new Grid();
        for (int i = 0; i < 9; i++)
        {
            grid.AddColumn(new GridColumn()
            {
                Padding = new Padding(0)
            });
        }
        var dark = new Color(119, 149, 86);
        var light = new Color(178, 188, 160);
        var white = new Color(248, 248, 248);
        var black = new Color(86, 83, 82);
        grid.AddRow("-", "a", "b", "c", "d", "e", "f", "g", "h");
        for (var i = 7; i >= 0; i--)
        {
            var row = new Text[9];
            row[0] = new Text($"{i + 1}");
            for (var j = 0; j < 8; j++)
            {
                var bg = (i + j) % 2 == 0 ? dark : light;
                var fg = state.GetPiece(i, j).GetSide() == Side.Black ? black : white;
                if (prevMove.HasValue)
                {
                    if (prevMove.Value.NewPos == (i, j) || prevMove.Value.OldPos == (i, j))
                    {
                        bg = Color.Salmon1;
                    }
                }
                row[j + 1] = new Text(state.GetPiece(i, j).GetPieceType() switch
                    {
                        PieceType.Knight => "N",
                        PieceType.Space => " ",
                        _ => state.GetPiece(i, j).GetPieceType().ToString()[0].ToString()
                    },
                    new Style(fg, bg, Decoration.Bold));
            }

            grid.AddRow(row);
        }

        return grid;
    }

    public static void Print(Span<(ChessMove, double)> moves)
    {
        
        AnsiConsole.Write("----------------------------------------------------------------------");
        foreach (var (move, eval) in moves)
        {
            if(move.Equals(new ChessMove())) continue;
            Console.WriteLine($"Step -- E: {eval} --");
    
            Console.WriteLine($"{move.Type} to {move.NewPos}");

            AnsiConsole.Write(PrintBoard(in move.NewState, move));
    
            Console.WriteLine($"{FenConvert.ToFen(move.NewState)}");
        }
    }
}