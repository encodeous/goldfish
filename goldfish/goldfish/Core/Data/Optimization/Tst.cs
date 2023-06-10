using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace goldfish.Core.Data.Optimization;

/// <summary>
/// Transposition table
/// </summary>
public static class Tst
{
    public const int TableSize = 1 << 20;
    public static TranspositionEntry[] Table = new TranspositionEntry[TableSize];
    internal static readonly ulong[,] ZobristCache = new ulong[8 * 4, 256];
    internal static readonly ulong WhiteToMove;
    internal static readonly ulong[] CastleState = new ulong[256];
    internal static readonly ulong[] EnPassant = new ulong[256];

    static Tst()
    {
        var rng = new Xoroshiro128(0xDEADBEEF);
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                for (int k = 0; k < 256; k++)
                {
                    ZobristCache[i * 4 + j, k] = rng.Next();
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

    public static ref TranspositionEntry Get(in ChessState state)
    {
        var hash = state.Additional.Hash;
        ref var entry = ref Unsafe.Add(ref MemoryMarshal.GetReference<TranspositionEntry>(Table), (int)(hash % TableSize));
        if (entry.Hash != hash)
        {
            entry.Clear();
            entry.Hash = hash;
        }
        return ref entry;
    }
}