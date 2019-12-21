using Godot;
using System;

/// <summary>
/// GlobalGameLaunch is for initialization code that will run every time the game is launched, even when using f6.
/// See also NormalGameLaunch.
/// Note: GlobalGameLaunch runs before NormalGameLaunch because GlobalGameLaunch is autoloaded.
/// </summary>
public class GlobalGameLaunch : Node
{
    public override void _EnterTree()
    {
        /* Global init game */
        QueueFree();
    }
}
