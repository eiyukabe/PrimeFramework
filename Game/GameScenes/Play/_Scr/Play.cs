using Godot;
using System;

public class Play : GameScene
{
    #region Initialization

        public Play()
        {
            IsMain = true;
        }

    #endregion

    #region Input

        public override void _Input(InputEvent ev)
        {
            if (Input.IsActionJustPressed(InputActions.UI_CANCEL))
            {
                Prime.PushScene(GameScenes.PAUSE_MENU, hideSceneBelow: false);
            }
        }

    #endregion
    
}
