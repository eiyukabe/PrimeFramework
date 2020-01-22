using Godot;
using System;

public partial class Play : GameScene
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
                Scene.Push(GameScenes.PAUSE_MENU, hideSceneBelow: false);
            }
        }

    #endregion
}
