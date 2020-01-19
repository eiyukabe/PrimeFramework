
public partial class Play : GameScene
{
    #region Score

        private bool ScoreCommitted = false;
        private int Score = 0;

        public void GainScore(int score)
        {
            Score += score;
        }

        /// <summary> Call this when you want to commit the player's current score for high score tracking. Note that this can be
        /// successfully done only once per play session. You will have to initialize a new Play scene to commit a new high score.
        /// </summary>
        public void CommitScore()
        {
            if (!ScoreCommitted)
            {
                Game.InsertHighScore(Score);
                ScoreCommitted = true;
            }
        }

    #endregion

}