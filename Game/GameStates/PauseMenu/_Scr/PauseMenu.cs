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
        if (Input.IsActionJustPressed(InputActions.UI_CANCEL))
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
        Prime.ChangeGameState("res://Game/GameStates/TitleScreen/TitleScreen.tscn");
    }

    private void OnQuitButtonPressed()
    {
        Prime.Quit();
    }

    #endregion


    #region GameState Callbacks

    public override void OnActivated()
    {
        Prime.Pause();
    }

    public override void OnRemoved()
    {
        Prime.Unpause();
    }

    #endregion
}
