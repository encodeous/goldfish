using Godot;

namespace chessium.scripts;

/// <summary>
/// Represents the button used to start a new game.
/// </summary>
public partial class NewButton : Dialog
{
	/// <summary>
	/// The width and height of the button.
	/// </summary>
	public const int newWidth = 54 * 2, newHeight = 16 * 2;
	
	/// <summary>
	/// Whether or not the mouse is currently hovering over the button.
	/// </summary>
	private bool mouseIn, lastMouseIn;

	/// <summary>
	/// The text to display on the button.
	/// </summary>
	private Sprite2D text;

	/// <summary>
	/// Constructs a new NewButton.
	/// </summary>
	public NewButton() : base(newWidth, newHeight)
	{
		// empty
	}
	
	/// Called when the node enters the scene tree for the first time.
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

	/// <summary>
	/// Handles user input for the button.
	/// </summary>
	/// <param name="event">The input event.</param>
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton { Pressed: false, ButtonIndex: MouseButton.Left } && mouseIn)
		{
			GetNode<Board>("/root/Root/Board").NewGame();
		}
	}

	/// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var mouse = GetLocalMousePosition();
		mouse.X -= size;
		mouse.Y -= size;

		// are we hovering over the button?
		if (mouse.X is >= 0 and <= newWidth && mouse.Y is >= 0 and <= newHeight)
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

	/// <summary>
	/// Draws a red rectangle to signify the button being hovered over.
	/// </summary>
	public override void _Draw()
	{
		if (mouseIn)
		{
			// draw the red rectangle when hovered over
			const float offset = size * 0.5f;
			var vec1 = new Vector2(offset, offset);
			var vec2 = new Vector2(newWidth + size, newHeight + size);
			
			DrawRect(new Rect2(vec1, vec2), new Color(1, 0, 0));
		}
	}
}
