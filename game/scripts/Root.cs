using Godot;

namespace chessium.scripts;

/// <summary>
/// Represents the main scene of the game (the chess board + other UI elements).
/// </summary>
public partial class Root : Node2D
{
	/// <summary>
	/// The chess board.
	/// </summary>
	private Board board;
	/// <summary>
	/// The other UI elements.
	/// </summary>
	private UI ui;

	/// <summary>
	/// The current player, game state and winner, if any.
	/// </summary>
	public int player, gameState = Constants.piece, winner;
	
	/// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		board = GetNode<Board>("Board");
		ui = GetNode<UI>("UI");
		
		NewGame();
	}

	/// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// to be implemented
	}

	/// <summary>
	/// Starts a new game.
	/// </summary>
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

	/// <summary>
	/// Switches the current player's turn.
	/// </summary>
	public void SwitchPlayer()
	{
		player = Mathf.Abs(player - 1);
		
		ui.SetPlayer(player);
		board.ClearJumps();
	}

	/// <summary>
	/// Captures a piece and removes it from the board.
	/// </summary>
	/// <param name="piece">The piece to capture.</param>
	public void Capture(Piece piece)
	{
		ui.CapturePiece(piece);
	}
}
