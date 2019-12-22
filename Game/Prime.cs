using Godot;
using System;

/// <summary>
/// Manages application level operations like changing scenes, changing game states, pausing, quitting, and loading resources.
///
/// See also Framework/Prime
/// </summary>
public static partial class Prime
{
    /// <summary> PrimeRoot will call this when the X is pressed to close the game. </summary>
    public static void OnXClicked()
    {
        Quit();
    }

    public static void Quit()
    {
        Tree.Quit();
    }
}
