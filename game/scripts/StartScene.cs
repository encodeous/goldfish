using Godot;

namespace chessium.scripts;

public partial class StartScene : Control
{
	/// The color of the first player.
	public static string chosenColor = "random";
	
	/// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetNode<Button>("FullscreenButton").Connect("button_up", Callable.From(OnFullscreenButtonUp));
		GetNode<Button>("Panel/StartButton").Connect("button_up", Callable.From(OnStartButtonUp));
		GetNode<Button>("Panel/WhiteColorButton").Connect("button_up", Callable.From(OnWhiteColorButtonUp));
		GetNode<Button>("Panel/BlackColorButton").Connect("button_up", Callable.From(OnBlackColorButtonUp));
		GetNode<Button>("Panel/RandomColorButton").Connect("button_up", Callable.From(OnRandomColorButtonUp));
		GetNode<Button>("GithubButton").Connect("button_up", Callable.From(OnGithubButtonUp));
	}

	/// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// to be implemented
	}
	
	/// Signal receiver for when the fullscreen button has been pressed.
	private void OnFullscreenButtonUp()
	{
		if (DisplayServer.WindowGetMode() == DisplayServer.WindowMode.Fullscreen)
		{
			DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
		}
		else
		{
			DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
		}
	}

	/// Signal receiver for when the start button has been pressed.
	private void OnStartButtonUp()
	{
		GetTree().ChangeSceneToFile("res://scenes/Game.tscn");
	}

	/// Signal receiver for when the white color button has been pressed.
	private void OnWhiteColorButtonUp()
	{
		chosenColor = "white";
	}
	
	/// Signal receiver for when the black color button has been pressed.
	private void OnBlackColorButtonUp()
	{
		chosenColor = "black";
	}
	
	/// Signal receiver for when the random color button has been pressed.
	private void OnRandomColorButtonUp()
	{
		chosenColor = "random";
	}

	/// Signal receiver for when the github button has been pressed.
	private void OnGithubButtonUp()
	{
		OS.ShellOpen("https://github.com/chessquared/chessium");
	}
}
