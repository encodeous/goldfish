using Godot;

namespace chessium.scripts;

public partial class PieceSlot : Node2D
{
	[Export] private bool black;
	private int player, index;

	private Sprite2D sprite;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (black)
		{
			player = 1;
		}

		sprite = GetNode<Sprite2D>("Sprite2D");
		sprite.QueueFree();
		
		RemoveChild(sprite);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// to be implemented
	}

	public void NewGame()
	{
		foreach (var child in GetChildren())
		{
			child.QueueFree();
			RemoveChild(child);
		}
	}

	public void AddPiece(int piece)
	{
		var pieceSprite = new Sprite2D();
		pieceSprite.Centered = false;
		pieceSprite.Position = pieceSprite.Position with { X = index * Constants.tileSize / 4.0f };
		pieceSprite.Texture = GD.Load<Texture2D>("res://assets/pieces.png");
		pieceSprite.Hframes = 6;
		pieceSprite.Vframes = 2;
		pieceSprite.ZIndex = 1000 + index;
		pieceSprite.FrameCoords = new Vector2I(piece, player);

		index++;
		AddChild(pieceSprite);
	}
}
