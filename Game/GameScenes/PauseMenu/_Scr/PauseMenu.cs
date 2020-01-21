using Godot;
using System;

public class PauseMenu : GameScene
{
    #region Initialization

        public override void _EnterTree()
        {
            PauseMode = PauseModeEnum.Process;
        }

    #endregion

    #region Input

        public override void _Input(InputEvent ev)
        {
            if (Input.IsActionJustPressed(InputActions.UI_CANCEL))
            {
                Scene.PopSub();
            }
        }

    #endregion

    #region Button Callbacks

        private void OnResumeButtonPressed()
        {
            Scene.PopSub();
        }

        private void OnOptionsButtonPressed()
        {
            Scene.Push(GameScenes.OPTIONS);
        }
        
        private void OnTitleScreenButtonPressed()
        {
            Scene.Set(GameScenes.TITLE_SCREEN);
        }

        private void OnQuitButtonPressed()
        {
            Prime.Quit();
        }

    #endregion

    #region Game Scene Callbacks

        public override void OnPushed()
        {
            Prime.Pause();
        }

        public override void OnPopped()
        {
            Prime.Unpause();
        }

        public override void OnCleared()
        {
            Prime.Unpause();
        }

    #endregion

}
