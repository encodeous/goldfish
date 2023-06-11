using Godot;
using Godot.Collections;

namespace chessium.scripts;

public partial class Piece : Node2D
{
	private const int SPRITE_SIZE = 256;
	
	public Constants.PieceType pieceType = Constants.PieceType.PAWN;
	[Export] public Constants.Player color = Constants.Player.BLACK;
	
	[Export] public Vector2 boardCoordinates, position;
	private int moveCount;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SetPieceSprite();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void SetPieceSprite()
	{
		var colorMod = color == Constants.Player.WHITE ? SPRITE_SIZE : 0;
		var regionRect = new Rect2();

		switch (pieceType)
		{
			case Constants.PieceType.PAWN:
				regionRect = new Rect2(SPRITE_SIZE * 0, colorMod, SPRITE_SIZE, SPRITE_SIZE);
				break;
				
			case Constants.PieceType.KNIGHT:
				regionRect = new Rect2(SPRITE_SIZE * 2, colorMod, SPRITE_SIZE, SPRITE_SIZE);
				break;

			case Constants.PieceType.KING:
				regionRect = new Rect2(SPRITE_SIZE * 5, colorMod, SPRITE_SIZE, SPRITE_SIZE);
				break;

			case Constants.PieceType.ROOK:
				regionRect = new Rect2(SPRITE_SIZE * 1, colorMod, SPRITE_SIZE, SPRITE_SIZE);
				break;

			case Constants.PieceType.BISHOP:
				regionRect = new Rect2(SPRITE_SIZE * 3, colorMod, SPRITE_SIZE, SPRITE_SIZE);
				break;

			case Constants.PieceType.QUEEN:
				regionRect = new Rect2(SPRITE_SIZE * 4, colorMod, SPRITE_SIZE, SPRITE_SIZE);
				break;
		}

		GetNode<Sprite2D>("Sprite").RegionRect = regionRect;
	}

	public void Promote(Constants.PieceType newType)
	{
		pieceType = newType;
		SetPieceSprite();
	}

	public void DragTo(Vector2 pos)
	{
		position = pos;
	}

	public void IncreaseMoveCount()
	{
		moveCount++;
	}

	public Array<Move> GetLegalMoves(Board board, Piece enPassantPiece)
	{
		var legalMoves = FilterKingSafeMoves(GetPseudoMoves(board, enPassantPiece), board, enPassantPiece);
		return legalMoves;
	}

	private Array<Move> GetPseudoMoves(Board board, Piece enPassantPiece)
	{
		switch (pieceType)
		{
			case Constants.PieceType.PAWN:
				return GetPawnMoves(board, enPassantPiece);
			
			case Constants.PieceType.ROOK:
				return GetRookMoves(board);
			
			case Constants.PieceType.KNIGHT:
				return GetKnightMoves(board);
			
			case Constants.PieceType.BISHOP:
				return GetBishopMoves(board);
			
			case Constants.PieceType.QUEEN:
				return GetQueenMoves(board);
			
			case Constants.PieceType.KING:
				return GetKingMoves(board);
			
			default:
				return null;
		}
	}

	private Array<Move> GetPawnMoves(Board board, Piece enPassantPiece)
	{
		var moves = new Array<Move>();
		var standardMove = color == Constants.Player.WHITE ? new Vector2(0, 1) : new Vector2(0, -1);

		if (IsValidPawn(standardMove, board))
		{
			moves.Add(new Move(this, Constants.MoveType.SINGLE, boardCoordinates + standardMove));
		}

		if (moveCount == 0)
		{
			var doubleMove = color == Constants.Player.WHITE ? new Vector2(0, 2) : new Vector2(0, -2);
			if (IsValidPawn(standardMove, board) && IsValidPawn(standardMove, board))
			{
				moves.Add(new Move(this, Constants.MoveType.DOUBLE, boardCoordinates + doubleMove));
			}
		}

		moves += GetPawnCaptures(board, enPassantPiece);
		return moves;
	}

