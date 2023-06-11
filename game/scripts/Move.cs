using Godot;

namespace chessium.scripts;

/// Represents a move that a piece can make.
public partial class Move : GodotObject
{
    /// The original piece, and the piece that was affected by the original piece, if any.
    public readonly Piece piece, affectedPiece;
    /// The type of move that was performed.
    public readonly Constants.MoveType moveType;
    /// The destination of the piece after moving.
    public Vector2 destination;
    
    public Move(Piece piece, Constants.MoveType moveType, Vector2 destination, Piece affectedPiece = null)
    {
        this.piece = piece;
        this.moveType = moveType;
        this.destination = destination;
        this.affectedPiece = affectedPiece;
    }
}