using Godot;
using Godot.Collections;

namespace chessium.scripts;

public partial class Board : Node2D
{
	private readonly Vector2 invalidTile = new (-1, -1);
	private Vector2 mouseTile, previousMouseTile;

	private Piece selectedPiece;
	private Vector2 selectedPiecePosition;
	private Array<Vector2> validMoves;
	private Dictionary<int, Piece> pieces;
	private Array<Vector2> kingPositions;

	private Root root;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		mouseTile = invalidTile;
		previousMouseTile = invalidTile;
		validMoves = new Array<Vector2>();
		pieces = new Dictionary<int, Piece>();
		kingPositions = new Array<Vector2>();
		root = GetParent<Root>();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var mouse = GetGlobalMousePosition();
		if (mouse.X > Constants.boardSize)
		{
			mouseTile = invalidTile;
		}
		else
		{
			mouseTile = new Vector2(Mathf.Floor(mouse.X / Constants.tileSize), Mathf.Floor(mouse.Y / Constants.tileSize));
		}

		if (mouseTile != previousMouseTile)
		{
			previousMouseTile = mouseTile;
			QueueRedraw();
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (root.gameState == Constants.awaiting)
		{
			return;
		}
		
		if(@event is not InputEventMouseButton)
		{
			return;
		}

		if (@event.IsPressed())
		{
			return;
		}

		if (root.gameState == Constants.piece)
		{
			InputPieceState((InputEventMouseButton) @event);
		}

		if (root.gameState == Constants.move)
		{
			InputMoveState((InputEventMouseButton) @event);
		}
	}

	public override void _Draw()
	{
		switch (root.gameState)
		{
			case 0: // grabbing a piece
				if (mouseTile == invalidTile)
				{
					return;
				}

				var piece = GetPieceFromVector2(mouseTile);
				if (piece != null)
				{
					if (piece.player == root.player)
					{
						DrawTileBorder(mouseTile, new Color(1, 1, 0));
					}
				}
				break;
			
			case 1: // making a move
				if (selectedPiecePosition != invalidTile)
				{
					DrawTileBorder(selectedPiecePosition, new Color(0, 1, 0));
				}

				foreach (var move in validMoves)
				{
					if (mouseTile == move)
					{
						DrawTileBorder(move, new Color(1, 1, 0));
					}
					else if (GetPieceFromVector2(move) != null)
					{
						if (GetPieceFromVector2(move).player == root.player)
						{
							DrawTileBorder(move, new Color(0, 1, 0));
						}
						else
						{
							DrawTileBorder(move, new Color(1, 0, 0));
						}
					}
					else
					{
						DrawTileBorder(move, new Color(0, 0, 1));
					}
				}
				break;
		}
	}

	public Piece GetPieceFromVector2(Vector2 position)
	{
		return GetPiece((int) position.X, (int) position.Y);
	}

	private Piece GetPiece(int x, int y)
	{
		return GetGrid(x, y, pieces);
	}

	private Piece GetGrid(int x, int y, Dictionary<int, Piece> dictionary)
	{
		if (x < 8 && x >= 0 && y < 8 && y >= 0)
		{
			if (dictionary.ContainsKey(CoordinatesToKey(x, y)))
			{
				var piece = dictionary[CoordinatesToKey(x, y)];
				if (piece != null)
				{
					return piece;
				}
			}
		}

		return null;
	}

	private void DrawTileBorder(Vector2 position, Color color)
	{
		DrawRect(new Rect2(new Vector2(position.X * Constants.tileSize, position.Y * Constants.tileSize), new Vector2(Constants.tileSize, Constants.tileSize)), color, false, 5);
	}

	public void ClearJumps()
	{
		foreach (var key in pieces.Keys)
		{
			if (pieces[key].player == root.player && pieces[key].type == Constants.pawn)
			{
				pieces[key].jumped = false;
			}
		}
	}

	private void InputPieceState(InputEventMouseButton @event)
	{
		if (mouseTile == invalidTile || @event.ButtonIndex != MouseButton.Left)
		{
			return;
		}

		var piece = GetPieceFromVector2(mouseTile);
		if (piece == null)
		{
			return;
		}

		if (piece.player == root.player)
		{
			if (piece.GetValidMovesFromVector2(mouseTile).Count > 0)
			{
				selectedPiece = piece;
				selectedPiecePosition = mouseTile;
				validMoves = piece.GetValidMovesFromVector2(mouseTile);

				root.gameState = Constants.move;
				QueueRedraw();
			}
		}
	}

	private void Deselect()
	{
		selectedPiece = null;
		selectedPiecePosition = invalidTile;
		validMoves = new Array<Vector2>();
		root.gameState = Constants.piece;
		
		QueueRedraw();
	}

