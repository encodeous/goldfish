using goldfish.Core.Data;
using goldfish.Core.Game;
using Terminal.Gui;

namespace goldfish_test.Console;

public static class ChessPrinter
{
    public static void PrintBoard(ChessState state, ChessMove? prevMove, Label[,] grid)
    {
        var dark = Color.DarkGray;
        var light = Color.Gray;
        var white = Color.White;
        var black = Color.Black;
        for (var i = 7; i >= 0; i--)
        {
            for (var j = 0; j < 8; j++)
            {
                var bg = (i + j) % 2 == 0 ? dark : light;
                var fg = state.GetPiece(i, j).GetSide() == Side.Black ? black : white;
                if (prevMove.HasValue)
                {
                    if (prevMove.Value.NewPos == (i, j) || prevMove.Value.OldPos == (i, j))
                    {
                        bg = Color.Green;
                    }
                }

                var nColor = Application.Driver.MakeColor(fg, bg);
                var lab = grid[i + 1, j + 1];
                if (lab.ColorScheme is null)
                    lab.ColorScheme = new ColorScheme();
                lab.ColorScheme.Normal = nColor;
                var p = state.GetPiece(i, j).GetPieceType();
                lab.Text = p switch
                {
                    PieceType.Space => " ",
                    PieceType.Knight => "N",
                    _ => p.ToString()[0].ToString()
                };
            }
        }
    }
    public static void PrintSelected(ChessState state, Label[,] grid, ChessMove[] moves)
    {
        var dark = Color.DarkGray;
        var light = Color.Gray;
        var white = Color.White;
        var black = Color.Black;
        for (var i = 7; i >= 0; i--)
        {
            for (var j = 0; j < 8; j++)
            {
                var bg = (i + j) % 2 == 0 ? dark : light;
                var fg = state.GetPiece(i, j).GetSide() == Side.Black ? black : white;
                if (moves.Any(x=>x.NewPos==(i, j)))
                {
                    bg = Color.Green;
                }

                var nColor = Application.Driver.MakeColor(fg, bg);
                var lab = grid[i + 1, j + 1];
                if (lab.ColorScheme is null)
                    lab.ColorScheme = new ColorScheme();
                lab.ColorScheme.Normal = nColor;
                var p = state.GetPiece(i, j).GetPieceType();
                lab.Text = p switch
                {
                    PieceType.Space => " ",
                    PieceType.Knight => "N",
                    _ => p.ToString()[0].ToString()
                };
            }
        }
    }
}