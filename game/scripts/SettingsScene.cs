using Godot;

namespace chessium.scripts;

public partial class SettingsScene : Control
{
	private Node board, pieces;
	
	private readonly string pathToBoardTextures = "res://assets/textures/chessboard/";
	private readonly string pathToPieceTextures = "res://assets/textures/chesspieces/";
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		board = GetNode("/root/Game/Board");
		pieces = GetNode("/root/Game/Board/Pieces");

		GetNode("Popup/ConfirmationButton").Connect("button_up", Callable.From(OnConfirmButtonUp));
		GetNode("Popup/CancelButton").Connect("button_up", Callable.From(OnCancelButtonUp));

		var boardTextures = DirAccess.GetFilesAt(pathToBoardTextures);
		var idx = 0;

		foreach (var boardTexture in boardTextures)
		{
			var boardTextureName = boardTexture.Split("_")[1];
			GetNode<OptionButton>("Popup/BoardsOptionButton").AddItem(boardTextureName, idx);
			GetNode<OptionButton>("Popup/BoardsOptionButton").SetItemMetadata(idx, boardTexture);

			if (pathToBoardTextures + boardTexture == board.GetNode<Sprite2D>("Sprite").Texture.ResourcePath)
			{
				GetNode<OptionButton>("Popup/BoardsOptionButton").Select(idx);
			}

			idx++;
		}
		
		var pieceTextures = DirAccess.GetFilesAt(pathToPieceTextures);
		idx = 0;

		foreach (var pieceTexture in pieceTextures)
		{
			var pieceTextureName = pieceTexture.Split("_")[1];
			GetNode<OptionButton>("Popup/PiecesOptionButton").AddItem(pieceTextureName, idx);
			GetNode<OptionButton>("Popup/PiecesOptionButton").SetItemMetadata(idx, pieceTexture);

			if (pathToPieceTextures + pieceTexture == board.GetNode<Sprite2D>("Sprite").Texture.ResourcePath)
			{
				GetNode<OptionButton>("Popup/PiecesOptionButton").Select(idx);
			}

			idx++;
		}

		GetNode<CheckBox>("Popup/SoundCheckBox").ButtonPressed = Utils.soundEnabled;
	}

	private void OnConfirmButtonUp()
	{
		var boardTexture = GD.Load<Texture2D>(pathToBoardTextures + GetNode<OptionButton>("Popup/BoardsOptionButton").GetSelectedMetadata());
		var pieceTexture = GD.Load<Texture2D>(pathToPieceTextures + GetNode<OptionButton>("Popup/PiecesOptionButton").GetSelectedMetadata());

		board.GetNode<Sprite2D>("Sprite").Texture = boardTexture;

		foreach (var piece in pieces.GetChildren())
		{
			piece.GetNode<Sprite2D>("Sprite").Texture = pieceTexture;
		}
		
		Utils.soundEnabled = GetNode<CheckBox>("Popup/SoundCheckBox").ButtonPressed;
		
		QueueFree();
	}

	private void OnCancelButtonUp()
	{
		QueueFree();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// to be implemented
	}
}
