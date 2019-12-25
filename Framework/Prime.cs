using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// Manages higher level scene tree operations like changing game states, pausing, quitting, and loading resources.
/// See also Game/Prime
/// </summary>
public static partial class Prime
{
    public static SceneTree Tree;   // Set by TreeMonitor
    public static Node TreeRoot;    // Set by TreeMonitor

    private static SceneStack StateStack = new SceneStack();


    #region Game State Management

    /// <summary>
    /// Changes to a new scene and returns that scene as an object. Similar to Godot's ChangeScene() but also handles game states. 
    /// Aborts and returns null if a PackedScene cannot be found from the given scenePath.
    ///
    /// Specifically: removes everything from PrimeRoot in the scene tree and adds a new instance of the given scene. If the new
    /// scene is a GameState the GameStateStack will be cleared and the new state will be pushed onto the stack.
    /// </summary>
    public static object ChangeGameState(string scenePath)
    {
        var gameState = GetSceneInstance<GameScene>(scenePath);
        if (gameState == null)
        {
            return null;
        }

        /* Clear all previous states */
        StateStack.Clear();

        /* Add and push new scene */
        TreeRoot.AddChild(gameState);
        StateStack.Push(gameState);

        return gameState;
    }

    /// <summary>
    /// Push a new GameState onto the stack. Aborts and returns null if a GameState cannot be found from the given scenePath.
    /// </summary>
    public static GameScene PushGameState(string scenePath)
    {
        var gameState = GetSceneInstance<GameScene>(scenePath);
        if (gameState == null)
        {
            return null;
        }

        /* Add and push new scene */
        TreeRoot.AddChild(gameState);
        StateStack.Push(gameState);
        
        return gameState;
    }

    /// <summary> Pop the current GameState from the GameStateStack. </summary>
    public static void PopGameState()
    {
        StateStack.Pop();
    }

    #endregion


    #region Resource Loading

    /// <summary>
    /// Loads and returns a PackedScene. Returns null if a PackedScene cannot be found from the given scenePath.
    /// See also GetSceneInstance() to get an instance of a PackedScene.
    ///</summary>
    public static PackedScene GetPackedScene(string scenePath)
    {
        var scene = ResourceLoader.Load(scenePath);
        if (scene is PackedScene)
        {
            return (PackedScene) scene;
        }
        return null;
    }

    /// <summary>
    /// Loads a PackedScene and then returns an instance of it. Returns null if an instance of the specified type cannot be found
    /// from the given scenePath. See also Prime.GetPackedScene() to load a scene without instancing it yet.
    /// </summary>
    public static T GetSceneInstance<T>(string scenePath) where T : Node
    {
        var scene = ResourceLoader.Load(scenePath);
        if (scene is PackedScene)
        {
            var packedScene = (PackedScene) scene;
            return packedScene.Instance() as T;
        }
        return null;
    }

    #endregion

    public static void Pause() { Tree.Paused = true; }
    public static void Unpause() { Tree.Paused = false; }
    public static void SetPause(bool pause) { Tree.Paused = pause; }

}
