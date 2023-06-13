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
	public static readonly int tileSize = 60;
	/// <summary>
	/// The size of the entire chess board.
	/// </summary>
	public static readonly int boardSize = 480;
	
	// TODO: switch to enum
	
	/// <summary>
	/// Are we clicking on a piece?
	/// </summary>
	public static readonly int piece = 0;
	/// <summary>
	/// Are we moving a piece?
	/// </summary>
	public static readonly int move = 1;
	/// <summary>
	/// Are we checkmated?
	/// </summary>
	public static readonly int checkmate = 2;
	/// <summary>
	/// Are we waiting for the user to make a choice?
	/// </summary>
	public static readonly int awaiting = 3;

	// TODO: switch to enum
	
	/// <summary>
	/// Represents a pawn.
	/// </summary>
	public static readonly int pawn = 0;
	/// <summary>
	/// Represents a rook.
	/// </summary>
	public static readonly int rook = 1;
	/// <summary>
	/// Represents a knight.
	/// </summary>
	public static readonly int knight = 2;
	/// <summary>
	/// Represents a bishop.
	/// </summary>
	public static readonly int bishop = 3;
	/// <summary>
	/// Represents a queen.
	/// </summary>
	public static readonly int queen = 4;
	/// <summary>
	/// Represents a king.
	/// </summary>
	public static readonly int king = 5;

	/// <summary>
	/// Represents the directions a rook can go.
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
