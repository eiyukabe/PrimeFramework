using Godot;
using System;

public class TitleScreen : GameScene
{
    public override void _Input(InputEvent ev)
    {
        if (Input.IsActionJustPressed(InputActions.UI_CANCEL))
        {
            Prime.Quit();
        }
    }


    #region Button Callbacks

    private void OnPlayButtonPressed()
    {
        Prime.ChangeGameState("res://Game/GameScenes/Play/Play.tscn");
    }

    private void OnQuitButtonPressed()
    {
        Prime.Quit();
    }

    #endregion
    
}
