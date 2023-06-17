namespace goldfish.Core.Data;

[Flags]
public enum CastleType
{
    WhiteQs = 1, // castling queenside
    BlackQs = 4,
    WhiteKs = 2, // castling king side
    BlackKs = 8
}