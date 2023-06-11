namespace goldfish.Core.Data.Optimization;

/// <summary>
/// Transposition table
/// </summary>
public static class Tst
{
    public const int TableSize = 1 << 23;
    public static TranspositionEntry[] Table = new TranspositionEntry[TableSize];
    private static readonly ulong[,,] ZobristCache = new ulong[8, 4, 256];
    private static readonly ulong WhiteToMove;
    private static readonly ulong[] CastleState = new ulong[256];
    private static readonly ulong[] EnPassant = new ulong[256];

    static Tst()
    {
        var rng = new Xoroshiro128(0xDEADBEEF);
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                for (int k = 0; k < 256; k++)
                {
                    ZobristCache[i, j, k] = rng.Next();
                }
            }
        }

        for (int i = 0; i < 256; i++)
        {
            CastleState[i] = rng.Next();
            EnPassant[i] = rng.Next();
        }
        
        WhiteToMove = rng.Next();

        for (int i = 0; i < TableSize; i++)
        {
            Table[i] = new TranspositionEntry();
        }
    }

    /// <summary>
    /// Calculates the Zobrist hash for the current chess state
    /// </summary>
    /// <param name="state"></param>
    /// <returns></returns>
    public static unsafe ulong HashState(this in ChessState state)
    {
        ulong hash = 0;
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                hash ^= ZobristCache[i, j, state.Pieces[i * 4 + j]];
            }
        }

        if (state.ToMove == Side.White) hash ^= WhiteToMove;
        hash ^= EnPassant[state.Additional.EnPassant];
        hash ^= CastleState[state.Additional.CastleState];
        return hash;
    }

    public static ref TranspositionEntry Get(in ChessState state)
    {
        var hash = state.HashState();
        ref var entry = ref Table[hash % TableSize];
        if (entry.Hash != hash)
        {
            entry.Clear();
            entry.Hash = hash;
        }
        return ref entry;
    }
}