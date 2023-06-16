using System.Collections.Generic;
using System.Linq;
using Godot;
using goldfish.Core.Data;
using goldfish.Core.Game;
using Side = goldfish.Core.Data.Side;

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
	private List<ChessMove> validMoves;
	
	/// <summary>
	/// Maps a piece to an index that corresponds with a piece's position.
	/// </summary>
	private Dictionary<int, Piece> pieces;

	/// <summary>
	/// Represents the Root scene.
	/// </summary>
	private Root root;

	/// <summary>
	/// Is the user holding left click down?
	/// </summary>
	private bool heldDown;

	/// <summary>
	/// The current state of the board.
	/// </summary>
	public ChessState state;

	/// <summary>
	/// Is the board flipped?
	/// </summary>
	private bool isBoardFlipped = true;
	
	/// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		mouseTile = invalidTile;
		previousMouseTile = invalidTile;
		
		validMoves = new List<ChessMove>();
		pieces = new Dictionary<int, Piece>();
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
		if(@event is not InputEventMouseButton button)
		{
			// only focus on events concerning the user's mouse
			return;
		}

		switch (root.gameState)
		{
			case Constants.GameState.WAITING_FOR_USER:
				// are we waiting for a user to pick an option (for example, pawn promotion)?
				return;
			
			case Constants.GameState.GETTING_PIECE:
				// has a user clicked a piece? if so, handle it
				if (!heldDown)
				{
					HandlePieceClick(button);
				}
				break;
			
			case Constants.GameState.MAKING_A_MOVE:
				// is the user making a move with a piece? if so, handle it
				HandlePieceMove(button);
				break;
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

				foreach (var mov in validMoves)
				{
					// draw a border around the possible move being hovered over
					var move = mov.NewPos.ToVector();
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
		return new Vector2(Mathf.Floor(position.Y / Constants.tileSize), Mathf.Floor(position.X / Constants.tileSize));
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
		DrawRect(new Rect2(new Vector2(position.Y * Constants.tileSize, position.X * Constants.tileSize), new Vector2(Constants.tileSize, Constants.tileSize)), color, false, 5);
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

		if (piece.player == state.ToMove && piece.GetValidMovesFromVector2(mouseTile).Any())
		{
			// we're good for making a move, set relevant vars to the needed values to do so
			selectedPiece = piece;
			selectedPiecePosition = mouseTile;
			validMoves = piece.GetValidMovesFromVector2(mouseTile);
				
			heldDown = true;

			root.gameState = Constants.GameState.MAKING_A_MOVE;
			QueueRedraw();
		}
	}

	/// <summary>
	/// Deselects a piece (the user clicked on a different piece or right clicked on the same piece).
	/// </summary>
	private void Deselect()
	{
		selectedPiece = null;
		selectedPiecePosition = invalidTile;
		validMoves = new List<ChessMove>();
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
			heldDown = false;
			return;
		}

		if (mouseTile == invalidTile)
		{
			heldDown = false;
			return;
		}

		// did the user drag the piece to a valid move location?
		if (validMoves.Any(move => move.NewPos.ToVector() == mouseTile))
		{
			var chessMove = validMoves.First(move => move.NewPos.ToVector() == mouseTile);

			if (chessMove.Taken is not null)
			{
				var capturePosition = CoordinatesToKey(chessMove.Taken.Value.Item1, chessMove.Taken.Value.Item2);
				if (chessMove.IsCastle)
				{
					var (rx, ry) = chessMove.Castle!.Value.GetCastleRookPos();
					var (tx, ty) = chessMove.Taken.Value;
					
					var rook = pieces[CoordinatesToKey(rx, ry)];
					rook.Position = new Vector2(ty * Constants.tileSize, tx * Constants.tileSize);
					pieces[CoordinatesToKey(tx, ty)] = rook;
				}
				else
				{
					root.Capture(pieces[capturePosition]);
					RemoveChild(pieces[capturePosition]);

					pieces[capturePosition].QueueFree();
					pieces.Remove(capturePosition);
				}
			}

			var (nx, ny) = chessMove.NewPos;
			var (ox, oy) = chessMove.OldPos;
			
			pieces.Remove(CoordinatesToKey(ox, oy));
			pieces[CoordinatesToKey(nx, ny)] = selectedPiece;

			selectedPiece!.Position = new Vector2(ny * Constants.tileSize, nx * Constants.tileSize);

			// handle pawns trying to promote
			if (chessMove.IsPromotion)
			{
				PromotePawn(mouseTile);
			}
			
			// reset all states after move has been completed
			selectedPiece = null;
			selectedPiecePosition = invalidTile;
			validMoves = new List<ChessMove>();

			// make sure the game should still be continuing, end it if not
			if (root.winner is null)
			{
				root.gameState = Constants.GameState.GETTING_PIECE;
			}
			else
			{
				var dialog = new WinnerDialog(root.winner.Value);
				dialog.Position = new Vector2(Constants.boardSize / 2.0f - (WinnerDialog.winnerWidth - Dialog.size) / 2.0f, Constants.boardSize / 2.0f - (WinnerDialog.winnerHeight - Dialog.size) / 2.0f);
				
				root.AddChild(dialog);
				root.gameState = Constants.GameState.CHECKMATE;
			}

			state = chessMove.NewState;
			heldDown = false;
			
			root.SwitchPlayer();
			QueueRedraw();
		}
		else
		{
			heldDown = false;
			Deselect();
		}
	}

	/// <summary>
	/// Handle the promotion of a pawn.
	/// TODO: fix event not firing
	/// </summary>
	/// <param name="position">The position of the promoting pawn.</param>
	private void PromotePawn(Vector2 position)
	{
		root.gameState = Constants.GameState.WAITING_FOR_USER;

		var dialog = new PromotionDialog(root.player);
		dialog.Position = new Vector2(Constants.boardSize / 2.0f - (PromotionDialog.promotionWidth - Dialog.size) / 2.0f, Constants.boardSize / 2.0f - (PromotionDialog.promotionHeight - Dialog.size) / 2.0f);
		root.AddChild(dialog);

		// capture user's choice of piece type
		var newPiece = PromotionType.Queen;
		dialog.OnSelected += type => { newPiece = (PromotionType) type; };
		root.RemoveChild(dialog);
		
		// update the pawn to its new sprite & type
		var key = CoordinatesToKey((int) position.X, (int) position.Y);
		pieces[key].type = (PieceType) newPiece;
		pieces[key].UpdateSprite();
		state.Promote(((int) position.X, (int) position.Y), newPiece);
	}

	/// <summary>
	/// Prepares the board for a new game (resets piece positions & sprites).
	/// </summary>
	public void NewGame()
	{
		state = ChessState.DefaultState();
		pieces = new Dictionary<int, Piece>();
		
		foreach (var child in GetChildren())
		{
			if (child is not Sprite2D)
			{
				RemoveChild(child);
				child.QueueFree();
			}
		}

		for (var x = 0; x < 8; x++)
		{
			for (var y = 0; y < 8; y++)
			{
				var dat = state.GetPiece(isBoardFlipped ? 7 - x : x, y);
				if(dat.IsPieceType(PieceType.Space)) continue;
				var piece = new Piece(dat.GetSide(), dat.GetPieceType());
				piece.Position = new Vector2(y * Constants.tileSize, x * Constants.tileSize);
				pieces[CoordinatesToKey(x, y)] = piece;
			}
		}

		foreach (var pair in pieces)
		{
			AddChild(pair.Value);
		}
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
