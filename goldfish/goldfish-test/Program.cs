using System.Diagnostics;
using goldfish_test.Console;
using goldfish.Core.Data;
using goldfish.Core.Game;
using goldfish.Core.Game.FEN;
using goldfish.Engine;
using Terminal.Gui;
using Attribute = Terminal.Gui.Attribute;

internal class Program
{
    private static ChessGame game = new ChessGame();
    private static ChessMove[]? _selMoves;
    private static MenuBarItem goldFishToggle;
    public static void Main(string[] args)
    {
        Application.Init();
        var grid = new Label[9, 9];
        var wnd = new Window()
        {
            X = Pos.Center(),
            Y = Pos.Center(),
            Width = 20,
            Height = 12,
        };
        var chessBoardView = new View()
        {
            X = Pos.Center(), Y = 1,
            Height = 9,
            Width = 9
        };
        var stateLabel = new Label("White's Turn");
        wnd.Add(
            stateLabel
        );
        grid[0, 0] = new Label(" ");
// add place names
        for (var i = 1; i <= 8; i++)
        {
            grid[0, i] = new Label((char)('a' + i - 1) + "")
            {
                X = i,
                Y = 0
            };
            grid[8 - i + 1, 0] = new Label(i + "")
            {
                X = 0,
                Y = 8 - i + 1
            };
        }
        for (var i = 1; i <= 8; i++)
        {
            for (var j = 1; j <= 8; j++)
            {
                var lab = grid[i, j] = new Label()
                {
                    X = j,
                    Y = 8-i+1
                };
                var x = i;
                var y = j;
                lab.Clicked += () =>
                {
                    var p = game.CurrentState.GetPiece(x - 1, y - 1);
                    if (p.GetPieceType() != PieceType.Space && p.GetSide() == game.CurrentState.ToMove)
                    {
                        var moves = game.CurrentState.GetValidMovesForSquare(x - 1, y - 1).ToArray();
                        _selMoves = moves;
                        ChessPrinter.PrintSelected(game.CurrentState, grid, _selMoves);
                    }
                    else if (_selMoves is not null && _selMoves.Any(m => m.NewPos == (x - 1, y - 1)))
                    {
                        // do the move
                        var move = _selMoves.First(m => m.NewPos == (x - 1, y - 1));
                        game.Commit();
                        game.CurrentState = move.NewState;
                        game.LastMove = move;
                        stateLabel.Text = $"{game.CurrentState.ToMove}'s Turn";
                        while (move.IsPromotion)
                        {
                            var sel = MessageBox.Query("Promotion", "Choose one of the following to promote to: ",
                                "Rook", "Knight",
                                "Bishop", "Queen");
                            PromotionType? promType = sel switch
                            {
                                1 => PromotionType.Knight,
                                0 => PromotionType.Rook,
                                2 => PromotionType.Bishop,
                                3 => PromotionType.Queen,
                                _ => null
                            };
                            if (!promType.HasValue) continue;
                            game.CurrentState.Promote(move.NewPos, promType.Value);
                            break;
                        }
                        var gState = game.CurrentState.GetGameState();
                        if (gState is not null)
                        {
                            if (gState.Value == Side.None)
                            {
                                MessageBox.Query("Stalemate", $"The game has ended in a draw.", "Restart Game");
                            }
                            else
                            {
                                MessageBox.Query("Checkmate", $"{gState.Value} has won by checkmate", "Restart Game");
                            }
                            game.Reset();
                        }
                        else
                        {
                            // let engine play
                            if (game.IsEngineActive)
                            {
                                game.Commit();
                                var nextMove = GoldFishEngine.NextOptimalMove(game.CurrentState, 4);
                                Debug.WriteLine($"Move calc w/ eval of {nextMove.Item1}");
                                game.LastMove = nextMove.Item2;
                                game.CurrentState = nextMove.Item2.Value.NewState;
                                var gState2 = game.CurrentState.GetGameState();
                                if (gState2 is not null)
                                {
                                    if (gState2.Value == Side.None)
                                    {
                                        MessageBox.Query("Stalemate", $"The game has ended in a draw.", "Restart Game");
                                    }
                                    else
                                    {
                                        MessageBox.Query("Checkmate", $"{gState2.Value} has won by checkmate", "Restart Game");
                                    }
                                    game.Reset();
                                }
                            }
                        }
                        ChessPrinter.PrintBoard(game.CurrentState, game.LastMove, grid);
                        _selMoves = null;
                    }
                };
            }
        }

        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                chessBoardView.Add(grid[i, j]);
            }
        }

        ChessPrinter.PrintBoard(game.CurrentState, game.LastMove, grid);

        wnd.Add(chessBoardView);
            
        goldFishToggle = new MenuBarItem("_GoldFish Active", "", () =>
        {
            game.IsEngineActive = !game.IsEngineActive;
            goldFishToggle.Title = game.IsEngineActive ? "_GoldFish Active" : "_GoldFish Inactive";
        });
        
        var menu = new MenuBar(
            new MenuBarItem[]
            {
                goldFishToggle,
                new("_Game", new[]
                {
                    new MenuItem("_Restart", "", () =>
                    {
                        game.Reset();
                        ChessPrinter.PrintBoard(game.CurrentState, game.LastMove, grid);
                    }),
                    new MenuItem("_Quit", "", () =>
                    {
                        Application.RequestStop();
                    }),
                }),
                new ("_Load FEN", "", () =>
                {
                    var load = new Button("Load");
                    var fenField = new TextField()
                    {
                        X = 0,
                        Y = 1,
                        Width = 50
                    };
                    var dialog = new Dialog ("Load game from FEN", load)
                    {
                        Height = 6,
                        Width = 52
                    };
                    dialog.Add(fenField);
                    var k = () =>
                    {
                        try
                        {
                            var state = FenParser.Parse(fenField.Text.ToString());
                            game.Reset();
                            game.CurrentState = state;
                            ChessPrinter.PrintBoard(game.CurrentState, game.LastMove, grid);
                            Application.RequestStop();
                        }
                        catch
                        {
                            MessageBox.Query("Invalid FEN", "The specified FEN was not valid, try again!", "Ok");
                        }
                    };
                    ;
                    load.Clicked += k;
                    fenField.KeyDown += eventArgs =>
                    {
                        if (eventArgs.KeyEvent.Key == Key.Enter) k();
                    };
                    Application.Run(dialog);
                }),
                new("_Undo", "", () =>
                {
                    game.Rollback();
                    ChessPrinter.PrintBoard(game.CurrentState, game.LastMove, grid);
                    stateLabel.Text = $"{game.CurrentState.ToMove}'s Turn";
                }),
            });
        menu.Text = "GoldFish Engine ALPHA";
        
        Application.Top.Add (menu, wnd);
        Application.Run();
        Application.Shutdown();
    }
}
// while (true)
// {
//     while (true)
//     {
//         AnsiConsole.WriteLine("To play: " + (isWhite? "White" : "Black"));
//         AnsiConsole.Write(state.PrintBoard(cMove));
//         var mv = AnsiConsole.Ask<string>("Enter a move: ");
//         var from = GetSquare(mv[..2]);
//         var to = GetSquare(mv[2..]);
//         var moves = state.GetValidMovesForSquare(from.Item1, from.Item2);
//         foreach (var move in moves)
//         {
//             if (move.NewPos == to)
//             {
//                 cMove = move;
//                 state = move.NewState;
//                 if (move.IsPromotion)
//                 {
//                     while (true)
//                     {
//                         var promo = char.ToLower(AnsiConsole.Ask<char>("Promotion (K, R, B, Q): "));
//                         PromotionType? promType = promo switch
//                         {
//                             'k' => PromotionType.Knight,
//                             'r' => PromotionType.Rook,
//                             'b' => PromotionType.Bishop,
//                             'q' => PromotionType.Queen,
//                             _ => null
//                         };
//                         if (promType.HasValue)
//                         {
//                             state.Promote(move.NewPos, promType.Value);
//                             break;
//                         }
//                         AnsiConsole.WriteLine("Invalid Choice.");
//                     }
//                 }
//                 goto SKIP;
//             }
//         }
//         AnsiConsole.Clear();
//         AnsiConsole.WriteLine("Invalid Move.");
//     }
//     SKIP:
//     AnsiConsole.Clear();
//     isWhite = !isWhite;
// }