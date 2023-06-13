using Godot;
using Godot.Collections;

namespace chessium.scripts;

public partial class Piece : Node2D
{
	/// <summary>
	/// The sprite of the piece.
	/// </summary>
	private Sprite2D sprite;
	/// <summary>
	/// The image of the piece.
	/// </summary>
	private Texture2D texture;
	/// <summary>
	/// The game board Node.
	/// </summary>
	private Board board;

	/// <summary>
	/// The player who owns the piece and its type.
	/// </summary>
	public int player, type;
	/// <summary>
	/// Has the piece moved or jumped?
	/// </summary>
	public bool moved = false, jumped = false;
	
	/// <summary>
	/// Constructs a new Piece instance.
	/// </summary>
	/// <param name="player">The player who owns the piece.</param>
	/// <param name="type">The type of piece.</param>
	public Piece(int player, int type)
	{
		this.player = player;
		this.type = type;
	}
	
	/// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		sprite = new Sprite2D();
		texture = GD.Load<Texture2D>("res://assets/pieces.png");
		board = GetParent<Board>();

		sprite.Texture = texture;
		sprite.Centered = false;
		sprite.Hframes = 6;
		sprite.Vframes = 2;

		UpdateSprite();
		AddChild(sprite);
	}

	/// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// to be implemented
	}

	/// <summary>
	/// Called when the Node is about to leave the SceneTree.
	/// </summary>
	public override void _ExitTree()
	{
		sprite.QueueFree();
	}

	/// <summary>
	/// Gets the valid piece directions depending on the type of piece.
	/// </summary>
	/// <returns>An array of valid piece directions depending on the type of piece.</returns>
	private Array<Vector2> GetPieceDirections()
	{
		return type switch
		{
			0 => // pawn
				new Array<Vector2> { new(0, 1) },
			1 => // rook
				Constants.rookDirections,
			2 => // knight
				Constants.knightDirections,
			3 => // bishop
				Constants.bishopDirections,
			4 or 5 => // queen or king
				Constants.allDirections,
			_ => new Array<Vector2>()
		};
	}

	/// <summary>
	/// Gets all valid moves for this piece, depending on its type.
	/// </summary>
	/// <param name="x">The row of the piece.</param>
	/// <param name="y">The column of the piece.</param>
	/// <returns>An array of valid moves.</returns>
	public Array<Vector2> GetValidMoves(int x, int y)
	{
		var positions = new Array<Vector2>();
		var multi = -1 + player * 2;

		// where can all regular pieces move (queen, bishop, knight, rook)?
		foreach (var direction in GetPieceDirections())
		{
			for (var i = 0; i < DistanceTravelled(); i++)
			{
				var position = new Vector2(x + direction.X * (i + 1), y + direction.Y * (i + 1) * multi);
				if (IsValidMove(position))
				{
					if (TileHasEnemy(position) || (TileHasEnemy(position) && type == Constants.pawn))
					{
						break;
					}

					if (!board.WouldBeInCheck(x, y, this, position))
					{
						positions.Add(position);
					}
				}

				if (board.GetPieceFromVector2(position) != null)
				{
					// if there's a piece in the way, stop
					break;
				}
			}
		}

		// can a pawn move? can it capture a piece with en passant?
		if (type == Constants.pawn)
		{
			for (var i = 0; i < 2; i++)
			{
				var position = new Vector2(x - 1 + i * 2, y + multi);
				
				var canEnPassant = false;
				var enPassantPosition = new Vector2(x - 1 + i * 2, y);
				var enPassantPiece = board.GetPieceFromVector2(enPassantPosition);

				if (enPassantPiece != null)
				{
					// has the pawn fulfilled all preconditions for en passant?
					canEnPassant = enPassantPiece.type == Constants.pawn && enPassantPiece.jumped && enPassantPiece.player != player;
				}

				if ((TileHasEnemy(position) || canEnPassant) && !board.WouldBeInCheck(x, y, this, position))
				{
					// would capturing or moving put the king in check? if not, add to move list
					positions.Add(position);
				}
			}
		}

		// can the king castle, move or capture?
		if (type == Constants.king && !moved)
		{
			for (var i = 0; i < 2; i++)
			{
				var xOffset = 1 - (2 * i);
				var xToCheck = x + xOffset;

				while (xToCheck != 0 && xToCheck != 7)
				{
					if (GetPiece(new Vector2(xToCheck, y)) != null)
					{
						break;
					}

					xToCheck += xOffset;
				}

				if (xToCheck != 0 && xToCheck != 7)
				{
					continue;
				}

				var piece = GetPiece(new Vector2(xOffset, y));
				if (piece == null)
				{
					continue;
				}

				if (piece.type == Constants.rook && piece.player == player && !piece.moved)
				{
					var position = new Vector2(x, y);
					var rookPosition = new Vector2(xToCheck, y);
					var newPosition = new Vector2(x + 2 * i, y);
					var newRookPosition = new Vector2(newPosition.X - i, y);

					if (!board.WouldBeInCheckFromCastling(position, rookPosition, newPosition, newRookPosition))
					{
						positions.Add(rookPosition); // castling is possible
					}
				}
			}
		}

		return positions;
	}

	/// <summary>
	/// Gets all valid moves for a piece from a Vector2 position.
	/// </summary>
	/// <param name="vector">The position of the piece.</param>
	/// <returns>An array of valid moves.</returns>
	public Array<Vector2> GetValidMovesFromVector2(Vector2 vector)
	{
		return GetValidMoves((int) vector.X, (int) vector.Y);
	}

	/// <summary>
	/// Gets the number of squares a piece can travel in a move.
	/// </summary>
	/// <returns>An int depending on the type of piece.</returns>
	private int DistanceTravelled()
	{
		return type switch
		{
			// pawn movement
			0 => moved ? 1 : 2, // 1 if the pawn has already moved once, 2 otherwise
			// knight or king movement
			2 or 5 => 1,
			_ => 8
		};
	}

	/// <summary>
	/// Checks if a move is legal.
	/// </summary>
	/// <param name="position">The position of the move to make.</param>
	/// <returns>True if the move is legal, false otherwise.</returns>
	private bool IsValidMove(Vector2 position)
	{
		if (IsInBounds(position))
		{
			// true if there is no piece in the way of a move, or if it is not out of bounds
			var piece = board.GetPieceFromVector2(position);
			return piece == null || TileHasEnemy(position);
		}

		return false;
	}

	/// <summary>
	/// Checks if a position is within the bounds of the chess board.
	/// </summary>
	/// <param name="position">The position to check.</param>
	/// <returns>True if the position is within the chess board, false otherwise.</returns>
	private bool IsInBounds(Vector2 position)
	{
		// is this position with the board grid (8 * 8)?
		return position.X < 8 && position.X >= 0 && position.Y < 8 && position.Y >= 0;
	}

	/// <summary>
	/// Checks if a tile contains an enemy piece.
	/// </summary>
	/// <param name="position">The position to check for.</param>
	/// <returns>True if a tile has an enemy, false otherwise.</returns>
	private bool TileHasEnemy(Vector2 position)
	{
		if (!IsInBounds(position))
		{
			// there can't be an enemy if it's not on the game board
			return false;
		}

		var piece = board.GetPieceFromVector2(position);
		if (piece != null)
		{
			// a piece isn't an enemy if it's of your same color
			return piece.player != player;
		}

		// an empty tile has no enemies for either player
		return false;
	}

	/// <summary>
	/// Gets a piece from the chess board.
	/// </summary>
	/// <param name="position">The position to get the piece from.</param>
	/// <returns>A piece from the position, if any.</returns>
	private Piece GetPiece(Vector2 position)
	{
		// gets a piece on a square so long as the position is within the bounds of the game board
		return !IsInBounds(position) ? null : board.GetPieceFromVector2(position);
	}

	/// <summary>
	/// Updates the sprite of the piece based on its type and the player that owns it.
	/// </summary>
	public void UpdateSprite()
	{
		sprite.FrameCoords = new Vector2I(type, player);
	}
}
