using Godot;

namespace chessium.scripts;

public partial class Constants : Node
{
	public enum Player
	{
		WHITE,
		BLACK
	}

	public enum GameMode
	{
		SINGLEPLAYER,
		LOCAL_MULTIPLAYER
	}

	public enum MoveType
	{
		SINGLE,
		DOUBLE,
		EN_PASSANT,
		CASTLING
	}

	public enum PieceType
	{
		PAWN,
		KNIGHT,
		BISHOP,
		ROOK,
		QUEEN,
		KING
	}
}
