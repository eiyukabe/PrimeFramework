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
            Prime.PopTopScene();
        }
    }


    #region Button Callbacks

    private void OnResumeButtonPressed()
    {
        Prime.PopTopScene();
    }
    
    private void OnTitleScreenButtonPressed()
    {
        Prime.SwapScene(Scenes.TITLE_SCREEN);
    }

    private void OnQuitButtonPressed()
    {
        Prime.Quit();
    }

    #endregion


    #region Game Scene Callbacks

    public override void OnVisit()
    {
        Prime.Pause();
    }

    public override void OnPop()
    {
        // TODO: Play "go back" sound
    }

    public override void OnRemove()
    {
        Prime.Unpause();
    }

    #endregion
}
