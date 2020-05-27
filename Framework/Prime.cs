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
        /// Loads and returns a resource. Returns null if a resource cannot be found from the given filepath.
        /// If GetResource<PackedScene>() is used the PackedScene will still need to be instanced by the caller.
        /// Use GetNew() to load and instance a PackedScene.
        ///</summary>
        public static T GetResource<T>(string filepath) where T : Resource
        {
            Resource resource = ResourceLoader.Load(filepath);
            if (resource != null && resource is T)
            {
                return (T) resource;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Loads a PackedScene and then returns a new instance of it. Returns null if an instance of the specified type cannot
        /// be created with the given filepath. Use GetResource<PackedScene>() to load a PackedScene without instancing it yet.
        /// </summary>
        public static T GetNewInstance<T>(string filepath) where T : Node
        {
            Resource resource = ResourceLoader.Load(filepath);

            if (resource != null && resource is PackedScene)
            {
                PackedScene packedScene = (PackedScene) resource;
                Node instance = packedScene.Instance();
                if (instance is T)
                {
                    return (T) instance;
                }
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

    #region Nodes

    public static T GetAncestorOfType<T>(Node node) where T : Node
    {
        Node ancestor = node.GetParent();
        while (ancestor != null)
        {
            if (ancestor is T)
            {
                return (T) ancestor;
            }
            ancestor = ancestor.GetParent();
        }
        return null;
    }

    #endregion
}
