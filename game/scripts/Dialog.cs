using Godot;
using Godot.Collections;

namespace chessium.scripts;

public partial class Dialog : Node2D
{
	private Sprite2D topLeft = new (), topRight = new (), bottomLeft = new (), bottomRight = new ();
	private Sprite2D top = new (), bottom = new (), left = new (), right = new ();
	private Sprite2D center = new ();

	private Texture2D texture = GD.Load<Texture2D>("res://assets/ui-element.png");

	public static readonly int size = 16;
	private int width, height;

	public Dialog(int width, int height)
	{
		this.width = width;
		this.height = height;
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SetWidth(top, width);
		SetWidth(bottom, width);

		SetHeight(left, height);
		SetHeight(right, height);

		SetWidth(center, width);
		SetHeight(center, height);

		topLeft.Position = new Vector2(0, size);
		top.Position = new Vector2(size, 0);
		topRight.Position = new Vector2(size + width, 0);

		left.Position = new Vector2(0, size);
		center.Position = new Vector2(size, size);
		right.Position = new Vector2(size + width, size);

		bottomLeft.Position = new Vector2(0, size + height);
		bottom.Position = new Vector2(size, size + height);
		bottomRight.Position = new Vector2(size + width, size + height);

		ZIndex = 100;

		var i = 0;
		var sprites = new Array<Sprite2D>
		{
			topLeft, topRight, top, bottomLeft, bottomRight, bottom, left, right, center
		};

		foreach (var sprite in sprites)
		{
			sprite.Centered = false;
			sprite.Texture = texture;
			sprite.Vframes = 3;
			sprite.Hframes = 3;
			sprite.Frame = i;
			sprite.ZIndex = -10;
			
			AddChild(sprite);
			
			i++;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// to be implemented
	}

	// Called when the node is about to leave the SceneTree.
	public override void _ExitTree()
	{
		foreach (var child in GetChildren())
		{
			child.QueueFree();
		}
	}

	private float PixelsToScale(int pixels)
	{
		return (float) pixels / size;
	}

	private void SetWidth(Sprite2D sprite, int w)
	{
		sprite.Scale = sprite.Scale with { X = PixelsToScale(w) };
	}

	private void SetHeight(Sprite2D sprite, int h)
	{
		sprite.Scale = sprite.Scale with { Y = PixelsToScale(h)};
	}
}
