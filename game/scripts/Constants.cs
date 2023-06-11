using Godot;
using Godot.Collections;

namespace chessium.scripts;

public partial class Constants : Node
{
	public static readonly int tileSize = 60;
	public static readonly int boardSize = 480;
	
	public static readonly int piece = 0;
	public static readonly int move = 1;
	public static readonly int checkmate = 2;
	public static readonly int awaiting = 3;

	public static readonly int pawn = 0;
	public static readonly int rook = 1;
	public static readonly int knight = 2;
	public static readonly int bishop = 3;
	public static readonly int queen = 4;
	public static readonly int king = 5;

	public static readonly Array<Vector2> rookDirections = new ()
	{
		new Vector2(0, 1),
		new Vector2(0, -1),
		new Vector2(1, 0),
		new Vector2(-1, 0)
	};
	
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
	
	public static readonly Array<Vector2> bishopDirections = new ()
	{
		new Vector2(1, 1),
		new Vector2(-1, 1),
		new Vector2(1, -1),
		new Vector2(-1, -1)
	};
	
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
