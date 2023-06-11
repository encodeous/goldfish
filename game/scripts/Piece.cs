using Godot;
using Godot.Collections;

namespace chessium.scripts;

public partial class Piece : Node2D
{
	private Sprite2D sprite;
	private Texture2D texture;
	private Board board;

	public int player, type;
	public bool moved = false, jumped = false;
	
	public Piece(int player, int type)
	{
		this.player = player;
		this.type = type;
	}
	
	// Called when the node enters the scene tree for the first time.
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

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// to be implemented
	}

	public override void _ExitTree()
	{
		sprite.QueueFree();
	}

	private Array<Vector2> GetPieceDirections()
	{
		switch (type)
		{
			case 0: // pawn
				return new Array<Vector2> { new (0, 1) };
			
			case 1: // rook
				return Constants.rookDirections;
			
			case 2: // knight
				return Constants.knightDirections;
			
			case 3: // bishop
				return Constants.bishopDirections;
			
			case 4 or 5: // queen or king
				return Constants.allDirections;
			
			default:
				return new Array<Vector2>();
		}
	}

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

	public Array<Vector2> GetValidMovesFromVector2(Vector2 vector)
	{
		return GetValidMoves((int) vector.X, (int) vector.Y);
	}

	private int DistanceTravelled()
	{
		switch (type)
		{
			case 0: // pawn movement
				return moved ? 1 : 2; // 1 if the pawn has already moved once, 2 otherwise
			
			case 2 or 5: // knight or king movement
				return 1;
			
			default:
				return 8; // every other piece can move the entire length of the board if possible
		}
	}

	private bool IsValidMove(Vector2 position)
	{
		if (IsInBounds(position))
		{
			// true if there is no piece in the way of a move, or if it is not out of bounds
			var piece = board.GetPieceFromVector2(position);
			return piece == null ? true : TileHasEnemy(position);
		}

		return false;
	}

	private bool IsInBounds(Vector2 position)
	{
		// is this position with the board grid (8 * 8)?
		return position.X < 8 && position.X >= 0 && position.Y < 8 && position.Y >= 0;
	}

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

	private Piece GetPiece(Vector2 position)
	{
		// gets a piece on a square so long as the position is within the bounds of the game board
		return !IsInBounds(position) ? null : board.GetPieceFromVector2(position);
	}

	public void UpdateSprite()
	{
		sprite.FrameCoords = new Vector2I(type, player);
	}
}
