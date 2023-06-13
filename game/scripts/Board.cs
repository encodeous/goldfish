using Godot;
using Godot.Collections;

namespace chessium.scripts;

/// <summary>
/// Represents the chess board.
/// </summary>
public partial class Board : Node2D
{
	/// <summary>
	/// Represents an invalid tile (one that is not on the board).
	/// </summary>
	private readonly Vector2 invalidTile = new (-1, -1);
	
	/// <summary>
	/// Represents the tile that the mouse has selected and is currently selecting.
	/// </summary>
	private Vector2 mouseTile, previousMouseTile;

	/// <summary>
	/// The piece selected by the user's mouse.
	/// </summary>
	private Piece selectedPiece;
	
	/// <summary>
	/// The position of the piece selected by the user's mouse.
	/// </summary>
	private Vector2 selectedPiecePosition;
	
	/// <summary>
	/// Holds a list of all valid move for a selected piece.
	/// </summary>
	private Array<Vector2> validMoves;
	
	/// <summary>
	/// Maps a piece to an index that corresponds with a piece's position.
	/// </summary>
	private Dictionary<int, Piece> pieces;
	
	/// <summary>
	/// A list of positions of the King piece.
	/// </summary>
	private Array<Vector2> kingPositions;

	/// <summary>
	/// Represents the Root scene.
	/// </summary>
	private Root root;
	
