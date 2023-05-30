using goldfish.Core.Data;

namespace goldfish.Core.Game;

public class ChessGame
{
    private ChessState currentState;

    public ChessGame()
    {
        currentState = ChessState.DefaultState();
    }
}