	private void InputMoveState(InputEventMouseButton @event)
	{
		if ((@event.ButtonIndex == MouseButton.Right || mouseTile == invalidTile) && selectedPiece != null)
		{
			Deselect();
			return;
		}

		if (mouseTile == invalidTile)
		{
			return;
		}

		if (validMoves.IndexOf(mouseTile) != -1)
		{
			var mouseKey = CoordinatesToKey((int) mouseTile.X, (int) mouseTile.Y);
			var captured = false;

			if (pieces.ContainsKey(mouseKey) && pieces[mouseKey].player != root.player)
			{
				root.Capture(pieces[mouseKey]);
				RemoveChild(pieces[mouseKey]);
				
				pieces[mouseKey].QueueFree();
				pieces.Remove(mouseKey);
				captured = true;
			}

			if (selectedPiece != null && selectedPiece.type == Constants.king)
			{
				kingPositions[root.player] = mouseTile;
			}

			if (selectedPiece != null && selectedPiece.type == Constants.king && pieces.ContainsKey(mouseKey))
			{
				if (pieces[mouseKey].type == Constants.rook)
				{
					var direction = -Mathf.Sign(selectedPiecePosition.X - mouseTile.X);
					
					var king = selectedPiece;
					var rook = pieces[mouseKey];

					var kingPosition = new Vector2(selectedPiecePosition.X + direction * 2, selectedPiecePosition.Y);
					var kingPositionKey = CoordinatesToKey((int) kingPosition.X, (int) kingPosition.Y);

					var rookPosition = new Vector2(kingPosition.X - direction, kingPosition.Y);
					var rookPositionKey = CoordinatesToKey((int) rookPosition.X, (int) rookPosition.Y);

					pieces.Remove(mouseKey);
					pieces.Remove(CoordinatesToKey((int) selectedPiecePosition.X, (int) selectedPiecePosition.Y));

					pieces[kingPositionKey] = king;
					pieces[rookPositionKey] = rook;

					rook.moved = true;
					rook.Position = new Vector2(rookPosition.X * Constants.tileSize, rookPosition.Y * Constants.tileSize);
					
					king.Position = new Vector2(kingPosition.X * Constants.tileSize, kingPosition.Y * Constants.tileSize);
					kingPositions[root.player] = kingPosition;
				}
			}
			else
			{
				pieces.Remove(CoordinatesToKey((int) selectedPiecePosition.X, (int) selectedPiecePosition.Y));
				pieces[mouseKey] = selectedPiece;

				selectedPiece!.Position = new Vector2(mouseTile.X * Constants.tileSize, mouseTile.Y * Constants.tileSize);
			}

			if (selectedPiece.type == Constants.pawn && Mathf.Abs( (int) (selectedPiecePosition.X - mouseTile.X)) == 1 && !captured)
			{
				var xOffset = -(selectedPiecePosition.X - mouseTile.X);
				var enPassantOffset = new Vector2(selectedPiecePosition.X + xOffset, selectedPiecePosition.Y);
				var enPassantKey = CoordinatesToKey((int) enPassantOffset.X, (int) enPassantOffset.Y);

				root.Capture(pieces[enPassantKey]);
				RemoveChild(pieces[enPassantKey]);
				
				pieces[enPassantKey].QueueFree();
				pieces.Remove(enPassantKey);
			}

			selectedPiece.moved = true;

			if (selectedPiece.type == Constants.pawn)
			{
				var y = (int) mouseTile.Y;
				var canPromote = y == 7 || y == 0;

				if (canPromote)
				{
					PromotePawn(mouseTile);
				}

				if (Mathf.Abs((int) (mouseTile.Y - selectedPiecePosition.Y)) == 2)
				{
					selectedPiece.jumped = true;
				}
			}

			selectedPiece = null;
			selectedPiecePosition = invalidTile;
			validMoves = new Array<Vector2>();

			root.SwitchPlayer();

			if (!CheckCheckmate(root.player))
			{
				root.gameState = Constants.piece;
			}
			else
			{
				root.winner = Mathf.Abs(root.player - 1);

				var dialog = new WinnerDialog(root.winner);
				dialog.Position = new Vector2(Constants.boardSize / 2.0f - (WinnerDialog.winnerWidth - Dialog.size) / 2.0f, Constants.boardSize / 2.0f - (WinnerDialog.winnerHeight - Dialog.size) / 2.0f);
				
				root.AddChild(dialog);
				root.gameState = Constants.checkmate;
			}
			
			QueueRedraw();
		}
		else
		{
			Deselect();
		}
	}

	private void PromotePawn(Vector2 position)
	{
		root.gameState = Constants.awaiting;

		var dialog = new PromotionDialog(root.player);
		dialog.Position = new Vector2(Constants.boardSize / 2.0f - (PromotionDialog.promotionWidth - Dialog.size) / 2.0f, Constants.boardSize / 2.0f - (PromotionDialog.promotionHeight - Dialog.size) / 2.0f);
		root.AddChild(dialog);

		var newPiece = -1;
		dialog.OnSelected += (type) => { newPiece = type; };
		root.RemoveChild(dialog);
		
		var key = CoordinatesToKey((int) position.X, (int) position.Y);
		pieces[key].type = newPiece;
		pieces[key].UpdateSprite();
	}

