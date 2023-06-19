using goldfish.Core.Game.FEN;
using goldfish.Engine.Searcher;

var searcher = new GoldFishSearcher(TimeSpan.FromSeconds(15), 6, (int)(Environment.ProcessorCount / 1.2));

void OptimalMove(string fen)
{
    var chessState = FenConvert.Parse(fen);
    var result = searcher.StartSearch(chessState);
    Console.WriteLine(result);
}

// Both of these positions are checkmate

OptimalMove("r1b2rk1/pp4pp/2p1pq2/2bpPp2/2P1n3/1P3PP1/PBQ1P1BP/RN3R1K b - - 0 1");
/**
 * OUTPUT:
 * SearchResult { EngineEval = -2002298.8151777354, BestMove = NewPos: (2, 6), OldPos: (3, 4), WasPromotion: False, Type: Knight, Depth
 = 6 }

 */
OptimalMove("8/8/p5r1/1p6/1P1R4/8/5K1p/7k w - - 0 1");
/**
 * OUTPUT:
 * SearchResult { EngineEval = -2025.7434917749852, BestMove = NewPos: (0, 3), OldPos: (3, 3), WasPromotion: False, Type: Rook, Depth =
 9 }
 */