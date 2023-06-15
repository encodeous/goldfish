using System;
using System.Collections.Generic;
using Godot;
using Godot.Collections;
using goldfish.Core.Data;
using goldfish.Core.Game;
using Side = goldfish.Core.Data.Side;

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
	/// Represents the player who owns the piece.
	/// TODO: refactor with adam's code
	/// </summary>
	public Side player;
	
	/// <summary>
	/// Represents the type of the piece.
	/// TODO: refactor with adam's code
	/// </summary>
	public PieceType type;
	
	/// <summary>
	/// Constructs a new Piece instance.
	/// TODO: refactor with adam's code
	/// </summary>
	/// <param name="player">The player who owns the piece.</param>
	/// <param name="type">The type of piece.</param>
	public Piece(Side player, PieceType type)
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
	/// Gets all valid moves for this piece, depending on its type.
	/// </summary>
	/// <param name="x">The row of the piece.</param>
	/// <param name="y">The column of the piece.</param>
	/// <returns>An array of valid moves.</returns>
	public List<ChessMove> GetValidMoves(int x, int y)
	{
		Span<ChessMove> positions = stackalloc ChessMove[35];
		board.state.GetValidMovesForSquare(x, y, positions, false);

		return new List<ChessMove>(positions.ToArray());
	}

	/// <summary>
	/// Gets all valid moves for a piece from a Vector2 position.
	/// TODO: refactor with adam's code
	/// </summary>
	/// <param name="vector">The position of the piece.</param>
	/// <returns>An array of valid moves.</returns>
	public List<ChessMove> GetValidMovesFromVector2(Vector2 vector)
	{
		return GetValidMoves((int) vector.X, (int) vector.Y);
	}
	
	/// <summary>
	/// Checks if a position is within the bounds of the chess board.
	/// </summary>
	/// <param name="position">The position to check.</param>
	/// <returns>True if the position is within the chess board, false otherwise.</returns>
	private bool IsInBounds(Vector2 position)
	{
		// is this position with the board grid (8 * 8)?
		return position.X is < 8 and >= 0 && position.Y is < 8 and >= 0;
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
		sprite.FrameCoords = new Vector2I((int) type, (int) player);
	}
}
