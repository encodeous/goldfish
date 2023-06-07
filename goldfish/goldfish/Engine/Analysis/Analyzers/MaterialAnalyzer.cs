using goldfish.Core.Data;

namespace goldfish.Engine.Analysis.Analyzers;

public class MaterialAnalyzer : IGameAnalyzer
{
    public double Weighting => 100;
    public double GetScore(in ChessState state)
    {
        throw new NotImplementedException();
    }
}