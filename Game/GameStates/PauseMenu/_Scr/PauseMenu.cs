using Godot;
using System;

public class PauseMenu : GameState
{
    public override void _EnterTree()
    {
        PauseMode = PauseModeEnum.Process;
    }

    public override void _Input(InputEvent ev)
    {
        if (Input.IsActionJustPressed(InputActions.CANCEL))
        {
            Prime.PopGameState();
        }
    }


    #region Button Callbacks

    private void OnResumeButtonPressed()
    {
        Prime.PopGameState();
    }
    
    private void OnTitleScreenButtonPressed()
    {
        Prime.ChangeScene("res://Game/GameStates/TitleScreen/TitleScreen.tscn");
    }

    private void OnQuitButtonPressed()
    {
        GetTree().Quit();
        // Prime.Quit();    // TODO
    }

    #endregion


    #region GameState Callbacks

    public override void OnActivated()
    {
        Prime.Pause();
    }

    public override void OnRemoved()
    {
        Prime.UnPause();
    }

    #endregion
}
