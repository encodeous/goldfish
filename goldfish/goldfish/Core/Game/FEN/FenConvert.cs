using System.Text;
using System.Text.RegularExpressions;
using goldfish.Core.Data;

namespace goldfish.Core.Game.FEN;

public static class FenConvert
{
    public static ChessState Parse(string fenString)
    {
        var mtch = Regex.Match(fenString, @"(?<PiecePlacement>((?<RankItem>[pnbrqkPNBRQK1-8]{1,8})\/?){8})\s+(?<SideToMove>b|w)\s+(?<Castling>-|[KQkq]+)\s+(?<EnPassant>-|[a-h][3-6])\s+(?<HalfMoveClock>\d+)\s+(?<FullMoveNumber>\d+)\s*");
        if (!mtch.Success)
        {
            throw new ArgumentException("Invalid FEN record.", nameof(fenString));
        }

        var state = ChessState.EmptyState();
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

    public static string ToFen(ChessState state)
    {
        var sb = new StringBuilder();
        for (var i = 7; i >= 0; i--)
        {
            var cnt = 0;
            for (var j = 0; j < 8; j++)
            {
                var p = state.GetPiece(i, j);
                if (p.GetPieceType() == PieceType.Space)
                {
                    cnt++;
                }
                else
                {
                    if (cnt != 0)
                    {
                        sb.Append(cnt);
                        cnt = 0;
                    }

                    sb.Append(ConvertPiece(p));
                }
            }
            if (cnt != 0)
            {
                sb.Append(cnt);
            }

            if(i != 0) sb.Append('/');
        }
        
        sb.Append(' ');

        sb.Append(state.ToMove == Side.White ? 'w' : 'b');

        sb.Append(' ');
        if (state.Additional.CastleState == 0b1111)
        {
            sb.Append('-');
        }
        else
        {
            if (!state.Additional.CheckCastle(CastleType.WhiteKs))
                sb.Append('K');
            if (!state.Additional.CheckCastle(CastleType.WhiteQs))
                sb.Append('Q');
            if (!state.Additional.CheckCastle(CastleType.BlackKs))
                sb.Append('k');
            if (!state.Additional.CheckCastle(CastleType.BlackQs))
                sb.Append('q');
        }

        sb.Append(' ');
        var epCol = state.Additional.EnPassant >> 2;
        var cSide = (Side)(state.Additional.EnPassant >> 1 & 1);
        if (state.Additional.CheckEnPassant(epCol, cSide))
        {
            var rCol = (char)(epCol + 'a');
            var rRow = cSide == Side.Black ? 6 : 3;
            sb.Append(rCol);
            sb.Append(rRow);
        }
        else
        {
            sb.Append('-');
        }

        sb.Append(" 0 0");
        return sb.ToString();
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
    
    private static char ConvertPiece(byte c)
    {
        var side = c.GetSide();

        var t = c.GetPieceType();
        var type = t switch
        {
            PieceType.Pawn => 'p',
            PieceType.Knight => 'n',
            PieceType.Bishop => 'b',
            PieceType.Rook => 'r',
            PieceType.Queen => 'q',
            PieceType.King => 'k',
            _ => throw new ArgumentOutOfRangeException(nameof(c), c, null)
        };
        return side == Side.Black ? type : char.ToUpper(type);
    }
}