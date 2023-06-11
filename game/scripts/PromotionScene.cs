using Godot;

namespace chessium.scripts;

public partial class PromotionScene : Control
{
	public Piece piece;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetNode("Popup/BishopButton").Connect("button_up", Callable.From(OnBishopButtonUp));
		GetNode("Popup/KnightButton").Connect("button_up", Callable.From(OnKnightButtonUp));
		GetNode("Popup/RookButton").Connect("button_up", Callable.From(OnRookButtonUp));
		GetNode("Popup/QueenButton").Connect("button_up", Callable.From(OnQueenButtonUp));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// to be implemented
	}

	private void ChoosePiece(Constants.PieceType type)
	{
		piece.Promote(type);
	}

	private void OnBishopButtonUp()
	{
		ChoosePiece(Constants.PieceType.BISHOP);
	}

	private void OnKnightButtonUp()
	{
		ChoosePiece(Constants.PieceType.KNIGHT);
	}

	private void OnRookButtonUp()
	{
		ChoosePiece(Constants.PieceType.ROOK);
	}

	private void OnQueenButtonUp()
	{
		ChoosePiece(Constants.PieceType.QUEEN);
	}
}
