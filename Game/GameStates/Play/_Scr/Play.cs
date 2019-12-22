using Godot;
using System;

public class Play : GameState
{
    public override void _Input(InputEvent ev)
    {
        if (Input.IsActionJustPressed(InputActions.CANCEL))
        {
            Prime.PushGameState("res://Game/GameStates/PauseMenu/PauseMenu.tscn");
        }
    }
}
