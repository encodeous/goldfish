using Godot;

namespace chessium.scripts;

/// <summary>
/// Contains utility methods.
/// </summary>
public static class Utils
{
    /// <summary>
    /// Converts a tuple of ints to a Vector2 position.
    /// </summary>
    /// <param name="thing">The tuple of (int, int) to convert.</param>
    /// <returns>A position in the form of (x, y) coords as a Vector2.</returns>
    public static Vector2 ToVector(this (int, int) thing)
    {
        return new Vector2(thing.Item1, thing.Item2);
    }
}