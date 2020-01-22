using Godot;
using System;

/// <summary> TreeMonitor is an autoloaded node that sends information about the scene tree to other classes. <summary>
public class TreeMonitor : Node
{
    public override void _EnterTree()
    {
        var tree = GetTree();
        Prime.Tree = tree;
        Scene.Tree = tree;
        Scene.TreeRoot = tree.GetRoot();
        tree.SetAutoAcceptQuit(false);  // We'll handle quitting manually in Game/Prime
    }

    /// <summary> This Godot callback will notify the Game/Prime class when the X is clicked to close the game. </summary>
    public override void _Notification(int what)
    {
        if (what == MainLoop.NotificationWmQuitRequest)
        {
            Prime.OnXClicked();
        }
    }
}
