using Godot;
using System;

public class Play : GameScene
{
    public override void _Input(InputEvent ev)
    {
        if (Input.IsActionJustPressed(InputActions.UI_CANCEL))
        {
            Prime.PushSubScene(GameScenes.PAUSE_MENU);
        }
    }
}
