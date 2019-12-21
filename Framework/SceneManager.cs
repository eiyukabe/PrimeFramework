using Godot;
using System;

public static class SceneManager
{
    /// <summary>
    /// Changes to a new scene and returns that scene as an object. Returns null if the scenePath is wrong.
    ///
    /// Specifically: removes everything from the SceneTree up to the root, clears the GameState stack, and adds a new
    /// instance of the given scene. If the new scene is a GameState it will be pushed onto the GameState stack.
    /// </summary>
    public static object ChangeScene(string scenePath, SceneTree sceneTree)
    {
        var scene = ResourceLoader.Load(scenePath);
        if (!(scene is PackedScene))
        {
            return null;
        }

        var root = sceneTree.GetRoot();
        
        /* Remove all scenes up to the root */
        var rootChildren = root.GetChildren();
        if (rootChildren.Count > 0)
        {
            var node = (Node) root.GetChildren()[0];     // The root node can only have one child.
            node.QueueFree();
        }

        GameStateManager.ClearStack();

        /* Set up new scene */
        var packedScene = (PackedScene) scene;
        var sceneInstance = packedScene.Instance();
        root.AddChild(sceneInstance);

        if (sceneInstance is GameState)
        {
            GameStateManager.PushState((GameState) sceneInstance, null);
        }

        return sceneInstance;
    }

    public static object ChangeGameState(string scenePath, Node parent)
    {
        var scene = ResourceLoader.Load(scenePath);
        if (!(scene is PackedScene))
        {
            return null;
        }

        var packedScene = (PackedScene) scene;
        var sceneInstance = packedScene.Instance();

        if (!(sceneInstance is GameState))
        {
            return null;
        }
        
        var state = (GameState) sceneInstance;
        GameStateManager.PushState(state, parent);
        return state;
    }
}
