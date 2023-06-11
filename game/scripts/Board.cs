using Godot;
using Godot.Collections;

namespace chessium.scripts;

public partial class Board : Node2D
{
	public readonly Piece[,] boardMatrix =
	{
		{null, null, null, null, null, null, null, null},
		{null, null, null, null, null, null, null, null},
		{null, null, null, null, null, null, null, null},
		{null, null, null, null, null, null, null, null},
		{null, null, null, null, null, null, null, null},
		{null, null, null, null, null, null, null, null},
		{null, null, null, null, null, null, null, null},
		{null, null, null, null, null, null, null, null}
	};
	
	private Piece enPassantPiece, heldPiece;
	private Array<Move> heldPieceMoves;
	
	[Export] public PackedScene promotionScene = GD.Load<PackedScene>("res://scenes/PromotionScene.tscn");
	[Export] public PackedScene endScene = GD.Load<PackedScene>("res://scenes/EndScene.tscn");
	[Export] public PackedScene pieceScene = GD.Load<PackedScene>("res://scenes/Piece.tscn");
	
	[Signal] public delegate void PieceMovedEventHandler();
	[Signal] public delegate void PawnPromotedEventHandler(Piece piece);
	[Signal] public delegate void CheckmateEventHandler();
	[Signal] public delegate void StalemateEventHandler();
	
