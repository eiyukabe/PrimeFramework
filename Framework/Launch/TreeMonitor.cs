using Godot;
using System;

/// <summary>
/// This node should be autoloaded. It's purpose is to communicate with the Prime class about the scene tree.
/// <summary>
public class TreeMonitor : Node
{
    public override void _EnterTree()
    {
        Prime.Tree = GetTree();
    }

    /// <summary> This Godot callback will notify the Game/Prime class when the application is sent a quit request. </summary>
    public override void _Notification(int what)
    {
        if (what == MainLoop.NotificationWmQuitRequest)
        {
            // Prime.Quit();
        }
    }

}
