using Godot;
using System;

public partial class Play : GameScene
{

    #region Input

        public override void _Input(InputEvent ev)
        {
            if (Input.IsActionJustPressed(InputActions.UI_CANCEL))
            {
                Prime.PushSubScene(GameScenes.PAUSE_MENU, false);
            }
        }

    #endregion
    
}
