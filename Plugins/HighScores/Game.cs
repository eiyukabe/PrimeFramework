using System.Collections.Generic;

public static partial class Game
{
    public const int MAX_SCORES = 10;
    public static List<int> HighScores = new List<int>();

    /// <summary> Loads user high scores from disk, defaulting any empty slots to low scores. <summary>
    public static void InitializeHighScores()
    {
        for (int i = 0; i < MAX_SCORES; i++)
        {
            HighScores.Add(100 * (MAX_SCORES - i));
        }

        //TODO: Read from disk.
    }

    public static void InsertHighScore(int newScore)
    {
        bool ScoreInserted = false;
        for (int i = 0; i < MAX_SCORES; i++)
        {
            if (newScore > HighScores[i])
            {
                HighScores.Insert(i, newScore);
                ScoreInserted = true;
                break;
            }
        }

        if (ScoreInserted)
        {
            HighScores.Remove(HighScores.Count - 1);
        }
    }

}