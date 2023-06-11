using Godot;
using System;
using Godot.Collections;

namespace chessium.scripts;

public partial class EndScene : Control
{
	public string endgameReason;
	public string player, otherPlayer;

	private Array<string> checkPhrases = new () { "Nice!", "What a game!", "Wow!" };
	private Array<string> drawPhrases = new() { "That's a draw!", "What a game!", "Almost there!" };
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetNode("Popup/CloseButton").Connect("button_up", Callable.From(OnCloseButtonUp));

		var random = new Random();
		var idx = random.Next(3);

		GetNode<Label>("Popup/ReasonLabel").Text = endgameReason.Capitalize() + "!";
		
		if (endgameReason == "checkmate")
		{
			GetNode<Label>("Popup/DescriptionLabel").Text = checkPhrases[idx] + " The " + player + "s " + endgameReason + "d the " + otherPlayer + "s.\nBetter luck next time, " + otherPlayer + "s!";
		}

		if (endgameReason == "stalemate")
		{
			GetNode<Label>("Popup/DescriptionLabel").Text = drawPhrases[idx] + " The " + player + "s almost had it.\n" + otherPlayer.Capitalize() + "s, that was a close call!";
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// to be implemented
	}

	private void OnCloseButtonUp()
	{
		QueueFree();
	}
}
