using Godot;
using System;

public class TitleScreen : GameState
{
    public override void _Input(InputEvent ev)
    {
        if (Input.IsActionJustPressed(InputActions.CANCEL))
        {
            GetTree().Quit();
        }
    }


    #region Button Callbacks

    private void OnPlayButtonPressed()
    {
        Prime.ChangeScene("res://Game/GameStates/Play/Play.tscn");
    }

    private void OnQuitButtonPressed()
    {
        GetTree().Quit();
    }

    #endregion
}