	public void NewGame()
	{
		pieces = new Dictionary<int, Piece>();
		kingPositions = new Array<Vector2>();

		foreach (var child in GetChildren())
		{
			if (child is not Sprite2D)
			{
				RemoveChild(child);
				child.QueueFree();
			}
		}

		for (var i = 0; i < 2; i++)
		{
			for (var x = 0; x < 8; x++)
			{
				var y = 6 - 5 * i;
				var piece = new Piece(i, Constants.pawn);
				piece.Position = new Vector2(x * Constants.tileSize, y * Constants.tileSize);
				pieces[CoordinatesToKey(x, y)] = piece;
			}

			var types = new Array<int>
			{
				Constants.rook, Constants.knight, Constants.bishop, Constants.king, Constants.queen, Constants.bishop, Constants.knight, Constants.rook
			};
			for (var x = 0; x < 8; x++)
			{
				var y = 7 - 7 * i;
				var piece = new Piece(i, types[x]);
				piece.Position = new Vector2(x * Constants.tileSize, y * Constants.tileSize);

				if (piece.type == Constants.king)
				{
					kingPositions.Add(new Vector2(x, y));
				}

				pieces[CoordinatesToKey(x, y)] = piece;
			}
		}

		foreach (var pair in pieces)
		{
			AddChild(pair.Value);
		}
	}

	public bool WouldBeInCheck(int x, int y, Piece piece, Vector2 position)
	{
		var player = piece.player;
		var kingPosition = kingPositions[player];

		if (piece.type == Constants.king)
		{
			kingPosition = position;
		}

		var boardState = pieces.Duplicate();
		boardState.Remove(CoordinatesToKey(x, y));
		boardState[CoordinatesToKey((int) position.X, (int) position.Y)] = piece;

		return IsKingInCheck(player, boardState, kingPosition);
	}

	public bool WouldBeInCheckFromCastling(Vector2 currentPosition, Vector2 castlePosition, Vector2 newPosition, Vector2 newCastlePosition)
	{
		var king = GetPieceFromVector2(currentPosition);
		var rook = GetPieceFromVector2(castlePosition);

		var boardState = pieces.Duplicate();
		boardState.Remove(CoordinatesToKey((int) currentPosition.X, (int) currentPosition.Y));
		boardState.Remove(CoordinatesToKey((int) castlePosition.X, (int) castlePosition.Y));

		boardState[CoordinatesToKey((int) newPosition.X, (int) newPosition.Y)] = king;
		boardState[CoordinatesToKey((int) newCastlePosition.X, (int) newCastlePosition.Y)] = rook;

		return IsKingInCheck(king.player, boardState, newPosition);
	}

	private bool IsKingInCheck(int player, Dictionary<int, Piece> boardState, Vector2 kingPosition)
	{
		// Check if pawns can capture the king
		var y = -1 + (player * 2);
		for (var i = 0; i < 2; i++)
		{
			var x = -1 + (i * 2);
			var piece = GetGrid((int) (kingPosition.X + x), (int) (kingPosition.Y + y), boardState);
			if (piece != null && piece.type == Constants.pawn && piece.player != player)
			{
				return true;
			}
		}
		
		// Check if knights can capture the king
		foreach (var direction in Constants.knightDirections)
		{
			var piece = GetGrid((int) (kingPosition.X + direction.X), (int) (kingPosition.Y + direction.Y), boardState);
			if (piece != null && piece.type == Constants.knight && piece.player != player)
			{
				return true;
			}
		}
		
		// Check to prevent a king from moving too close to another king
		foreach (var direction in Constants.allDirections)
		{
			var piece = GetGrid((int) (kingPosition.X + direction.X), (int) (kingPosition.Y + direction.Y), boardState);
			if (piece != null && piece.type == Constants.king && piece.player != player)
			{
				return true;
			}
		}
		
		// Check if a queen, bishop or rook can capture the king
		var directionList = new Array<Array<Vector2>> { Constants.allDirections, Constants.rookDirections, Constants.bishopDirections };
		var pieceList = new Array<int> { Constants.queen, Constants.rook, Constants.bishop };

		for (var i = 0; i < directionList.Count; i++)
		{
			var directions = directionList[i];
			var piece = pieceList[i];

			foreach (var direction in directions)
			{
				var multi = 1;
				while (multi < 8)
				{
					var king = GetGrid((int) (kingPosition.X + direction.X * multi), (int) (kingPosition.Y + direction.Y * multi), boardState);
					if (king != null)
					{
						if (king.type == piece && king.player != player)
						{
							return true;
						}

						if (king.type != piece || king.player == player)
						{
							break;
						}
					}

					multi++;
				}
			}
		}
		
		return false;
	}

	private bool CheckCheckmate(int player)
	{
		foreach (var pair in pieces)
		{
			var piece = pair.Value;
			if (piece.player == player)
			{
				if (piece.GetValidMoves((int) piece.Position.X / Constants.tileSize, (int) piece.Position.Y / Constants.tileSize).Count > 0)
				{
					return false;
				}
			}
		}

		return true;
	}

	private int CoordinatesToKey(int x, int y)
	{
		return (x << 3) + y;
	}
}
