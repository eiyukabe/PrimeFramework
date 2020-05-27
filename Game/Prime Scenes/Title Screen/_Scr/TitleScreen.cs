using Godot;
using System;

public class TitleScreen : PrimeScene
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
            Scene.Set(PrimeScenes.PLAY);
        }

        private void OnOptionsButtonPressed()
        {
            Scene.Push(PrimeScenes.OPTIONS);
        }

        private void OnCreditsButtonPressed()
        {
            Scene.Push(PrimeScenes.CREDITS);
        }

        private void OnQuitButtonPressed()
        {
            Prime.Quit();
        }

    #endregion
    
}
