using goldfish_test.Console;
using goldfish.Core.Data;
using goldfish.Core.Game;
using Spectre.Console;

var state = ChessState.DefaultState();
bool isWhite = true;
ChessMove? cMove = null;

(int, int) GetSquare(string s)
{
    s = s.Trim().ToLower();
    return (s[1] - '1', s[0] - 'a');
}
while (true)
{
    while (true)
    {
        AnsiConsole.WriteLine("To play: " + (isWhite? "White" : "Black"));
        AnsiConsole.Write(state.PrintBoard(cMove));
        var mv = AnsiConsole.Ask<string>("Enter a move: ");
        var from = GetSquare(mv[..2]);
        var to = GetSquare(mv[2..]);
        var moves = state.GetValidMovesForSquare(from.Item1, from.Item2);
        foreach (var move in moves)
        {
            if (move.NewPos == to)
            {
                cMove = move;
                state = move.NewState;
                goto SKIP;
            }
        }
        AnsiConsole.Clear();
        AnsiConsole.WriteLine("Invalid Move.");
    }
    SKIP:
    AnsiConsole.Clear();
    isWhite = !isWhite;
}