	/// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		mouseTile = invalidTile;
		previousMouseTile = invalidTile;
		validMoves = new Array<Vector2>();
		pieces = new Dictionary<int, Piece>();
		kingPositions = new Array<Vector2>();
		root = GetParent<Root>();
	}

	/// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var mouse = GetGlobalMousePosition();
		// sets the current mouse tile to where the user is hovering their mouse (or has clicked)
		mouseTile = mouse.X > Constants.boardSize ? invalidTile : MapGlobalCoordsToBoard(mouse);

		if (mouseTile == previousMouseTile)
		{
			return;
		}
		
		// redraw borders if the mouse position has changed
		previousMouseTile = mouseTile;
		QueueRedraw();
	}

	/// <summary>
	/// Handles user input.
	/// </summary>
	/// <param name="event">Any user input.</param>
	public override void _Input(InputEvent @event)
	{
		if (root.gameState == Constants.GameState.WAITING_FOR_USER)
		{
			// are we waiting for a user to pick an option (for example, pawn promotion)?
			return;
		}
		
		if(@event is not InputEventMouseButton button)
		{
			// only focus on events concerning the user's mouse
			return;
		}

		if (button.Pressed)
		{
			return;
		}

		if (root.gameState == Constants.GameState.GETTING_PIECE)
		{
			// has a user clicked a piece? if so, handle it
			HandlePieceClick(button);
		}

		if (root.gameState == Constants.GameState.MAKING_A_MOVE)
		{
			// is the user making a move with a piece? if so, handle it
			HandlePieceMove(button);
		}
	}

	/// <summary>
	/// Called when the Node is being drawn to the Scene.
	/// </summary>
	public override void _Draw()
	{
		switch (root.gameState)
		{
			case Constants.GameState.GETTING_PIECE: // grabbing a piece
				if (mouseTile == invalidTile)
				{
					return;
				}

				var piece = GetPieceFromVector2(mouseTile);
				if (piece != null)
				{
					if (piece.player == root.player)
					{
						// draw a border around the piece being hovered over
						DrawTileWithBorder(mouseTile, new Color(1, 1, 0));
					}
				}
				break;
			
			case Constants.GameState.MAKING_A_MOVE: // making a move
				if (selectedPiecePosition != invalidTile)
				{
					DrawTileWithBorder(selectedPiecePosition, new Color(0, 1, 0));
				}

				foreach (var move in validMoves)
				{
					// draw a border around the possible move being hovered over
					if (mouseTile == move)
					{
						DrawTileWithBorder(move, new Color(1, 1, 0));
					}
					else if (GetPieceFromVector2(move) != null)
					{
						if (GetPieceFromVector2(move).player == root.player)
						{
							DrawTileWithBorder(move, new Color(0, 1, 0));
						}
						else
						{
							DrawTileWithBorder(move, new Color(1, 0, 0));
						}
					}
					else
					{
						DrawTileWithBorder(move, new Color(0, 0, 1));
					}
				}
				break;
		}
	}

	private Vector2 MapGlobalCoordsToBoard(Vector2 position)
	{
		return new Vector2(Mathf.Floor(position.X / Constants.tileSize), Mathf.Floor(position.Y / Constants.tileSize));
	}

	/// <summary>
	/// Gets the piece from a Vector2 position.
	/// </summary>
	/// <param name="position">The position to fetch from.</param>
	/// <returns>The piece at the position, if any.</returns>
	public Piece GetPieceFromVector2(Vector2 position)
	{
		return GetPiece((int) position.X, (int) position.Y);
	}

	/// <summary>
	/// Gets the piece from an (x, y) coordinate pair.
	/// </summary>
	/// <param name="x">The row to fetch from.</param>
	/// <param name="y">The column to fetch from.</param>
	/// <returns>The piece at the position, if any.</returns>
	private Piece GetPiece(int x, int y)
	{
		return GetPieceFromGrid(x, y, pieces);
	}

	/// <summary>
	/// Gets the piece from the pieces stored in the game board.
	/// </summary>
	/// <param name="x">The row to fetch from.</param>
	/// <param name="y">The column to fetch from.</param>
	/// <param name="dictionary">The list of pieces currently present on the board.</param>
	/// <returns>The piece at the position, if any.</returns>
	private Piece GetPieceFromGrid(int x, int y, Dictionary<int, Piece> dictionary)
	{
		if (x is < 8 and >= 0 && y is < 8 and >= 0)
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

	/// <summary>
	/// Draws a border around a tile on the board.
	/// </summary>
	/// <param name="position">The position of the tile.</param>
	/// <param name="color">The color of the border.</param>
	private void DrawTileWithBorder(Vector2 position, Color color)
	{
		DrawRect(new Rect2(new Vector2(position.X * Constants.tileSize, position.Y * Constants.tileSize), new Vector2(Constants.tileSize, Constants.tileSize)), color, false, 5);
	}

	/// <summary>
	/// Resets each piece's "jumped" attribute.
	/// </summary>
	public void ClearJumps()
	{
		foreach (var key in pieces.Keys)
		{
			if (pieces[key].player == root.player && pieces[key].type == Constants.Pieces.PAWN)
			{
				pieces[key].jumped = false;
			}
		}
	}

	/// <summary>
	/// Handles left clicks on a piece (selection).
	/// </summary>
	/// <param name="event">The click event.</param>
	private void HandlePieceClick(InputEventMouseButton @event)
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
				// we're good for making a move, set relevant vars to the needed values to do so
				selectedPiece = piece;
				selectedPiecePosition = mouseTile;
				validMoves = piece.GetValidMovesFromVector2(mouseTile);

				root.gameState = Constants.GameState.MAKING_A_MOVE;
				QueueRedraw();
			}
		}
	}

	/// <summary>
	/// Deselects a piece (the user clicked on a different piece or right clicked on the same piece).
	/// </summary>
	private void Deselect()
	{
		selectedPiece = null;
		selectedPiecePosition = invalidTile;
		validMoves = new Array<Vector2>();
		root.gameState = Constants.GameState.GETTING_PIECE;
		
		QueueRedraw();
	}

	/// <summary>
	/// Handles right clicks and mouse dragging (user deselected or is moving a piece).
	/// </summary>
	/// <param name="event">The click or drag event.</param>
	private void HandlePieceMove(InputEventMouseButton @event)
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

		// did the user drag the piece to a valid move location?
		if (validMoves.IndexOf(mouseTile) != -1)
		{
			var mouseKey = CoordinatesToKey((int) mouseTile.X, (int) mouseTile.Y);
			var captured = false;

			if (pieces.ContainsKey(mouseKey) && pieces[mouseKey].player != root.player)
			{
				// capture a piece, if there is one to be captured safely
				root.Capture(pieces[mouseKey]);
				RemoveChild(pieces[mouseKey]);
				
				pieces[mouseKey].QueueFree();
				pieces.Remove(mouseKey);
				
				captured = true;
			}

			if (selectedPiece != null && selectedPiece.type == Constants.Pieces.KING)
			{
				// update the position of the king of the current player
				kingPositions[(int) root.player] = mouseTile;
			}

			if (selectedPiece is { type: Constants.Pieces.KING } && pieces.ContainsKey(mouseKey))
			{
				// are we castling?
				if (pieces[mouseKey].type == Constants.Pieces.ROOK)
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
					kingPositions[(int) root.player] = kingPosition;
				}
			}
			else
			{
				// move the selected to piece to its new position
				pieces.Remove(CoordinatesToKey((int) selectedPiecePosition.X, (int) selectedPiecePosition.Y));
				pieces[mouseKey] = selectedPiece;

				selectedPiece!.Position = new Vector2(mouseTile.X * Constants.tileSize, mouseTile.Y * Constants.tileSize);
			}

			// can a pawn perform an en passant capture?
			if (selectedPiece.type == Constants.Pieces.PAWN && Mathf.Abs( (int) (selectedPiecePosition.X - mouseTile.X)) == 1 && !captured)
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

			// handle pawns trying to promote
			if (selectedPiece.type == Constants.Pieces.PAWN)
			{
				var y = (int) mouseTile.Y;
				var canPromote = y is 7 or 0;

				if (canPromote)
				{
					PromotePawn(mouseTile);
				}

				if (Mathf.Abs((int) (mouseTile.Y - selectedPiecePosition.Y)) == 2)
				{
					selectedPiece.jumped = true;
				}
			}

			// reset all states after move has been completed
			selectedPiece = null;
			selectedPiecePosition = invalidTile;
			validMoves = new Array<Vector2>();

			root.SwitchPlayer();

			// make sure the game should still be continuing, end it if not
			if (!CheckCheckmate(root.player))
			{
				root.gameState = Constants.GameState.GETTING_PIECE;
			}
			else
			{
				root.winner = root.player == Constants.Player.WHITE ? Constants.Player.BLACK : Constants.Player.WHITE;

				var dialog = new WinnerDialog(root.winner);
				dialog.Position = new Vector2(Constants.boardSize / 2.0f - (WinnerDialog.winnerWidth - Dialog.size) / 2.0f, Constants.boardSize / 2.0f - (WinnerDialog.winnerHeight - Dialog.size) / 2.0f);
				
				root.AddChild(dialog);
				root.gameState = Constants.GameState.CHECKMATE;
			}
			
			QueueRedraw();
		}
		else
		{
			Deselect();
		}
	}

	/// <summary>
	/// Handle the promotion of a pawn.
	/// </summary>
	/// <param name="position">The position of the promoting pawn.</param>
	private void PromotePawn(Vector2 position)
	{
		root.gameState = Constants.GameState.WAITING_FOR_USER;

		var dialog = new PromotionDialog(root.player);
		dialog.Position = new Vector2(Constants.boardSize / 2.0f - (PromotionDialog.promotionWidth - Dialog.size) / 2.0f, Constants.boardSize / 2.0f - (PromotionDialog.promotionHeight - Dialog.size) / 2.0f);
		root.AddChild(dialog);

		// capture user's choice of piece type
		var newPiece = Constants.Pieces.QUEEN;
		dialog.OnSelected += type => { newPiece = type; };
		root.RemoveChild(dialog);
		
		// update the pawn to its new sprite & type
		var key = CoordinatesToKey((int) position.X, (int) position.Y);
		pieces[key].type = newPiece;
		pieces[key].UpdateSprite();
	}

	/// <summary>
	/// Prepares the board for a new game (resets piece positions & sprites).
	/// </summary>
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
				var piece = new Piece((Constants.Player) i, Constants.Pieces.PAWN);
				piece.Position = new Vector2(x * Constants.tileSize, y * Constants.tileSize);
				pieces[CoordinatesToKey(x, y)] = piece;
			}

			var types = new Array<Constants.Pieces>
			{
				Constants.Pieces.ROOK, Constants.Pieces.KNIGHT, Constants.Pieces.BISHOP, Constants.Pieces.KING, Constants.Pieces.QUEEN, Constants.Pieces.BISHOP, Constants.Pieces.KNIGHT, Constants.Pieces.ROOK
			};
			for (var x = 0; x < 8; x++)
			{
				var y = 7 - 7 * i;
				var piece = new Piece((Constants.Player) i, types[x]);
				piece.Position = new Vector2(x * Constants.tileSize, y * Constants.tileSize);

				if (piece.type == Constants.Pieces.KING)
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

	/// <summary>
	/// Checks if a move were to put a king in check.
	/// </summary>
	/// <param name="x">The row of the piece being moved.</param>
	/// <param name="y">The column of the piece being moved.</param>
	/// <param name="piece">The piece being moved.</param>
	/// <param name="position">The position of the piece being moved to.</param>
	/// <returns>True if the king were to be in check, false otherwise.</returns>
	public bool WouldBeInCheck(int x, int y, Piece piece, Vector2 position)
	{
		var player = piece.player;
		var kingPosition = kingPositions[(int) player];

		if (piece.type == Constants.Pieces.KING)
		{
			kingPosition = position;
		}

		// create a temporary position to check
		var boardState = pieces.Duplicate();
		boardState.Remove(CoordinatesToKey(x, y));
		boardState[CoordinatesToKey((int) position.X, (int) position.Y)] = piece;

		return IsKingInCheck(player, boardState, kingPosition);
	}

	/// <summary>
	/// Checks if castling were to put a king in check.
	/// </summary>
	/// <param name="currentPosition">The king's current position.</param>
	/// <param name="castlePosition">The rook's current position.</param>
	/// <param name="newPosition">The king's new position after castling.</param>
	/// <param name="newCastlePosition">The rook's new position after castling.</param>
	/// <returns>True if the king were to be in check, false otherwise.</returns>
	public bool WouldBeInCheckFromCastling(Vector2 currentPosition, Vector2 castlePosition, Vector2 newPosition, Vector2 newCastlePosition)
	{
		var king = GetPieceFromVector2(currentPosition);
		var rook = GetPieceFromVector2(castlePosition);

		// create a temporary position to check
		var boardState = pieces.Duplicate();
		boardState.Remove(CoordinatesToKey((int) currentPosition.X, (int) currentPosition.Y));
		boardState.Remove(CoordinatesToKey((int) castlePosition.X, (int) castlePosition.Y));

		boardState[CoordinatesToKey((int) newPosition.X, (int) newPosition.Y)] = king;
		boardState[CoordinatesToKey((int) newCastlePosition.X, (int) newCastlePosition.Y)] = rook;

		return IsKingInCheck(king.player, boardState, newPosition);
	}

	/// <summary>
	/// Checks if the king is currently in check.
	/// </summary>
	/// <param name="player">The player who is currently in check.</param>
	/// <param name="boardState">The current state of the board.</param>
	/// <param name="kingPosition">The current position of the player's king.</param>
	/// <returns>True if the king is in check, flase otherwise.</returns>
	private bool IsKingInCheck(Constants.Player player, Dictionary<int, Piece> boardState, Vector2 kingPosition)
	{
		// check if pawns can capture the king
		var y = -1 + (int) player * 2;
		for (var i = 0; i < 2; i++)
		{
			var x = -1 + (i * 2);
			var piece = GetPieceFromGrid((int) (kingPosition.X + x), (int) (kingPosition.Y + y), boardState);
			if (piece is { type: Constants.Pieces.PAWN } && piece.player != player)
			{
				return true;
			}
		}
		
		// check if knights can capture the king
		foreach (var direction in Constants.knightDirections)
		{
			var piece = GetPieceFromGrid((int) (kingPosition.X + direction.X), (int) (kingPosition.Y + direction.Y), boardState);
			if (piece is { type: Constants.Pieces.KNIGHT } && piece.player != player)
			{
				return true;
			}
		}
		
		// check to prevent a king from moving too close to another king
		foreach (var direction in Constants.allDirections)
		{
			var piece = GetPieceFromGrid((int) (kingPosition.X + direction.X), (int) (kingPosition.Y + direction.Y), boardState);
			if (piece is { type: Constants.Pieces.KING } && piece.player != player)
			{
				return true;
			}
		}
		
		// check if a queen, bishop or rook can capture the king
		var directionList = new Array<Array<Vector2>> { Constants.allDirections, Constants.rookDirections, Constants.bishopDirections };
		var pieceList = new Array<Constants.Pieces> { Constants.Pieces.QUEEN, Constants.Pieces.ROOK, Constants.Pieces.BISHOP };

		for (var i = 0; i < directionList.Count; i++)
		{
			var directions = directionList[i];
			var piece = pieceList[i];

			foreach (var direction in directions)
			{
				var multi = 1;
				while (multi < 8)
				{
					var king = GetPieceFromGrid((int) (kingPosition.X + direction.X * multi), (int) (kingPosition.Y + direction.Y * multi), boardState);
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

	/// <summary>
	/// Checks if the game has ended by checkmate.
	/// </summary>
	/// <param name="player">The player who might've gotten checkmated.</param>
	/// <returns>True if it's checkmate, false otherwise.</returns>
	private bool CheckCheckmate(Constants.Player player)
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

	/// <summary>
	/// Converts (x, y) coordinates to unique keys for accessing the current board state.
	/// </summary>
	/// <param name="x">The row of a piece.</param>
	/// <param name="y">The column of a piece.</param>
	/// <returns>An int that represents a unique value pertaining to a piece on the board.</returns>
	private int CoordinatesToKey(int x, int y)
	{
		return (x << 3) + y;
	}
}
