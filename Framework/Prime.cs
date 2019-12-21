using Godot;
using System;

/// <summary>
/// Manages application level operations like changing scenes, changing game states, quitting, and loading PackedScenes.
/// </summary>
public static class Prime
{
    private static GameStateStack GameStateStack = new GameStateStack();
    // private static SceneTree Tree;  // TODO


    #region Scene Management

    /// <summary>
    /// Changes to a new scene and returns that scene as an object. Similar to Godot's ChangeScene() but also manages game states. 
    /// Aborts and returns null if a PackedScene cannot be found from the given scenePath.
    ///
    /// Specifically: removes everything from the SceneTree up to the root and adds a new instance of the given scene. If the new
    /// scene is a GameState the GameStateStack will be cleared and the new state will be pushed onto the stack.
    /// </summary>
    public static object ChangeScene(string scenePath, SceneTree sceneTree)
    {
        var packedScene = GetPackedScene(scenePath);
        if (packedScene == null)
        {
            return null;
        }
        
        var root = sceneTree.GetRoot();
        
        /* Remove all scenes up to the root */
        var rootChildren = root.GetChildren();
        if (rootChildren.Count > 0)
        {
            var node = (Node) rootChildren[0];     // The root node can only have one child.
            node.QueueFree();
        }

        /* Set up new scene */
        var sceneInstance = packedScene.Instance();
        root.AddChild(sceneInstance);
        sceneTree.CurrentScene = sceneInstance;     // This is used by Godot's ReloadCurrentScene() function

        if (sceneInstance is GameState)
        {
            GameStateStack.Clear();
            GameStateStack.Push((GameState) sceneInstance);
        }

        return sceneInstance;
    }

    #endregion


    #region GameState Management

    /// <summary>
    /// Push a new GameState onto the stack. Aborts and returns null if a GameState cannot be found from the given scenePath.
    /// </summary>
    public static GameState PushGameState(string scenePath, Node stateParent)
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
        stateParent.AddChild(state);
        GameStateStack.Push(state);

        return state;
    }

    /// <summary> Pop the current GameState from the GameStateStack. </summary>
    public static void PopGameState()
    {
        GameStateStack.Pop();
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
}
