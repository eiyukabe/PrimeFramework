using Godot;
using System;

/// <summary>
/// Manages application level operations like changing scenes, changing game states, pausing, quitting, and loading resources.
///
/// See also Game/Prime
/// </summary>
public static partial class Prime
{
    private static GameStateStack GameStates = new GameStateStack();
    
    public static SceneTree Tree;   // Set by PrimeRoot
    public static Node PrimeRoot;   // Set by PrimeRoot


    #region Scene Management

    /// <summary>
    /// Changes to a new scene and returns that scene as an object. Similar to Godot's ChangeScene() but also handles game states. 
    /// Aborts and returns null if a PackedScene cannot be found from the given scenePath.
    ///
    /// Specifically: removes everything from the SceneTree up to the root and adds a new instance of the given scene. If the new
    /// scene is a GameState the GameStateStack will be cleared and the new state will be pushed onto the stack.
    /// </summary>
    public static object ChangeScene(string scenePath)
    {
        var packedScene = GetPackedScene(scenePath);
        if (packedScene == null)
        {
            return null;
        }
        
        /* Remove all of PrimeRoot's children */
        var rootChildren = PrimeRoot.GetChildren();
        for (int i = rootChildren.Count - 1; i >= 0 ; i--)
        {
            var child = (Node) rootChildren[i];
            child.QueueFree();
        }

        /* Set up new scene */
        var sceneInstance = packedScene.Instance();
        PrimeRoot.AddChild(sceneInstance);

        /* Handle GameStates */
        if (sceneInstance is GameState)
        {
            GameStates.Clear();
            GameStates.Push((GameState) sceneInstance);
        }

        return sceneInstance;
    }

    #endregion


    #region GameState Management

    /// <summary>
    /// Push a new GameState onto the stack. Aborts and returns null if a GameState cannot be found from the given scenePath.
    /// </summary>
    public static GameState PushGameState(string scenePath, Node stateParent = null)
    {
        var packedScene = GetPackedScene(scenePath);
        if (packedScene == null)
        {
            return null;
        }

        var sceneInstance = packedScene.Instance();
        if (!(sceneInstance is GameState))
        {
            return null;
        }
        
        var state = (GameState) sceneInstance;

        if (stateParent == null)
        {
            PrimeRoot.AddChild(state);
        }
        else
        {
            stateParent.AddChild(state);
        }
        
        GameStates.Push(state);

        return state;
    }

    /// <summary> Pop the current GameState from the GameStateStack. </summary>
    public static void PopGameState()
    {
        GameStates.Pop();
    }

    #endregion


    #region Resource Loading

    /// <summary>
    /// Loads and returns a PackedScene. You must then call myScene.Instance() to instance the scene. Alternatively you
    /// can use Prime.GetSceneInstance<>(). Returns null if a PackedScene cannot be found from the given scenePath.
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
    /// Loads and instances a scene and returns the instance. Returns null if a scene of the correct type cannot be found
    /// from the given path. See also Prime.GetPackedScene() to load a scene without instancing it yet.
    /// </summary>
    public static T GetSceneInstance<T>(string scenePath) where T : Node
    {
        return ResourceLoader.Load(scenePath) as T;
    }

    #endregion

    public static void Pause()  { Tree.Paused = true; }
    public static void Unpause() { Tree.Paused = false; }
    public static void SetPause(bool pause) { Tree.Paused = pause; }

}
