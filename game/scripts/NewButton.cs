using Godot;

namespace chessium.scripts;

public partial class NewButton : Dialog
{
	public static readonly int newWidth = 54 * 2, newHeight = 16 * 2;
	private bool mouseIn, lastMouseIn;

	private Sprite2D text;

	public NewButton() : base(newWidth, newHeight)
	{
		// to be implemented
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready(); // remove if not working
		text = new Sprite2D();

		text.Texture = GD.Load<Texture2D>("res://assets/player.png");
		text.Vframes = 4;
		text.FrameCoords = text.FrameCoords with { Y = 3 };
		text.Centered = false;
		text.Position = new Vector2(size, size);
		
		AddChild(text);
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton && !@event.IsPressed() && ((InputEventMouseButton) @event).ButtonIndex == MouseButton.Left && mouseIn)
		{
			GetNode<Board>("/root/Root/Board").NewGame();
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var mouse = GetLocalMousePosition();
		mouse.X -= size;
		mouse.Y -= size;

		if (mouse.X >= 0 && mouse.X <= newWidth && mouse.Y >= 0 && mouse.Y <= newHeight)
		{
			mouseIn = true;
		}
		else
		{
			mouseIn = false;
		}

		if (mouseIn != lastMouseIn)
		{
			QueueRedraw();
		}

		lastMouseIn = mouseIn;
	}

	public override void _Draw()
	{
		if (mouseIn)
		{
			var offset = size * 0.5f;
			var vec1 = new Vector2(offset, offset);
			var vec2 = new Vector2(newWidth + size, newHeight + size);
			
			DrawRect(new Rect2(vec1, vec2), new Color(1, 0, 0));
		}
	}
}
