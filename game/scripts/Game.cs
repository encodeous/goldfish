using System;
using Godot;
using Godot.Collections;

namespace chessium.scripts;

public partial class Game : Node2D
{
	/// The instance of the game board.
	private Board board;
	/// A modified FEN position with default pieces, used to initialize the board to a default position.
	private readonly string startingFen = "pppppppprnbqkbnr/PPPPPPPPRNBQKBNR";

	/// An array of vectors that hold the default positions of all black pieces.
	private readonly Array<Vector2> blackPositions = new ()
	{
		new Vector2(0, 6),
		new Vector2(1, 6),
		new Vector2(2, 6),
		new Vector2(3, 6),
		new Vector2(4, 6),
		new Vector2(5, 6),
		new Vector2(6, 6),
		new Vector2(7, 6),
		
		new Vector2(0, 7),
		new Vector2(1, 7),
		new Vector2(2, 7),
		new Vector2(3, 7),
		new Vector2(4, 7),
		new Vector2(5, 7),
		new Vector2(6, 7),
		new Vector2(7, 7)
	};
	
	/// An array of vectors that hold the default positions of all white pieces.
	private readonly Array<Vector2> whitePositions = new ()
	{
		new Vector2(7, 1),
		new Vector2(1, 1),
		new Vector2(2, 1),
		new Vector2(3, 1),
		new Vector2(4, 1),
		new Vector2(5, 1),
		new Vector2(6, 1),
		new Vector2(7, 1),
		
		new Vector2(0, 0),
		new Vector2(1, 0),
		new Vector2(2, 0),
		new Vector2(3, 0),
		new Vector2(4, 0),
		new Vector2(5, 0),
		new Vector2(6, 0),
		new Vector2(7, 0)
	};
	
	/// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		board = GetNode<Board>("Board");
		SetBoard();

		var pieces = board.GetBoardPieces();
		board.SnapPiecesToPosition(pieces);
	}

	/// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// to be implemented
	}

	/// Parses a FEN string to set the default positions and types of pieces on the game board.
	private void SetBoard()
	{
		var pieces = startingFen.Split("/");
		var black = pieces[0];
		var white = pieces[1];

		var idx = 0;
		foreach (var piece in black)
		{
			var type = piece switch
			{
				'p' => Constants.PieceType.PAWN,
				'r' => Constants.PieceType.ROOK,
				'n' => Constants.PieceType.KNIGHT,
				'b' => Constants.PieceType.BISHOP,
				'k' => Constants.PieceType.KING,
				'q' => Constants.PieceType.QUEEN
			};
			
			board.AddPieceToBoard(Constants.Player.BLACK, type, blackPositions[idx]);
			idx++;
		}
		
		idx = 0;
		foreach (var piece in white)
		{
			var type = piece switch
			{
				'P' => Constants.PieceType.PAWN,
				'R' => Constants.PieceType.ROOK,
				'N' => Constants.PieceType.KNIGHT,
				'B' => Constants.PieceType.BISHOP,
				'K' => Constants.PieceType.KING,
				'Q' => Constants.PieceType.QUEEN
			};
			
			board.AddPieceToBoard(Constants.Player.WHITE, type, whitePositions[idx]);
			idx++;
		}
		
		var random = new Random();
		switch (StartScene.chosenColor)
		{
			case "white":
				Utils.SetPlayerTurn(Constants.Player.WHITE);
				break;
			
			case "black":
				Utils.SetPlayerTurn(Constants.Player.BLACK);
				board.RotateBoard();
				break;
			
			case "random":
				var num = random.Next(2);
				if (num < 1)
				{
					Utils.SetPlayerTurn(Constants.Player.BLACK);
					board.RotateBoard();
				}
				else
				{
					Utils.SetPlayerTurn(Constants.Player.WHITE);
				}
				break;
		}
	}
}
