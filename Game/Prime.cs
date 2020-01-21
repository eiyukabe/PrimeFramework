using Godot;
using System;

/// <summary> I don't know how to summarize this class. Thanks for reading. </summary>
public static partial class Prime
{
    /// <summary> TreeMonitor will call this when the X is pressed to close the game. </summary>
    public static void OnXClicked()
    {
        Quit();
    }

    /// <summary> Quit the game. </summary>
    public static void Quit()
    {
        Tree.Quit();
    }
}
