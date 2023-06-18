# GoldFish - A cute little chess engine.

> In the ocean of AI, there's a sprightly creature, GoldFish - the humble chess engine. This modest protege of the legendary StockFish navigates the chessboard with earnest determination. As its pixelated fins flap, it conjures clever moves, attempting to echo the strategic depth of its colossal predecessor. It may not rival StockFish's commanding tide, but it triumphs in its tenacity and charm. Watching GoldFish is like witnessing a small boat bravely challenging a storm; its spirit undeterred, and its ambition unfathomable. It may be just a tiny spark in the vast AI sea, but its light is warm, bright, and uniquely Goldfish.
> ~ GPT 4

## Basic Info

GoldFish provides a simple API to query valid chess moves according to (mostly) FIDE rules, and to search / evaluate optimal positions.

- Language: C#/.NET
- Elo: 1200-1500 ish?
- Search Depth: 6-ply+ (can reach deeper depths on simple boards)
- Algorithm: Minimax + Alpha Beta Pruning & Transposition Table

# Features

- Loading/Saving FEN (Forsyth-Edwards Notation)
- Getting the valid moves for a piece
- Checking for Checkmates & Draws
- Searching for optimal moves
- Calculating static/static evaluations for a game state
- Printing & visualizing the game board

## API Usage

View the API Docs here:

Loading/Saving FEN