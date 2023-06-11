using Godot;

namespace chessium.scripts;

public partial class Root : Node2D
{
	private Board board;
	private UI ui;

	public int player, gameState = Constants.piece, winner;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		board = GetNode<Board>("Board");
		ui = GetNode<UI>("UI");
		
		NewGame();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// to be implemented
	}

	private void NewGame()
	{
		player = 1;
		gameState = Constants.piece;

		SwitchPlayer();
		
		board.NewGame();
		ui.NewGame();

		foreach (var child in GetChildren())
		{
			if (child is Dialog)
			{
				RemoveChild(child);
				child.QueueFree();
			}
		}
	}

	public void SwitchPlayer()
	{
		player = Mathf.Abs(player - 1);
		
		ui.SetPlayer(player);
		board.ClearJumps();
	}

	public void Capture(Piece piece)
	{
		ui.CapturePiece(piece);
	}
}
