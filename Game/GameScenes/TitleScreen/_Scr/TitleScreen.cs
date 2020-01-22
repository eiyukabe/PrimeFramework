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
            Scene.Set(GameScenes.PLAY);
        }

        private void OnOptionsButtonPressed()
        {
            Scene.Push(GameScenes.OPTIONS);
        }

        private void OnCreditsButtonPressed()
        {
            Scene.Push(GameScenes.CREDITS);
        }

        private void OnQuitButtonPressed()
        {
            Prime.Quit();
        }

    #endregion
    
}
