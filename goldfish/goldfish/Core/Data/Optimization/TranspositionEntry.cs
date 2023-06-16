namespace goldfish.Core.Data.Optimization;

public struct TranspositionEntry
{
    public ulong Hash;
    public Side? Checked;
    public Grid8x8? WhiteCache;
    public Grid8x8? BlackCache;
    public double EngineEval;
    public double StaticEval;
    public int Moves;
    public int EvalDepth;
    public bool PV;

    
    
    public TranspositionEntry()
    {
        Hash = 0;
        Checked = null;
        WhiteCache = null;
        BlackCache = null;
        StaticEval = double.NaN;
        Moves = 10000;
        EngineEval = double.NaN;
        EvalDepth = 0;
        PV = false;
    }
    public void Clear()
    {
        Checked = null;
        EngineEval = double.NaN;
        StaticEval = double.NaN;
        Moves = 10000;
        WhiteCache = null;
        BlackCache = null;
        EvalDepth = 0;
        PV = false;
    }
}