	private Array<Move> GetPawnCaptures(Board board, Piece enPassantPiece)
	{
		var moves = new Array<Move>();
		var captures = color == Constants.Player.WHITE ? new Array<Vector2>(new []{ new Vector2(1, 1), new Vector2(-1, 1) }) : new Array<Vector2>(new []{new Vector2(-1, -1), new Vector2(1, -1)});

		foreach (var vec in captures)
		{
			var move = boardCoordinates + vec;
			if (Utils.IsInsideBoard(move))
			{
				var piece = board.boardMatrix[(int) move.X, (int) move.Y];
				if (piece != null && piece.color != this.color)
				{
					moves.Add(new Move(this, Constants.MoveType.SINGLE, move));
				}
			}
		}
		
		var enPassantMoves = color == Constants.Player.WHITE ? new Array<Vector2>(new []{ new Vector2(1, 1), new Vector2(-1, 1) }) : new Array<Vector2>(new []{new Vector2(-1, -1), new Vector2(1, -1)});

		foreach (var vec in enPassantMoves)
		{
			var move = boardCoordinates + vec;
			if (Utils.IsInsideBoard(move))
			{
				var victim = board.boardMatrix[(int)move.X, (int)move.Y];
				if (victim != null && victim == enPassantPiece)
				{
					moves.Add(new Move(this, Constants.MoveType.EN_PASSANT, move, victim));
				}
			}
		}
		
		return moves;
	}

	private Array<Move> GetKnightMoves(Board board)
	{
		var possibleMoves = new Array<Vector2>();
		possibleMoves.Add(new Vector2(2, 1));
		possibleMoves.Add(new Vector2(2, -1));
		possibleMoves.Add(new Vector2(-2, 1));
		possibleMoves.Add(new Vector2(-2, -1));
		possibleMoves.Add(new Vector2(1, 2));
		possibleMoves.Add(new Vector2(1, -2));
		possibleMoves.Add(new Vector2(-1, 2));
		possibleMoves.Add(new Vector2(-1, -2));

		var moves = new Array<Move>();
		foreach (var move in possibleMoves)
		{
			if (IsValidPlacing(move, board))
			{
				moves.Add(new Move(this, Constants.MoveType.SINGLE, boardCoordinates + move));
			}
		}

		return moves;
	}

	private Array<Move> GetKingMoves(Board board)
	{
		var possibleMoves = new Array<Vector2>();
		possibleMoves.Add(new Vector2(0, 1));
		possibleMoves.Add(new Vector2(0, -1));
		possibleMoves.Add(new Vector2(1, 0));
		possibleMoves.Add(new Vector2(-1, 0));
		possibleMoves.Add(new Vector2(1, 1));
		possibleMoves.Add(new Vector2(1, -1));
		possibleMoves.Add(new Vector2(-1, 1));
		possibleMoves.Add(new Vector2(-1, -1));

		var moves = new Array<Move>();
		foreach (var move in possibleMoves)
		{
			if (IsValidPlacing(move, board))
			{
				moves.Add(new Move(this, Constants.MoveType.SINGLE, boardCoordinates + move));
			}
		}

		moves += GetCastlingMoves(board);
		return moves;
	}

	private Array<Move> GetCastlingMoves(Board board)
	{
		var moves = new Array<Move>();

		if (moveCount != 0)
		{
			return moves;
		}

		var leftRook = board.boardMatrix[0, (int) boardCoordinates.Y];
		var rightRook = board.boardMatrix[7, (int) boardCoordinates.Y];

		if (leftRook != null && board.boardMatrix[1, (int) boardCoordinates.Y] == null && board.boardMatrix[2, (int) boardCoordinates.Y] == null && board.boardMatrix[3, (int) boardCoordinates.Y] == null && leftRook.pieceType == Constants.PieceType.ROOK && leftRook.moveCount == 0)
		{
			var possibleMove = new Vector2(-2, 0);
			if (IsValidPlacing(possibleMove, board))
			{
				moves.Add(new Move(this, Constants.MoveType.CASTLING, boardCoordinates + possibleMove, leftRook));
			}
		}

		if (rightRook != null && board.boardMatrix[5, (int) boardCoordinates.Y] == null && board.boardMatrix[6, (int) boardCoordinates.Y] == null && rightRook.pieceType == Constants.PieceType.ROOK && rightRook.moveCount == 0)
		{
			var possibleMove = new Vector2(2, 0);
			if (IsValidPlacing(possibleMove, board))
			{
				moves.Add(new Move(this, Constants.MoveType.CASTLING, boardCoordinates + possibleMove, rightRook));
			}
		}

		return moves;
	}

	private Array<Move> GetRookMoves(Board board)
	{
		var directions = new Array<Vector2>();
		directions.Add(new Vector2(0, 1));
		directions.Add(new Vector2(0, -1));
		directions.Add(new Vector2(1, 0));
		directions.Add(new Vector2(-1, 0));

		return GetSlidingMoves(directions, board);
	}
	
	private Array<Move> GetBishopMoves(Board board)
	{
		var directions = new Array<Vector2>();
		directions.Add(new Vector2(1, 1));
		directions.Add(new Vector2(1, -1));
		directions.Add(new Vector2(-1, -1));
		directions.Add(new Vector2(-1, 1));

		return GetSlidingMoves(directions, board);
	}

