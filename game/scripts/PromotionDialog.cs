using Godot;
using Godot.Collections;

namespace chessium.scripts;

public partial class PromotionDialog : Dialog
{
	[Signal] public delegate void OnSelectedEventHandler(int type);

	public static readonly float promotionWidth = Constants.tileSize * 4.0f;
	public static readonly float promotionHeight = Constants.tileSize;

	private Array<int> pieces = new () { Constants.rook, Constants.knight, Constants.bishop, Constants.queen };

	private readonly Vector2 invalidPosition = new (-1, -1);
	private Vector2 mousePosition, lastMousePosition;

	private int player;

	public PromotionDialog(int player) : base((int) promotionWidth, (int) promotionHeight)
	{
		this.player = player;
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready(); // remove if not working
		
		mousePosition = invalidPosition;
		lastMousePosition = invalidPosition;

		var i = 0;
		foreach (var piece in pieces)
		{
			var sprite = new Sprite2D();
			sprite.Texture = GD.Load<Texture2D>("res://assets/pieces.png");
			sprite.Hframes = 6;
			sprite.Vframes = 2;
			sprite.FrameCoords = new Vector2I(piece, player);
			sprite.Position = new Vector2(i * Constants.tileSize + size, size);
			sprite.ZIndex = 10;
			sprite.Centered = false;

			i++;
			AddChild(sprite);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var mouse = GetLocalMousePosition();
		mouse.X -= size;
		mouse.Y -= size;

		mouse.X = Mathf.Floor(mouse.X / Constants.tileSize);
		mouse.Y = Mathf.Floor(mouse.Y / Constants.tileSize);

		if (mouse.X < 0 || mouse.X >= 4 || mouse.Y != 0)
		{
			mouse = invalidPosition;
		}

		mousePosition = mouse;

		if (mouse != lastMousePosition)
		{
			QueueRedraw();
		}

		lastMousePosition = mouse;
	}

	public override void _Draw()
	{
		if (mousePosition != invalidPosition)
		{
			var vec1 = new Vector2(size + Constants.tileSize * mousePosition.X, size);
			var vec2 = new Vector2(Constants.tileSize, Constants.tileSize);
			
			DrawRect(new Rect2(vec1, vec2), new Color(1, 0, 0));
		}
	}

	public override void _Input(InputEvent @event)
	{
		if(@event is InputEventMouseButton && mousePosition != invalidPosition)
		{
			EmitSignal("OnSelected", pieces[(int) mousePosition.X]);
		}
	}
}
