using Godot;

public class HighScoreMenu : PrimeScene
{
    #region Initialization

        public override void OnPushed()
        {
            for (int i = 0; i < Game.MAX_SCORES; i++)
            {
                Label ScoreLabel = GetNodeOrNull<Label>("CanvasLayer/Center2/Menu/Score" + (i+1));
                if (ScoreLabel != null)
                {
                    ScoreLabel.Text = ("" + Game.HighScores[i]);
                }
            }
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

        private void OnBackButtonPressed()
        {
            Scene.PopSub();
        }

    #endregion
}
