# GoldFish - A cute little chess engine.

> In the ocean of AI, there's a sprightly creature, GoldFish - the humble chess engine. This modest protege of the legendary StockFish navigates the chessboard with earnest determination. As its pixelated fins flap, it conjures clever moves, attempting to echo the strategic depth of its colossal predecessor. It may not rival StockFish's commanding tide, but it triumphs in its tenacity and charm. Watching GoldFish is like witnessing a small boat bravely challenging a storm; its spirit undeterred, and its ambition unfathomable. It may be just a tiny spark in the vast AI sea, but its light is warm, bright, and uniquely Goldfish.
> ~ GPT 4

## Basic Info

GoldFish provides a simple API to query valid chess moves according to (mostly) FIDE rules, and to search / evaluate optimal positions.

- Language: C#/.NET
- Elo: 1500-1600 ish?
- Search Depth: 6-ply+ (can reach deeper depths on simple boards)
- Algorithm: Minimax + Alpha Beta Pruning & Transposition Table

# Features

- Loading/Saving FEN (Forsyth-Edwards Notation)
- Calculating Valid moves for pieces
- Evaluating the board numerically
- Search for optimal moves, with the option of using multiple CPU cores
- Printing & visualizing the game board

## API Usage

View the API Docs [Here](https://encodeous.github.io/goldfish/)

## Examples

### Loading/Saving FEN

> This C# code segment converts a Forsyth-Edwards Notation (FEN) string to a chess game state and subsequently converts this state back to a FEN string, outputting the result to the console.

```csharp
using goldfish.Core.Game.FEN;

// loads the FEN string into a chess state
var chessState = FenConvert.Parse("2kr1b2/1pp1p3/p3N1pn/3r4/5p1p/1PBP3P/P1P1KPP1/1R1R4 b - - 1 23");

// saves the game state into FEN
Console.WriteLine(FenConvert.ToFen(chessState));

/**
 * OUTPUT:
 * 2kr1b2/1pp1p3/p3N1pn/3r4/5p1p/1PBP3P/P1P1KPP1/1R1R4 b - - 0 0
 * NOTE: Move counting is not yet supported
*/
```

### Calculate Optimal Moves

> This C# code segment calculates and prints the optimal chess move for a given game state defined by a Forsyth-Edwards Notation (FEN) string, using the GoldFishSearcher algorithm with a specified time limit and depth.

```csharp
using goldfish.Core.Game.FEN;
using goldfish.Engine.Searcher;

// 15 second soft limit with the minimal search depth of 6
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
```

### Iterating through the valid moves

> This C# code segment generates and prints the valid moves a chess piece can make from a specific position on a chess board, using a given Forsyth-Edwards Notation (FEN) string to set the initial game state.

```csharp
using goldfish.Core.Data;
using goldfish.Core.Game;
using goldfish.Core.Game.FEN;

var chessState = FenConvert.Parse("2kr1b2/1pp1p3/p3N1pn/3r4/5p1p/1PBP3P/P1P1KPP1/1R1R4 b - - 1 23");

// allocate an array for the moves to be added into, recommend at least 35 spaces
var moveArr = new ChessMove[35];
var len = chessState.GetValidMovesForSquare(7, 3, moveArr);

for(var i = 0; i < len; i++)
    Console.WriteLine(moveArr[i]);

/**
 * OUTPUT:
 * NewPos: (6, 3), OldPos: (7, 3), WasPromotion: False, Type: Rook
 * NewPos: (5, 3), OldPos: (7, 3), WasPromotion: False, Type: Rook
 * NewPos: (7, 4), OldPos: (7, 3), WasPromotion: False, Type: Rook
*/
```

### Analyzing the game and detecting checkmates

> This C# code segment analyzes the state of a chess game defined by a Forsyth-Edwards Notation (FEN) string, providing an evaluation of the game state and detecting if a checkmate condition is present, then outputs the result to the console.

```csharp
using goldfish.Core.Game.FEN;
using goldfish.Engine;
using goldfish.Engine.Analysis.Analyzers;

void Analyze(string fen)
{
    var chessState = FenConvert.Parse(fen);
    var (eval, distance) = GoldFishEngine.EngineEval(chessState, 6);
    if (Math.Abs(eval) >= WinAnalyzer.CheckmateWeighting)
        Console.WriteLine($"Eval: {eval} - Checkmate in {distance} half-move(s)");
    else
        Console.WriteLine($"Eval: {eval}");
}

Analyze("r1b2rk1/pp4pp/2p1pq2/2bpPp2/2P1n3/1P3PP1/PBQ1P1BP/RN3R1K b - - 0 1");
/**
 * OUTPUT:
 * Eval: -2002298.8151777354 - Checkmate in 5 half-move(s)
*/
Analyze("k1K1N3/p7/8/8/8/8/8/8 w - - 0 1");
/**
 * OUTPUT:
 * Eval: 2002056.5491268865 - Checkmate in 1 half-move(s)
*/
```