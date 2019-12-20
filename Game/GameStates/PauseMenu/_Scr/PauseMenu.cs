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
            GameStateManager.PopState();
        }
    }


    #region Button Callbacks

    private void OnResumeButtonPressed()
    {
        GameStateManager.PopState();
    }
    
    private void OnTitleScreenButtonPressed()
    {
        SceneManager.ChangeScene("res://Game/GameStates/TitleScreen/TitleScreen.tscn", GetTree());
    }

    private void OnQuitButtonPressed()
    {
        GetTree().Quit();
    }

    #endregion


    #region GameState Callbacks

    public override void OnActivated()
    {
        GetTree().Paused = true;
    }

    public override void OnRemoved()
    {
        GetTree().Paused = false;
    }

    #endregion
}
