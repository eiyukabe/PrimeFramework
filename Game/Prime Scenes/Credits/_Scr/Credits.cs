using Godot;
using System;

public class Credits : PrimeScene
{
    #region Input

        public override void _Input(InputEvent ev)
        {
            if (ev.IsActionPressed(InputActions.UI_CANCEL))
            {
                Scene.PopSub();
            }
        }

    #endregion

    #region Button Callbacks
        
        private void OnBackButtonPressed()
        {
            Scene.PopSub();
        }

    #endregion
}
