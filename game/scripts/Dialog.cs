using Godot;
using Godot.Collections;

namespace chessium.scripts;

/// <summary>
/// Represents a dialog that shows up on the screen.
/// </summary>
public partial class Dialog : Node2D
{
	/// <summary>
	/// Represents the sprites of the corners and filled part of the dialog.
	/// </summary>
	private Sprite2D topLeft = new (), topRight = new (), bottomLeft = new (), bottomRight = new ();
	private Sprite2D top = new (), bottom = new (), left = new (), right = new ();
	private Sprite2D center = new ();

	/// <summary>
	/// A texture that contains all elements needed for a dialog.
	/// </summary>
	private Texture2D texture = GD.Load<Texture2D>("res://assets/ui-element.png");

	/// <summary>
	/// The size of a dialog part in pixels.
	/// </summary>
	public const int size = 16;

	/// <summary>
	/// The width and height of the dialog.
	/// </summary>
	private int width, height;

	/// <summary>
	/// Constructs a new Dialog with a specified width and height.
	/// </summary>
	/// <param name="width">The width of the dialog.</param>
	/// <param name="height">The height of the dialog.</param>
	public Dialog(int width, int height)
	{
		this.width = width;
		this.height = height;
	}
	
	/// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// set the sizes of the component sprites that make up the middle parts of the dialog
		SetWidth(top, width);
		SetWidth(bottom, width);

		SetHeight(left, height);
		SetHeight(right, height);

		SetWidth(center, width);
		SetHeight(center, height);

		// set the position of all component sprites
		topLeft.Position = new Vector2(0, 0);
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

	/// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// to be implemented
	}

	/// Called when the node is about to leave the SceneTree.
	public override void _ExitTree()
	{
		foreach (var child in GetChildren())
		{
			child.QueueFree();
		}
	}

	/// <summary>
	/// Converts a pixel value to a scale relative to the size of the dialog.
	/// </summary>
	/// <param name="pixels">An amount of pixels.</param>
	/// <returns>A decimal representing the relative scale of the pixel amount.</returns>
	private float PixelsToScale(int pixels)
	{
		return (float) pixels / size;
	}

	/// <summary>
	/// Sets the width of a sprite component.
	/// </summary>
	/// <param name="sprite">The sprite to set the width of.</param>
	/// <param name="w">The width to set to.</param>
	private void SetWidth(Sprite2D sprite, int w)
	{
		sprite.Scale = sprite.Scale with { X = PixelsToScale(w) };
	}

	/// <summary>
	/// Sets the height of a sprite component.
	/// </summary>
	/// <param name="sprite">The sprite to set the height of.</param>
	/// <param name="h">The height to set to.</param>
	private void SetHeight(Sprite2D sprite, int h)
	{
		sprite.Scale = sprite.Scale with { Y = PixelsToScale(h)};
	}
}
