using System.Text.RegularExpressions;
using goldfish.Core.Data;

namespace goldfish.Core.Game.FEN;

public static partial class FenParser
{
    public static ChessState Parse(string fenString)
    {
        var mtch = Regex.Match(fenString, @"(?<PiecePlacement>((?<RankItem>[pnbrqkPNBRQK1-8]{1,8})\/?){8})\s+(?<SideToMove>b|w)\s+(?<Castling>-|[KQkq]+)\s+(?<EnPassant>-|[a-h][3-6])\s+(?<HalfMoveClock>\d+)\s+(?<FullMoveNumber>\d+)\s*");
        if (!mtch.Success)
        {
            throw new ArgumentException("Invalid FEN record.", nameof(fenString));
        }

        var state = new ChessState();
        for (var i = 0; i < 32; i++)
        {
            unsafe
            {
                state.Pieces[i] = (byte)PieceType.Space | (byte)PieceType.Space << 4;
            }
        }
        // ranks
        for (var i = 0; i < 8; i++)
        {
            var rank = mtch.Groups["RankItem"].Captures[i].Value;
            var cur = 0;
            var curPos = 0;
            while (cur < rank.Length)
            {
                if (char.IsNumber(rank[cur]))
                {
                    var num = rank[cur] - '0';
                    for (var j = 0; j < num; j++)
                    {
                        state.SetPiece(8 - i - 1, j + curPos, PieceType.Space.ToPiece(Side.None));
                    }
                    curPos += num - 1;
                }
                else
                {
                    state.SetPiece(8 - i - 1, curPos, ParsePiece(rank[cur])); 
                }

                curPos++;
                cur++;
            }
        }

        state.ToMove = mtch.Groups["SideToMove"].Value == "w" ? Side.White : Side.Black;
        var castling = mtch.Groups["Castling"].Value;
        if(!castling.Contains('K'))
            state.Additional.MarkCastle(CastleType.WhiteKs);
        if(!castling.Contains('Q'))
            state.Additional.MarkCastle(CastleType.WhiteQs);
        if(!castling.Contains('k'))
            state.Additional.MarkCastle(CastleType.BlackKs);
        if(!castling.Contains('q'))
            state.Additional.MarkCastle(CastleType.BlackQs);
        var enPassant = mtch.Groups["EnPassant"].Value;
        if (enPassant != "-")
        {
            var col = enPassant[0] - 'a';
            var side = enPassant[1] - '0' <= 4 ? Side.White : Side.Black;
            state.Additional.MarkEnPassant(col, side);
        }

        return state;
    }

    private static byte ParsePiece(char c)
    {
        var side = Side.Black;
        if (char.IsUpper(c))
        {
            side = Side.White;
            c = char.ToLower(c);
        }

        var type = c switch
        {
            'p' => PieceType.Pawn,
            'n' => PieceType.Knight,
            'b' => PieceType.Bishop,
            'r' => PieceType.Rook,
            'q' => PieceType.Queen,
            'k' => PieceType.King,
            _ => throw new ArgumentOutOfRangeException(nameof(c), c, null)
        };
        return type.ToPiece(side);
    }
}