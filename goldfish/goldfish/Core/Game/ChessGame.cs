using goldfish.Core.Data;

namespace goldfish.Core.Game;

public class ChessGame
{
    private ChessState _currentState;

    public ChessGame()
    {
        _currentState = ChessState.DefaultState();
    }
}