using Godot;
using System;

public class Credits : GameScene
{
    public override void _Input(InputEvent ev)
    {
        if (ev.IsActionPressed(InputActions.UI_CANCEL))
        {
            Prime.Leave();
        }
    }

    private void OnBackButtonPressed()
    {
        Prime.Leave();
    }
}
