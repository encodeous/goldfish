using Godot;

namespace chessium.scripts;

public partial class Utils : Node
{
    public static readonly Constants.GameMode gameMode = Constants.GameMode.LOCAL_MULTIPLAYER;
    public static Constants.Player playerTurn = Constants.Player.WHITE;
    public static Constants.Player otherPlayerTurn = Constants.Player.BLACK;

    public static bool soundEnabled = true;

    public static void SetPlayerTurn(Constants.Player player)
    {
        if (playerTurn != player)
        {
            SwapPlayerTurn();
        }
    }

    public static void SwapPlayerTurn()
    {
        otherPlayerTurn = playerTurn;
        playerTurn = playerTurn == Constants.Player.WHITE ? Constants.Player.BLACK : Constants.Player.WHITE;
    }

    public static bool IsInsideBoard(Vector2 location)
    {
        return !(location.X < 0 || location.X > 7 || location.Y < 0 || location.Y > 7);
    }

    public static string PlayerToString(Constants.Player player)
    {
        return player == Constants.Player.WHITE ? "white" : "black";
    }

    public static void PlaySound(AudioStreamPlayer audio)
    {
        if (soundEnabled)
        {
            audio.Play();
        }
    }
}