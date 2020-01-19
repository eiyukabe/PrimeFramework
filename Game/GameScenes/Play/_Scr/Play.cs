using Godot;
using System;

public class Play : GameScene
{

    #region Input

        public override void _Input(InputEvent ev)
        {
            if (Input.IsActionJustPressed(InputActions.UI_CANCEL))
            {
                Prime.PushSubScene(GameScenes.PAUSE_MENU, hideSceneBelow: false);
                Prime.PushSubScene(GameScenes.PAUSE_MENU, false);
            }
        }

    #endregion
    
}
