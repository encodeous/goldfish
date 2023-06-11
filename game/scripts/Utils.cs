using Godot;

namespace chessium.scripts;

public partial class Utils : Node
{
    /// The current game mode of the game.
    public static readonly Constants.GameMode gameMode = Constants.GameMode.LOCAL_MULTIPLAYER;
    /// The color of the current player's turn.
    public static Constants.Player playerTurn = Constants.Player.WHITE;
    /// The other color of the player who's turn it isn't.
    public static Constants.Player otherPlayerTurn = Constants.Player.BLACK;

    /// If sounds are selected to be playable.
    public static bool soundEnabled = true;

    /// Sets the color of current player's turn.
    public static void SetPlayerTurn(Constants.Player player)
    {
        if (playerTurn != player)
        {
            SwapPlayerTurn();
        }
    }

    /// Swaps the turn of the players.
    public static void SwapPlayerTurn()
    {
        otherPlayerTurn = playerTurn;
        playerTurn = playerTurn == Constants.Player.WHITE ? Constants.Player.BLACK : Constants.Player.WHITE;
    }

    /// Checks if a vector is inside the game board.
    public static bool IsInsideBoard(Vector2 location)
    {
        return !(location.X < 0 || location.X > 7 || location.Y < 0 || location.Y > 7);
    }

    /// Stringifies the player's color.
    public static string PlayerToString(Constants.Player player)
    {
        return player == Constants.Player.WHITE ? "white" : "black";
    }

    /// Plays a sound, if enabled.
    public static void PlaySound(AudioStreamPlayer audio)
    {
        if (soundEnabled)
        {
            audio.Play();
        }
    }
}