using Godot;
using System.Collections.Generic;

public static partial class Game
{
    public const int MAX_SCORES = 10;
    private const string HIGHSCORE_PASS = "a1b2c3"; // TODO: Make this unique for your project.
    public static List<int> HighScores = new List<int>();

    private const string HIGH_SCORE_FILE_PATH = "scores";

    /// <summary> Loads user high scores from disk, defaulting any empty slots to low scores. <summary>
    public static void InitializeHighScores()
    {
        for (int i = 0; i < MAX_SCORES; i++)
        {
            HighScores.Add(100 * (MAX_SCORES - i));
        }

        LoadHighScores();
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
            SaveHighScores();
        }
    }

    #region Saving/Loading

        private static void LoadHighScores()
        {
            File f = new File();
            Error e = f.OpenEncryptedWithPass(HIGH_SCORE_FILE_PATH, Godot.File.ModeFlags.Read, HIGHSCORE_PASS);
            if (e == Error.Ok)
            {
                for (int i = 0; i < MAX_SCORES; i++)
                {
                    HighScores[i] = (int) f.Get32();
                }
            }
            f.Close();
        }

        private static void SaveHighScores()
        {
            File f = new File();
            Error e = f.OpenEncryptedWithPass(HIGH_SCORE_FILE_PATH, Godot.File.ModeFlags.Write, HIGHSCORE_PASS);
            if (e == Error.Ok)
            {
                for (int i = 0; i < MAX_SCORES; i++)
                {
                    f.Store32((uint) HighScores[i]);
                }
            }
            f.Close();
        }

    #endregion

}