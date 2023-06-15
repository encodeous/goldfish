using Godot;
using Godot.Collections;

namespace chessium.scripts;

/// <summary>
/// A utility class to store constants.
/// </summary>
public partial class Constants : Node
{
	/// <summary>
	/// The size of an individual tile (that contains a piece).
	/// </summary>
	public const int tileSize = 60;

	/// <summary>
	/// The size of the entire chess board.
	/// </summary>
	public const int boardSize = 480;

	/// <summary>
	/// An enum representing the current state of the game.
	/// </summary>
	public enum GameState
	{
		/// <summary>
		/// Are we clicking on a piece?
		/// </summary>
		GETTING_PIECE,
		/// <summary>
		/// Are we moving a piece?
		/// </summary>
		MAKING_A_MOVE,
		/// <summary>
		/// Are we stalemated?
		/// </summary>
		STALEMATE, // TODO: implement with adam's code
		/// <summary>
		/// Are we checkmated?
		/// </summary>
		CHECKMATE,
		/// <summary>
		/// Are we waiting for the user to make a choice?
		/// </summary>
		WAITING_FOR_USER
	}

	/// <summary>
	/// An enum representing all possible chess pieces.
	/// TODO: refactor with adam's code (make sure enum ordinals match those in adam's code)
	/// </summary>
	public enum Pieces
	{
		/// <summary>
		/// Represents a pawn.
		/// </summary>
		PAWN,
		/// <summary>
		/// Represents a rook.
		/// </summary>
		ROOK,
		/// <summary>
		/// Represents a knight.
		/// </summary>
		KNIGHT,
		/// <summary>
		/// Represents a bishop.
		/// </summary>
		BISHOP,
		/// <summary>
		/// Represents a queen.
		/// </summary>
		QUEEN,
		/// <summary>
		/// Represents a king.
		/// </summary>
		KING
	}

	/// <summary>
	/// Represents the color of the player who is currently playing.
	/// TODO: most likely unnecessary (to be deleted)
	/// </summary>
	public enum Player
	{
		/// <summary>
		/// Represents the white player.
		/// </summary>
		WHITE,
		/// <summary>
		/// Represents the black player.
		/// </summary>
		BLACK
	}

	/// <summary>
	/// Represents the directions a rook can go.
	/// TODO: most likely unnecessary (to be deleted)
	/// </summary>
	public static readonly Array<Vector2> rookDirections = new ()
	{
		new Vector2(0, 1),
		new Vector2(0, -1),
		new Vector2(1, 0),
		new Vector2(-1, 0)
	};
	
	/// <summary>
	/// Represents the directions a knight can go.
	/// TODO: most likely unnecessary (to be deleted)
	/// </summary>
	public static readonly Array<Vector2> knightDirections = new ()
	{
		new Vector2(1, 2),
		new Vector2(-1, 2),
		new Vector2(1, -2),
		new Vector2(-1, -2),
		new Vector2(2, 1),
		new Vector2(2, -1),
		new Vector2(-2, 1),
		new Vector2(-2, -1)
	};
	
	/// <summary>
	/// Represents the directions a bishop can go.
	/// TODO: most likely unnecessary (to be deleted)
	/// </summary>
	public static readonly Array<Vector2> bishopDirections = new ()
	{
		new Vector2(1, 1),
		new Vector2(-1, 1),
		new Vector2(1, -1),
		new Vector2(-1, -1)
	};
	
	/// <summary>
	/// Represents the directions a queen can go, and all directions in general.
	/// TODO: most likely unnecessary (to be deleted)
	/// </summary>
	public static readonly Array<Vector2> allDirections = new ()
	{
		new Vector2(0, 1),
		new Vector2(0, -1), 
		new Vector2(1, 0), 
		new Vector2(-1, 0),
		new Vector2(1, 1),
		new Vector2(-1, 1),
		new Vector2(1, -1),
		new Vector2(-1, -1)
	};
}
