using Godot;
using System;

public class TitleScreen : GameScene
{
    #region Initialization
        
        public TitleScreen()
        {
            IsMain = true;
        }

    #endregion

    #region Input
    
        public override void _Input(InputEvent ev)
        {
            if (Input.IsActionJustPressed(InputActions.UI_CANCEL))
            {
                Prime.Quit();
            }
        }

    #endregion

    #region Button Callbacks

        private void OnPlayButtonPressed()
        {
            Prime.SetScene(GameScenes.PLAY);
        }

        private void OnOptionsButtonPressed()
        {
            Prime.PushScene(GameScenes.OPTIONS);
        }

        private void OnCreditsButtonPressed()
        {
            Prime.PushScene(GameScenes.CREDITS);
        }

        private void OnQuitButtonPressed()
        {
            Prime.Quit();
        }

    #endregion
    
}
