using Godot;

namespace chessium.scripts;

public partial class HUD : CanvasLayer
{
	/// The settings scene is loaded to instantiate when needed.
	[Export] public PackedScene settingsScene = GD.Load<PackedScene>("res://scenes/SettingsScene.tscn");
	
	/// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetNode("SettingsButton").Connect("button_up", Callable.From(OnSettingsButtonUp));
		GetNode("FullscreenButton").Connect("button_up", Callable.From(OnFullscreenButtonUp));
		GetNode("CloseButton").Connect("button_up", Callable.From(OnCloseButtonUp));
	}

	/// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// to be implemented
	}

	/// Signal receiver for when the settings button is pressed.
	private void OnSettingsButtonUp()
	{
		var settings = settingsScene.Instantiate();
		AddChild(settings);
	}

	/// Signal receiver for when the fullscreen button is pressed.
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

	/// Signal receiver for when the close button is pressed.
	private void OnCloseButtonUp()
	{
		GetTree().ChangeSceneToFile("res://scenes/StartScene.tscn");
	}
}
