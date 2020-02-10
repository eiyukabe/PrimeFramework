using Godot;
using System;
using System.Collections.Generic;

/// <summary> I don't know how to summarize this class. Thanks for reading. </summary>
public static partial class Prime
{
    public static SceneTree Tree;   // Set by TreeMonitor

    #region Graphics

        public static int ResolutionX = 1280;
        public static int ResolutionY = 720;

        public static void ToggleFullScreen()
        {
            if (OS.WindowFullscreen)
            {
                OS.WindowFullscreen = false;
                OS.WindowSize = new Vector2(ResolutionX, ResolutionY);
                OS.CenterWindow();
            }
            else
            {
                OS.WindowFullscreen = true;
            }
        }

    #endregion

    #region Pausing

        /// <summary>
        /// Pause the game.  
        /// Set 'PauseMode' to change if an object processes or not while the game is paused.
        /// </summary>
        public static void Pause() { Tree.Paused = true; }

        /// <summary>
        /// Unpause the game.  
        /// Set 'PauseMode' to change if an object processes or not while the game is paused.
        /// </summary>
        public static void Unpause() { Tree.Paused = false; }

        /// <summary>
        /// Set the game to paused or unpaused.  
        /// Set 'PauseMode' to change if an object processes or not while the game is paused.
        /// </summary>
        public static void SetPaused(bool pause) { Tree.Paused = pause; }

    #endregion

    #region Resource Loading

        /// <summary>
        /// Loads and returns a PackedScene. Returns null if a PackedScene cannot be found from the given filepath.
        /// See also GetSceneInstance() to get an instance of a PackedScene.
        ///</summary>
        public static PackedScene GetPackedScene(string filepath)
        {
            var scene = ResourceLoader.Load(filepath);
            if (scene == null) { return null; }
            
            if (scene is PackedScene)
            {
                return (PackedScene) scene;
            }
            return null;
        }

        /// <summary>
        /// Loads a PackedScene and then returns an instance of it. Returns null if an instance of the specified type cannot be found
        /// from the given filepath. See also Prime.GetPackedScene() to load a scene without instancing it yet.
        /// </summary>
        public static T GetSceneInstance<T>(string filepath) where T : Node
        {
            var scene = ResourceLoader.Load(filepath);
            if (scene == null) { return null; }

            if (scene is PackedScene)
            {
                var packedScene = (PackedScene) scene;
                var instance = packedScene.Instance();
                if (instance is T)
                {
                    return (T) instance;
                }
            }
            return null;
        }

        /// <summary>
        /// Loads a PackedScene and then returns an instance of it. Returns null if a PackedScene cannot be found
        /// from the given filepath. See also Prime.GetPackedScene() to load a scene without instancing it yet.
        /// </summary>
        public static Node GetSceneInstance(string filepath)
        {
            var scene = ResourceLoader.Load(filepath);
            if (scene == null) { return null; }

            if (scene is PackedScene)
            {
                var packedScene = (PackedScene) scene;
                return packedScene.Instance();
            }
            return null;
        }

    #endregion

    #region Random

        private static Random RandomNumberGenerator = new Random((int)DateTime.Now.Ticks);

        /// <summary> Returns a random int n where 0 <= n < max </summary>
        public static int GetRandomInt(int max)
        {
            return RandomNumberGenerator.Next(max);
        }

        /// <summary> Returns a random int n where min <= n < max </summary>
        public static int GetRandomInt(int min, int max)
        {
            return RandomNumberGenerator.Next(min, max);
        }

        /// <summary> Returns a random float n where 0.0f <= n < max </summary>
        public static float GetRandomFloat(float max)
        {
            return (float)RandomNumberGenerator.NextDouble() * max;
        }

        /// <summary> Returns a random float n where min <= n < max </summary>
        public static float GetRandomFloat(float min, float max)
        {
            return min + (float)RandomNumberGenerator.NextDouble() * (max - min);
        }

    #endregion
}