	private Texture2D whiteDotTexture = GD.Load<Texture2D>("res://assets/misc/white_dot.png");

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Connect("PieceMoved", Callable.From(OnBoardPieceMoved));
		Connect("Checkmate", Callable.From(OnBoardCheckmate));
		Connect("Stalemate", Callable.From(OnBoardStalemate));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// to be implemented
	}

	public override void _Input(InputEvent @event)
	{
		if (Input.IsActionJustPressed("click"))
		{
			var mousePosition = GetLocalMousePosition();
			var clickedBoardCoordinates = GetBoardCoordinates(mousePosition);

			if (Utils.IsInsideBoard(clickedBoardCoordinates))
			{
				heldPiece = boardMatrix[clickedBoardCoordinates.X, clickedBoardCoordinates.Y];
				if (heldPiece != null)
				{
					if (Utils.playerTurn != heldPiece.color)
					{
						heldPiece = null;
					}
					else
					{
						heldPieceMoves = heldPiece.GetLegalMoves(this, enPassantPiece);
						HighlightMoves(heldPieceMoves);
					}
				}
			}

			return;
		}

		if (heldPiece != null)
		{
			var mousePosition = GetLocalMousePosition();
			var clickedBoardCoordinates = GetBoardCoordinates(mousePosition);

			if (Input.IsActionPressed("click"))
			{
				heldPiece.DragTo(mousePosition);
			}

			if (Input.IsActionJustReleased("click"))
			{
				ClearHighlightedMoves();

				foreach (var move in heldPieceMoves)
				{
					if (move.destination == clickedBoardCoordinates)
					{
						MakePlay(move);
					}
				}
				
				SnapPiecesToPosition(new Array<Piece> {heldPiece});

				heldPiece = null;
				heldPieceMoves = null;
			}
		}
	}

	public void SnapPiecesToPosition(Array<Piece> pieces)
	{
		foreach (var piece in pieces)
		{
			piece.position = GetWorldCoordinates(piece.boardCoordinates);
		}
	}

	private Vector2I GetBoardCoordinates(Vector2 worldCoordinates)
	{
		return GetNode<TileMap>("TileMap").LocalToMap(worldCoordinates) * new Vector2I(1, -1) * new Vector2I(4, 3);
	}

	private Vector2 GetTileMapCoordinates(Vector2 boardCoordinates)
	{
		return boardCoordinates * new Vector2(1, -1) + new Vector2(4, 3);
	}

	private Vector2 GetWorldCoordinates(Vector2 boardCoordinates)
	{
		return GetNode<TileMap>("TileMap").MapToLocal(new Vector2I((int) boardCoordinates.X, (int) boardCoordinates.Y) + new Vector2I(-4, 3)) + GetNode<Sprite2D>("Sprite").Texture.GetSize() / 16;
	}

	private void HighlightMoves(Array<Move> possibleMoves)
	{
		foreach (var move in possibleMoves)
		{
			var highlight = new Sprite2D();
			highlight.Position = GetWorldCoordinates(move.destination);
			highlight.Texture = whiteDotTexture;
			highlight.Modulate = new Color(0, 0, 0, 0.25f);

			if (Utils.IsInsideBoard(move.destination) && (boardMatrix[(int) move.destination.X, (int) move.destination.Y] != null || move.moveType == Constants.MoveType.EN_PASSANT))
			{
				highlight.Scale *= 3;
			}
			
			GetNode("Highlights").AddChild(highlight);
		}
	}

	private void ClearHighlightedMoves()
	{
		foreach (var highlight in GetNode("Highlights").GetChildren())
		{
			highlight.QueueFree();
		}
	}

	public Piece AddPieceToBoard(Constants.Player color, Constants.PieceType type, Vector2 piecePosition)
	{
		var piece = pieceScene.Instantiate<Piece>();
		piece.pieceType = type;
		piece.color = color;
		piece.boardCoordinates = piecePosition;
		
		boardMatrix[(int) piecePosition.X, (int) piecePosition.Y] = piece;
		GetNode("Pieces").AddChild(piece);

		return piece;
	}

	private void MakePlay(Move move)
	{
		var victim = boardMatrix[(int) move.destination.X, (int) move.destination.Y];
		if (victim != null)
		{
			victim.QueueFree();
			victim.Free();
		}

		MoveCastlingRook(move);
		HandleEnPassant(move);
		HandlePromotion(move);
		MovePiece(move);

		EmitSignal("PieceMoved");
	}

	private void MovePiece(Move move)
	{
		move.piece.IncreaseMoveCount();
		boardMatrix[(int) move.piece.boardCoordinates.X, (int) move.piece.boardCoordinates.Y] = null;
		boardMatrix[(int) move.destination.X, (int) move.destination.Y] = move.piece;
		move.piece.boardCoordinates = move.destination;
		
		Utils.PlaySound(GetNode<AudioStreamPlayer>("MoveSoundPlayer"));
	}

	private void MoveCastlingRook(Move move)
	{
		if (move.moveType != Constants.MoveType.CASTLING)
		{
			return;
		}

		var kingDestination = move.destination;
		var kingMove = move.piece.boardCoordinates - kingDestination;
		var kingMoveDirection = kingMove.Normalized() * -1;

		var castlingRook = move.affectedPiece;
		var rookMove = new Move(castlingRook, Constants.MoveType.CASTLING, new Vector2(kingDestination.X + kingMoveDirection.X * -1, kingDestination.Y));
		
		MovePiece(rookMove);
		Utils.PlaySound(GetNode<AudioStreamPlayer>("CastlingSoundPlayer"));
		SnapPiecesToPosition(new Array<Piece> {castlingRook});
	}

	private void HandleEnPassant(Move move)
	{
		if (move.moveType == Constants.MoveType.EN_PASSANT)
		{
			boardMatrix[(int) enPassantPiece.boardCoordinates.X, (int) enPassantPiece.boardCoordinates.Y] = null;
			enPassantPiece.QueueFree();
			enPassantPiece.Free();
		}

		enPassantPiece = move.moveType == Constants.MoveType.DOUBLE ? move.piece : null;
	}

	private void HandlePromotion(Move move)
	{
		if (move.piece.pieceType == Constants.PieceType.PAWN && ((int) move.destination.Y == 0 || (int) move.destination.Y == 7))
		{
			EmitSignal("PawnPromoted", move.piece);
		}
	}

	private bool GameOver()
	{
		var moves = GetAllLegalMoves(Utils.playerTurn);
		var check = IsInCheck();

		if (moves.Count == 0)
		{
			if (check)
			{
				EmitSignal("Checkmate");
			}
			else
			{
				EmitSignal("Stalemate");
			}

			return true;
		}

		return false;
	}

	private bool IsInCheck()
	{
		var moves = GetAllLegalMoves(Utils.otherPlayerTurn);
		var kingPosition = GetKing(Utils.playerTurn).boardCoordinates;
		var check = false;

		foreach (var move in moves)
		{
			if (move.destination == kingPosition)
			{
				check = true;
			}
		}

		return check;
	}

	private Array<Move> GetAllLegalMoves(Constants.Player player)
	{
		var allMoves = new Array<Move>();
		foreach (var piece in boardMatrix)
		{
			if (piece != null && piece.color == player)
			{
				allMoves += piece.GetLegalMoves(this, enPassantPiece);
			}
		}

		return allMoves;
	}

	private Piece GetKing(Constants.Player player)
	{
		var king = new Piece();
		foreach (var piece in boardMatrix)
		{
			if (piece != null && piece.color == player && piece.pieceType == Constants.PieceType.KING)
			{
				king = piece;
			}
		}

		return king;
	}
	
	public Array<Piece> GetBoardPieces()
	{
		var children = GetNode("Pieces").GetChildren();
		var pieces = new Array<Piece>();

		foreach (var child in children)
		{
			if (child is Piece)
			{
				pieces.Add(child as Piece);
			}
		}

		return pieces;
	}
	
	public void RotateBoard()
	{
		if (Utils.gameMode == Constants.GameMode.LOCAL_MULTIPLAYER)
		{
			var camera = GetNode<Camera2D>("/root/Game/Camera");
			var angleTo = 0;
			
			if (camera.RotationDegrees == 0)
			{
				angleTo = 180;
			}

			camera.RotationDegrees = angleTo;

			var pieces = GetBoardPieces();
			foreach (var piece in pieces)
			{
				piece.RotationDegrees = angleTo;
			}
		}
	}
	
	private void OnBoardPieceMoved()
	{
		Utils.SwapPlayerTurn();
		if (!GameOver())
		{
			RotateBoard();
		}
	}

	private void OnBoardPawnPromoted(Piece pawn)
	{
		var promotion = promotionScene.Instantiate<PromotionScene>();
		promotion.piece = pawn;
		
		GetNode("/root/Game/HUD").AddChild(promotion);
	}

	private void OnBoardCheckmate()
	{
		var end = endScene.Instantiate<EndScene>();
		end.endgameReason = "checkmate";
		end.player = Utils.PlayerToString(Utils.otherPlayerTurn);
		end.otherPlayer = Utils.PlayerToString(Utils.playerTurn);
		
		GetNode("/root/Game/HUD").AddChild(end);
	}

	private void OnBoardStalemate()
	{
		var end = endScene.Instantiate<EndScene>();
		end.endgameReason = "stalemate";
		end.player = Utils.PlayerToString(Utils.otherPlayerTurn);
		end.otherPlayer = Utils.PlayerToString(Utils.playerTurn);
		
		GetNode("/root/Game/HUD").AddChild(end);
	}
}
