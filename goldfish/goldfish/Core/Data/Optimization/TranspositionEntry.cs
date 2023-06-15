namespace goldfish.Core.Data.Optimization;

public struct TranspositionEntry
{
    public ulong Hash;
    public Side? Checked;
    public Grid8x8? WhiteCache;
    public Grid8x8? BlackCache;
    public double EngineEval;
    public double StaticEval;
    public double QuiesceEval;
    public int EvalDepth;
    public int QuiesceDepth;
    public bool PV;

    
    
    public TranspositionEntry()
    {
        Hash = 0;
        Checked = null;
        WhiteCache = null;
        BlackCache = null;
        StaticEval = double.NaN;
        QuiesceEval = double.NaN;
        EngineEval = double.NaN;
        EvalDepth = 0;
        QuiesceDepth = 10000;
        PV = false;
    }
    public void Clear()
    {
        Checked = null;
        EngineEval = double.NaN;
        StaticEval = double.NaN;
        QuiesceEval = double.NaN;
        WhiteCache = null;
        BlackCache = null;
        EvalDepth = 0;
        QuiesceDepth = 10000;
        PV = false;
    }
}