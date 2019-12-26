using Godot;
using System;

public class PauseMenu : GameScene
{
    public override void _EnterTree()
    {
        PauseMode = PauseModeEnum.Process;
    }

    public override void _Input(InputEvent ev)
    {
        if (Input.IsActionJustPressed(InputActions.UI_CANCEL))
        {
            Prime.PopTop();
        }
    }


    #region Button Callbacks

    private void OnResumeButtonPressed()
    {
        Prime.PopTop();
    }
    
    private void OnTitleScreenButtonPressed()
    {
        Prime.ChangeScene(Scenes.TITLE_SCREEN);
    }

    private void OnQuitButtonPressed()
    {
        Prime.Quit();
    }

    #endregion


    #region Game Scene Callbacks

    public override void OnActivated()
    {
        Prime.Pause();
    }

    public override void OnPopped()
    {
        // TODO: Play "go back" sound
    }

    public override void OnRemoved()
    {
        Prime.Unpause();
    }

    #endregion
}
