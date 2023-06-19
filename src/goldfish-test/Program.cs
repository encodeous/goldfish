using System.Diagnostics;
using goldfish_test.Console;
using goldfish.Core.Data;
using goldfish.Core.Game;
using goldfish.Core.Game.FEN;
using goldfish.Engine;
using goldfish.Engine.Searcher;
using Terminal.Gui;
using Attribute = Terminal.Gui.Attribute;

internal class Program
{
    private static ChessGame game = new ChessGame();
    private static ArraySegment<ChessMove> _selMoves;
    private static MenuBarItem goldFishToggle;

    private static GoldFishSearcher _searcher = new(TimeSpan.FromSeconds(6), 6, (int)(Environment.ProcessorCount / 1.2));
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

        void RunEngine()
        {
            stateLabel.Text = $"Thinking...";
            Task.Run(() =>
            {
                game.Commit();
                // Span<(ChessMove, double)> moves = new (ChessMove, double)[6];
                // var eval = _searcher.ParallelSearch(game.CurrentState, 6, CancellationToken.None);
                // game.LastMove = eval.BestMove;
                // game.CurrentState = eval.BestMove.NewState;
                // var gState2 = game.CurrentState.GetGameState();
                var res = _searcher.StartSearch(game.CurrentState);
                Debug.WriteLine($"Move calc w/ eval of {res.EngineEval} - depth {res.Depth}");
                game.LastMove = res.BestMove;
                game.CurrentState = res.BestMove.NewState;
                var gState2 = game.CurrentState.GetGameState();
                Application.MainLoop.Invoke(() =>
                {
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

                    stateLabel.Text = $"{game.CurrentState.ToMove}'s Turn";
                    ChessPrinter.PrintBoard(game.CurrentState, game.LastMove, grid);
                });
            });
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
                        var tempArr = new ChessMove[30];
                        var moves = game.CurrentState.GetValidMovesForSquare(x - 1, y - 1, tempArr, false);
                        _selMoves = tempArr[..moves];
                        ChessPrinter.PrintSelected(game.CurrentState, grid, _selMoves);
                    }
                    else if (_selMoves.Count != 0 && _selMoves.Any(m => m.NewPos == (x - 1, y - 1)))
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
                            if (promType is null) continue;
                            game.CurrentState.Promote(move.NewPos, promType.Value);
                            break;
                        }
                        ChessPrinter.PrintBoard(game.CurrentState, game.LastMove, grid);
                        _selMoves = null;
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
                                RunEngine();
                            }
                        }
                    }
                };
            }
        }

        _searcher.SearchUpdate += result =>
        {
            Application.MainLoop.Invoke(() =>
            {
                stateLabel.Text = $"Depth: {result.Depth}";
            });
        };
        
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
            ChessPrinter.PrintBoard(game.CurrentState, game.LastMove, grid);
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
                            var state = FenConvert.Parse(fenField.Text.ToString());
                            game.Reset();
                            game.CurrentState = state;
                            stateLabel.Text = $"{game.CurrentState.ToMove}'s Turn";
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
                new ("_Go Fish!", "", RunEngine),
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