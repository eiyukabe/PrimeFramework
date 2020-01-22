using Godot;
using System;

/// <summary>
/// GlobalGameLaunch is for initialization code that will run every time the game is launched, even when using f6.
/// See also NormalGameLaunch.
/// Note: GlobalGameLaunch runs before NormalGameLaunch because GlobalGameLaunch is autoloaded.
/// </summary>
public class GlobalGameLaunch : Node
{
    public GlobalGameLaunch()
    {
        /* Game initialization */
        PauseMode = PauseModeEnum.Process;
        Bind.BindCommonDebugKeys();
        Bind.BindUIWASDKeys();
    }

    public override void _Input(InputEvent ev)
    {
        if (ev.IsActionPressed(InputActions.PRINT_SCENE_STACK))
        {
            Scene.PrintSceneStack();
        }

        if (ev.IsActionPressed(InputActions.RELOAD_SCENE))
        {
            Scene.Reload();
        }
    }
}
