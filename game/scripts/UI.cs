using Godot;

namespace chessium.scripts;

public partial class UI : Node2D
{
	private Sprite2D playerIndicator;
	private Root root;

	private Dialog dialog = new (640 - Constants.boardSize - Dialog.size * 2, Constants.boardSize - Dialog.size * 2);
	private NewButton newButton = new ();
	
	private PieceSlot whitePawnSlot, whitePieceSlot;
	private PieceSlot blackPawnSlot, blackPieceSlot;

	// Called when the node enters the scene tree for the first time.
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

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// to be implemented
	}

	public void NewGame()
	{
		whitePawnSlot.NewGame();
		whitePieceSlot.NewGame();
		
		blackPawnSlot.NewGame();
		blackPieceSlot.NewGame();
	}

	public void SetPlayer(int player)
	{
		playerIndicator.FrameCoords = playerIndicator.FrameCoords with { Y = player };
	}

	public void CapturePiece(Piece piece)
	{
		if (piece.player == 0)
		{
			if (piece.type == Constants.pawn)
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
			if (piece.type == Constants.pawn)
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
