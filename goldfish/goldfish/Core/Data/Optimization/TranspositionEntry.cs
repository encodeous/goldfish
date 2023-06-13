namespace goldfish.Core.Data.Optimization;

public struct TranspositionEntry
{
    public ulong Hash;
    public Side? Checked;
    public Grid8x8? WhiteCache;
    public Grid8x8? BlackCache;
    public double EngineEval;
    public double StaticEval;
    public int EvalDepth;
    public ulong Positions;

    public TranspositionEntry()
    {
        Hash = 0;
        Checked = null;
        WhiteCache = null;
        BlackCache = null;
        StaticEval = double.NaN;
        EngineEval = double.NaN;
    }
    public void Clear()
    {
        Checked = null;
        EngineEval = double.NaN;
        StaticEval = double.NaN;
        WhiteCache = null;
        BlackCache = null;
        Positions = 0;
    }
}