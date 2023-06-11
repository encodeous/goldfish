using Godot;

namespace chessium.scripts;

public partial class Constants : Node
{
	/// Enum that represents the color of a player.
	public enum Player
	{
		WHITE,
		BLACK
	}

	/// Enum that represents the game mode.
	public enum GameMode
	{
		SINGLEPLAYER,
		LOCAL_MULTIPLAYER
	}

	/// Enum that represents the type of move played.
	public enum MoveType
	{
		SINGLE,
		DOUBLE,
		EN_PASSANT,
		CASTLING
	}

	/// Enum that represents the type of piece on a cell on the board.
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
