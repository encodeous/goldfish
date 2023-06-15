using Godot;

namespace chessium.scripts;

/// <summary>
/// Represents additional UI elements (sidebars, etc).
/// </summary>
public partial class UI : Node2D
{
	/// <summary>
	/// Indicates the current player's turn.
	/// </summary>
	private Sprite2D playerIndicator;
	
	/// <summary>
	/// The instance of the root scene.
	/// </summary>
	private Root root;

	/// <summary>
	/// The base window for the other UI elements.
	/// </summary>
	private Dialog dialog = new (640 - Constants.boardSize - Dialog.size * 2, Constants.boardSize - Dialog.size * 2);
	
	/// <summary>
	/// The button to start a new game.
	/// </summary>
	private NewButton newButton = new ();
	
	/// <summary>
	/// The piece slots to store captured pieces for both players.
	/// </summary>
	private PieceSlot whitePawnSlot, whitePieceSlot;
	private PieceSlot blackPawnSlot, blackPieceSlot;

	/// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		playerIndicator = GetNode<Sprite2D>("PlayerIndicator/PlayerIndicatorSprite");
		root = GetParent<Root>();

		whitePawnSlot = GetNode<PieceSlot>("PieceSlots/PieceSlotWP");
		whitePieceSlot = GetNode<PieceSlot>("PieceSlots/PieceSlotWO");

		blackPawnSlot = GetNode<PieceSlot>("PieceSlots/PieceSlotBP");
		blackPieceSlot = GetNode<PieceSlot>("PieceSlots/PieceSlotBO");

		dialog.ZIndex = -10;
		AddChild(dialog);

		newButton.Position = new Vector2(80.0f - NewButton.newWidth / 2.0f - Dialog.size, Constants.boardSize - NewButton.newHeight - Dialog.size * 2 - 10);
		newButton.ZIndex = 10;
		AddChild(newButton);
	}

	/// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// to be implemented
	}

	/// <summary>
	/// Starts a new game.
	/// </summary>
	public void NewGame()
	{
		whitePawnSlot.NewGame();
		whitePieceSlot.NewGame();
		
		blackPawnSlot.NewGame();
		blackPieceSlot.NewGame();
	}

	/// <summary>
	/// Sets the player indicator to the current player.
	/// TODO: refactor with adam's code
	/// </summary>
	/// <param name="player">The current player.</param>
	public void SetPlayer(Constants.Player player)
	{
		playerIndicator.FrameCoords = playerIndicator.FrameCoords with { Y = (int) player };
	}

	/// <summary>
	/// Adds a captured piece to the relevant piece slot depending on its type and the player who owns it.
	/// TODO: refactor with adam's code
	/// </summary>
	/// <param name="piece">The captured piece.</param>
	public void CapturePiece(Piece piece)
	{
		if (piece.player == 0)
		{
			if (piece.type == Constants.Pieces.PAWN)
			{
				whitePawnSlot.AddPiece(piece.type);
			}
			else
			{
				whitePieceSlot.AddPiece(piece.type);
			}
		}
		else
		{
			if (piece.type == Constants.Pieces.PAWN)
			{
				blackPawnSlot.AddPiece(piece.type);
			}
			else
			{
				blackPieceSlot.AddPiece(piece.type);
			}
		}
	}
}
