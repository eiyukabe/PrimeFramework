using Godot;
using System;

public class Play : GameScene
{

    #region Input

        public override void _Input(InputEvent ev)
        {
            if (Input.IsActionJustPressed(InputActions.UI_CANCEL))
            {
                Prime.VisitSubScene(GameScenes.PAUSE_MENU, hideSceneBelow: false);
            }
        }

    #endregion
    
}
