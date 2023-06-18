using goldfish.Core.Data;

namespace goldfish.Engine.Analysis;

public interface IGameAnalyzer
{
    public double Weighting { get; }
    public double GetScore(in ChessState state);
}