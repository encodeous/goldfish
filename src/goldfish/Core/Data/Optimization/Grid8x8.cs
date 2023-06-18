using System.Runtime.CompilerServices;

namespace goldfish.Core.Data.Optimization;

public struct Grid8x8
{
    public ulong Data;
    public bool this[int x, int y]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ((Data >> (x * 8 + y)) & 1) == 1;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            Data = (Data & ~(1ul << (x * 8 + y))) | ((ulong)(value ? 1 : 0) << (x * 8 + y));
        }
    }    
}