	private Array<Move> GetQueenMoves(Board board)
	{
		var directions = new Array<Vector2>();
		directions.Add(new Vector2(0, 1));
		directions.Add(new Vector2(0, -1));
		directions.Add(new Vector2(1, 0));
		directions.Add(new Vector2(-1, 0));
		directions.Add(new Vector2(1, 1));
		directions.Add(new Vector2(1, -1));
		directions.Add(new Vector2(-1, -1));
		directions.Add(new Vector2(-1, 1));

		return GetSlidingMoves(directions, board);
	}

	private Array<Move> GetSlidingMoves(Array<Vector2> directions, Board board)
	{
		var moves = new Array<Move>();

		foreach (var direction in directions)
		{
			var i = 1;
			while (true)
			{
				var move = direction * i;
				move += boardCoordinates;

				if (!Utils.IsInsideBoard(move))
				{
					break;
				}

				var piece = board.boardMatrix[(int) move.X, (int) move.Y];
				if (piece != null)
				{
					if (piece.color != this.color)
					{
						moves.Add(new Move(this, Constants.MoveType.SINGLE, move));
					}
					else
					{
						break;
					}
				}
				
				moves.Add(new Move(this, Constants.MoveType.SINGLE, move));
				i++;
			}
		}

		return moves;
	}

	private bool IsValidPawn(Vector2 move, Board board)
	{
		move += boardCoordinates;
		if (!Utils.IsInsideBoard(move) || board.boardMatrix[(int) move.X, (int) move.Y] != null)
		{
			return false;
		}

		return true;
	}

	private bool IsValidPlacing(Vector2 move, Board board)
	{
		move += boardCoordinates;
		if (!Utils.IsInsideBoard(move))
		{
			return false;
		}
		
		var piece = board.boardMatrix[(int) move.X, (int) move.Y];
		if (piece != null)
		{
			if (piece.color == this.color)
			{
				return false;
			}
			
			return true;
		}

		return true;
	}

	private Array<Move> FilterKingSafeMoves(Array<Move> moves, Board board, Piece enPassantPiece)
	{
		var movesToRemove = new Array<int>();

		for (var i = 0; i < moves.Count; i++)
		{
			var move = moves[i];
			var startingPosition = move.piece.boardCoordinates;

			var playerTurn = Utils.playerTurn;
			var otherPlayerTurn = Utils.otherPlayerTurn;

			var capturedPiece = TestMovePiece(move, board);

			foreach (var nextMove in GetAllPseudoMoves(otherPlayerTurn, board, enPassantPiece))
			{
				var king = GetKing(playerTurn, board);
				if (king != null && nextMove.destination == king.boardCoordinates)
				{
					movesToRemove.Add(i);
					break;
				}
			}

			TestUnmovePiece(new Move(move.piece, Constants.MoveType.SINGLE, startingPosition), board, capturedPiece);
		}

		if (movesToRemove.Count == 0)
		{
			return moves;
		}
		
		movesToRemove.Reverse();

		foreach (var index in movesToRemove)
		{
			moves.RemoveAt(index);
		}

		return moves;
	}

	private Piece TestMovePiece(Move move, Board board)
	{
		board.boardMatrix[(int) move.piece.boardCoordinates.X, (int) move.piece.boardCoordinates.Y] = null;
		var capturedPiece = board.boardMatrix[(int) move.destination.X, (int) move.destination.Y];

		board.boardMatrix[(int) move.destination.X, (int) move.destination.Y] = move.piece;
		move.piece.boardCoordinates = move.destination;

		return capturedPiece;
	}

	private void TestUnmovePiece(Move move, Board board, Piece capturedPiece)
	{
		board.boardMatrix[(int) move.piece.boardCoordinates.X, (int) move.piece.boardCoordinates.Y] = capturedPiece;
		board.boardMatrix[(int) move.destination.X, (int) move.destination.Y] = move.piece;

		move.piece.boardCoordinates = move.destination;
	}

	private Piece GetKing(Constants.Player player, Board board)
	{
		var king = new Piece();
		foreach (var piece in board.boardMatrix)
		{
			if (piece != null && piece.color == player && piece.pieceType == Constants.PieceType.KING)
			{
				king = piece;
			}
		}

		return king;
	}

	private Array<Move> GetAllPseudoMoves(Constants.Player player, Board board, Piece enPassantPiece)
	{
		var allMoves = new Array<Move>();
		foreach (var piece in board.boardMatrix)
		{
			if (piece != null && piece.color == player)
			{
				allMoves += piece.GetPseudoMoves(board, enPassantPiece);
			}
		}

		return allMoves;
	}
}
