using Godot;

namespace chessium.scripts;

public partial class Move : GodotObject
{
    public readonly Piece piece, affectedPiece;
    public readonly Constants.MoveType moveType;
    public Vector2 destination;
    
    public Move(Piece piece, Constants.MoveType moveType, Vector2 destination, Piece affectedPiece = null)
    {
        this.piece = piece;
        this.moveType = moveType;
        this.destination = destination;
        this.affectedPiece = affectedPiece;
    }
}