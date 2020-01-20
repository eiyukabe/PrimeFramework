using Godot;
using System;

public class Play : GameScene
{

    #region Input

        public override void _Input(InputEvent ev)
        {
            if (Input.IsActionJustPressed(InputActions.UI_CANCEL))
            {
                Prime.VisitScene(GameScenes.PAUSE_MENU, isMain: true, hideSceneBelow: false);
            }
        }

    #endregion
    
}
