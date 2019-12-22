using Godot;
using System;

/// <summary>
/// PrimeRoot's purpose is to serve as a root container for the game in the scene tree; this allows us to change scenes
/// without clearing out certain nodes we always want to keep in the tree, such as autoloads -- see Prime.ChangeScene()
/// to see where this is done.
///
/// PrimeRoot also sends information about the SceneTree to the Prime class.
///
/// PrimeRoot should be autoloaded to work correctly.
/// <summary>
public class PrimeRoot : Node
{
    public override void _EnterTree()
    {
        var tree = GetTree();

        Prime.Tree = tree;
        Prime.PrimeRoot = this;
        tree.CurrentScene = this;       // This is used by Godot's ReloadCurrentScene()
        tree.SetAutoAcceptQuit(false);  // We'll handle this manually in this class
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
