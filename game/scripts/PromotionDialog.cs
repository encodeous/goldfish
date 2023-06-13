using Godot;
using Godot.Collections;

namespace chessium.scripts;

/// <summary>
/// Represents the dialog that shows up when a pawn is promoting.
/// </summary>
public partial class PromotionDialog : Dialog
{
	/// <summary>
	/// The event that fires when a pawn is promoting.
	/// </summary>
	[Signal] public delegate void OnSelectedEventHandler(Constants.Pieces type);

	/// <summary>
	/// The width and height of this dialog.
	/// </summary>
	public const float promotionWidth = Constants.tileSize * 4.0f;
	public const float promotionHeight = Constants.tileSize;

	/// <summary>
	/// All possible choices for promotion.
	/// </summary>
	private Array<Constants.Pieces> pieces = new () { Constants.Pieces.ROOK, Constants.Pieces.KNIGHT, Constants.Pieces.BISHOP, Constants.Pieces.QUEEN };

	/// <summary>
	/// The position of the user's mouse.
	/// </summary>
	private readonly Vector2 invalidPosition = new (-1, -1);
	private Vector2 mousePosition, lastMousePosition;

	/// <summary>
	/// The player who owns the promoting pawn.
	/// </summary>
	private Constants.Player player;

	/// <summary>
	/// Constructs a new PromotionDialog.
	/// </summary>
	/// <param name="player">The player who owns the promoting pawn.</param>
	public PromotionDialog(Constants.Player player) : base((int) promotionWidth, (int) promotionHeight)
	{
		this.player = player;
	}
	
	/// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready(); // remove if not working
		
		mousePosition = invalidPosition;
		lastMousePosition = invalidPosition;

		// adds the possible choices to the dialog
		var i = 0;
		foreach (var piece in pieces)
		{
			var sprite = new Sprite2D();
			sprite.Texture = GD.Load<Texture2D>("res://assets/pieces.png");
			sprite.Hframes = 6;
			sprite.Vframes = 2;
			sprite.FrameCoords = new Vector2I((int) piece, (int) player);
			sprite.Position = new Vector2(i * Constants.tileSize + size, size);
			sprite.ZIndex = 10;
			sprite.Centered = false;

			i++;
			AddChild(sprite);
		}
	}

	/// Called every frame. 'delta' is the elapsed time since the previous frame.
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

		// record the user's mouse position
		mousePosition = mouse;

		if (mouse != lastMousePosition)
		{
			QueueRedraw();
		}

		lastMousePosition = mouse;
	}

	/// <summary>
	/// Draws a border around the piece to be selected.
	/// </summary>
	public override void _Draw()
	{
		if (mousePosition != invalidPosition)
		{
			var vec1 = new Vector2(size + Constants.tileSize * mousePosition.X, size);
			var vec2 = new Vector2(Constants.tileSize, Constants.tileSize);
			
			DrawRect(new Rect2(vec1, vec2), new Color(1, 0, 0));
		}
	}

	/// <summary>
	/// Handles user input.
	/// </summary>
	/// <param name="event">The input event.</param>
	public override void _Input(InputEvent @event)
	{
		if(@event is InputEventMouseButton && mousePosition != invalidPosition)
		{
			EmitSignal("OnSelected", (int) pieces[(int) mousePosition.X]); // this might not work since we are casting an enum value to an int
		}
	}
}
