using goldfish.Core.Data;

namespace goldfish.Core.Game.Rules.Pieces;

public struct Bishop : IPieceLogic
{
    public IEnumerable<ChessMove> GetMoves(ChessState state, int r, int c)
    {
        for (int i = 1; i < 8; i++)
        {
            if (!PieceUtils.MovePiece(state, r, c, r - i, c - i, out var move1))
            {
                if(move1.HasValue) 
                    yield return move1.Value;
                break;
            }
            yield return move1.Value;
        }

        for (int i = 1; i < 8; i++)
        {
            if (!PieceUtils.MovePiece(state, r, c, r - i, c + i, out var move2))
            {
                if (move2.HasValue)
                    yield return move2.Value;
                break;
            }

            yield return move2.Value;
        }

        for (int i = 1; i < 8; i++)
        {
            if (!PieceUtils.MovePiece(state, r, c, r + i, c - i, out var move3))
            {
                if (move3.HasValue)
                    yield return move3.Value;
                break;
            }

            yield return move3.Value;
        }
        for (int i = 1; i < 8; i++)
        {
            if (!PieceUtils.MovePiece(state, r, c, r + i, c + i, out var move4))
            {
                if(move4.HasValue) 
                    yield return move4.Value;
                break;
            }
            yield return move4.Value;
        }
    }

    public void GetAttacks(ChessState state, int r, int c, List<(int, int)> attacks)
    {
        for (int i = 1; i < 8; i++)
        {
            if (!PieceUtils.IsEmptySquare(state, r - i, c - i))
            {
                if ((r - i, c - i).IsWithinBoard())
                    attacks.Add((r - i, c - i));
                break;
            }
            attacks.Add((r - i, c - i));
        }

        for (int i = 1; i < 8; i++)
        {
            if (!PieceUtils.IsEmptySquare(state, r - i, c + i))
            {
                if ((r - i, c + i).IsWithinBoard())
                    attacks.Add((r - i, c + i));
                break;
            }

            attacks.Add((r - i, c + i));
        }

        for (int i = 1; i < 8; i++)
        {
            if (!PieceUtils.IsEmptySquare(state, r + i, c - i))
            {
                if ((r + i, c - i).IsWithinBoard())
                    attacks.Add((r + i, c - i));
                break;
            }

            attacks.Add((r + i, c - i));
        }

        for (int i = 1; i < 8; i++)
        {
            if (!PieceUtils.IsEmptySquare(state, r + i, c + i))
            {
                if ((r + i, c + i).IsWithinBoard())
                    attacks.Add((r + i, c + i));
                break;
            }

            attacks.Add((r + i, c + i));
        }
    }
}