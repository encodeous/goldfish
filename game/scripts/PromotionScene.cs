using Godot;

namespace chessium.scripts;

public partial class PromotionScene : Control
{
	/// The piece that is going to promote.
	public Piece piece;
	
	/// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetNode("Popup/BishopButton").Connect("button_up", Callable.From(OnBishopButtonUp));
		GetNode("Popup/KnightButton").Connect("button_up", Callable.From(OnKnightButtonUp));
		GetNode("Popup/RookButton").Connect("button_up", Callable.From(OnRookButtonUp));
		GetNode("Popup/QueenButton").Connect("button_up", Callable.From(OnQueenButtonUp));
	}

	/// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// to be implemented
	}

	/// Promotes the piece to whatever piece type the user has chosen.
	private void ChoosePiece(Constants.PieceType type)
	{
		piece.Promote(type);
	}

	/// Signal receiver for when the bishop was chosen.
	private void OnBishopButtonUp()
	{
		ChoosePiece(Constants.PieceType.BISHOP);
	}

	/// Signal receiver for when the knight was chosen.
	private void OnKnightButtonUp()
	{
		ChoosePiece(Constants.PieceType.KNIGHT);
	}

	/// Signal receiver for when the rook was chosen.
	private void OnRookButtonUp()
	{
		ChoosePiece(Constants.PieceType.ROOK);
	}

	/// Signal receiver for when the queen was chosen.
	private void OnQueenButtonUp()
	{
		ChoosePiece(Constants.PieceType.QUEEN);
	}
}
