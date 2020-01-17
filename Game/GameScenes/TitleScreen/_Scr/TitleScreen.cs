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
            Prime.SetScene(GameScenes.PLAY);
        }

        private void OnCreditsButtonPressed()
        {
            Prime.PushSubScene(GameScenes.CREDITS);
        }

        private void OnQuitButtonPressed()
        {
            Prime.Quit();
        }

    #endregion
    
}
