using Godot;
using System;

public class Credits : GameScene
{
    public override void _Input(InputEvent ev)
    {
        if (ev.IsActionPressed(InputActions.UI_CANCEL))
        {
            Prime.PopTop();
        }
    }

    private void OnBackButtonPressed()
    {
        Prime.PopTop();
    }
}
