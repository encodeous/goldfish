using goldfish.Core.Data;

namespace goldfish.Core.Game;

public class ChessGame
{
    public ChessState CurrentState;
    public ChessMove? LastMove;
    public Stack<ChessState> States;

    public ChessGame()
    {
        Reset();
    }

    public void Reset()
    {
        CurrentState = ChessState.DefaultState();
        LastMove = null;
        States = new Stack<ChessState>();
    }

    public void Commit()
    {
        States.Push(CurrentState);
    }

    public void Rollback()
    {
        if (!States.Any()) return;
        var nState = States.Pop();
        CurrentState = nState;
        LastMove = null;
    }
}