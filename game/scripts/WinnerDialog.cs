using Godot;

namespace chessium.scripts;

public partial class WinnerDialog : Dialog
{
	public static readonly int winnerWidth = 54 * 2, winnerHeight = 32 * 2;
	
	private Sprite2D playerSprite = new (), winnerSprite = new ();
	private int player;

	public WinnerDialog(int winner) : base(winnerWidth, winnerHeight)
	{
		player = winner;
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready(); // remove if not working

		ConfigureSprite(playerSprite);
		ConfigureSprite(winnerSprite);

		playerSprite.FrameCoords = playerSprite.FrameCoords with { Y = player };
		winnerSprite.FrameCoords = winnerSprite.FrameCoords with { Y = 2 };

		var y = winnerSprite.Position.Y;
		winnerSprite.Position = winnerSprite.Position with { Y = y + 32 };
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// to be implemented
	}

	private void ConfigureSprite(Sprite2D sprite)
	{
		sprite.Texture = GD.Load<Texture2D>("res://assets/player.png");
		sprite.Centered = false;
		sprite.Hframes = 1;
		sprite.Vframes = 4;
		sprite.ZIndex = 10;
		sprite.Position = new Vector2(size, size);
		
		AddChild(sprite);
	}
}
