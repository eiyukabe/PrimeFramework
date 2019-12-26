using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// Manages higher level scene tree operations like changing game scenes, pausing, quitting, and loading resources.
/// See also Game/Prime
/// </summary>
public static partial class Prime
{
    public static SceneTree Tree;   // Set by TreeMonitor
    public static Node TreeRoot;    // Set by TreeMonitor


    #region Game Scene Management - Private

    private static List<GameScene> Stack = new List<GameScene>();

    /// <summary> Get the scene at the top of the stack; returns null if the stack is empty. </summary>
    private static GameScene TopScene
    {
        get
        {
            if (Stack.Count > 0) { return Stack[Stack.Count - 1]; }
            return null;
        }
    }

    /// <summary> Get the index of the top most main scene in the stack. Returns -1 if there is no main scene in the stack. </summary>
    private static int TopMainSceneIndex
    {
        get
        {
            for (int i = Stack.Count - 1; i >= 0; i--)
            {
                if (Stack[i].IsMain) { return i; }
            }
            return -1;
        }
    }

    /// <summary> The basics for pushing a scene onto the stack. </summary>
    private static void _Push(GameScene scene)
    {
        TopScene?.Deactivate();
        Stack.Add(scene);
        TreeRoot.AddChild(scene);
        scene.Activate();
    }

    /// <summary> The basics for popping a scene off the stack. </summary>
    public static void _Pop()
    {
        var scene = TopScene;
        if (scene == null) { return; }
        Stack.RemoveAt(Stack.Count - 1);
        scene.OnPopped();
        scene.OnRemoved();
        scene.QueueFree();
    }

    /// <summary> The basics for popping the top most main scene on the stack. </summary>
    private static void _PopScene()
    {
        int j = TopMainSceneIndex;
        if (j == -1)
        {
            return;         // No main scene to pop
        }

        for (int i = Stack.Count - 1; i > j; i--)
        {
            _RemoveTop();   // Remove all subscenes above the main scene
        }
        
        _Pop();          // Pop main scene
    }

    /// <summary> The basics for removing the top most main scene on the stack. </summary>
    private static void _RemoveTop()
    {
        var scene = TopScene;
        Stack.RemoveAt(Stack.Count - 1);
        scene.OnRemoved();
        scene.QueueFree();
    }

    #endregion


    #region Game Scene Management - Public

    /// <summary> Push a main scene onto the stack. OnActivated() will be called on the new scene. </summary>
    public static void PushScene(GameScene scene)
    {
        if (scene == null) { return; }
        scene.IsMain = true;
        _Push(scene);
    }

    /// <summary> Push a main scene onto the stack. OnActivated() will be called on the new scene. </summary>
    public static void PushScene(string filepath)
    {
        PushScene(GetSceneInstance<GameScene>(filepath));
    }

    /// <summary> Push a subscene onto the stack. OnActivated() will be called on the new scene. </summary>
    public static void PushSubScene(GameScene scene)
    {
        if (scene == null) { return; }
        scene.IsMain = false;
        _Push(scene);
    }

    /// <summary> Push a subscene onto the stack. OnActivated() will be called on the new scene. </summary>
    public static void PushSubScene(string filepath)
    {
        PushSubScene(GetSceneInstance<GameScene>(filepath));
    }

    /// <summary> Pop the top most scene on the stack. OnPopped() and OnRemoved() will be called. OnActivated() will be called on the new top most scene. </summary>
    public static void PopTop()
    {
        _Pop();
        TopScene?.Activate();
    }

    /// <summary>
    /// Remove all subscenes until reaching a main scene then pop the main scene. Noop if there's no main scene in the stack.
    /// OnRemoved() will be called on the subscenes but not OnPopped(); OnPopped and OnRemoved() will be called on the main scene.
    /// </summary>
    public static void PopScene()
    {
        _PopScene();
        TopScene?.Activate();
    }

    /// <summary>
    /// Pop the top most subscene from the stack; it doesn't matter if there's a main scene or not. Noop if the top scene is a main scene.
    /// OnPopped() and OnRemoved() will be called on the subscene.
    /// </summary>
    public static void PopSubScene()
    {
        if (TopScene == null) { return; }
        if (!TopScene.IsMain) { PopTop(); }
        TopScene?.Activate();
    }

    /// <summary> Clear all scenes from the stack. OnRemoved() will be called on all scenes but not OnPopped(). </summary>
    public static void ClearScenes()
    {
        while(TopScene != null)
        {
            _RemoveTop();
        }
    }

    /// <summary>
    /// Clear all subscenes in the stack until reaching a main scene or the stack is empty. OnRemoved() will be called on the
    /// subscenes but not OnPopped(). OnActivated() will be called on the main scene if there is one.
    /// </summary>
    public static void ClearSubScenes()
    {
        while(TopScene != null && !TopScene.IsMain)
        {
            _RemoveTop();
        }
        TopScene?.Activate();
    }

    /// <summary>
    /// Change main scenes.  
    /// Remove all subscenes until reaching a main scene, then pop that main scene, then push a new main scene. OnRemoved() will be
    /// called on the subscenes but not OnPopped(); OnPopped() and OnRemoved() will be called on the old main scene and OnActivated()
    /// on the new main scene.
    /// </summary>
    public static void ChangeScene(GameScene scene)
    {
        _PopScene();
        PushScene(scene);
    }

    /// <summary> Change main scenes. </summary>
    public static void ChangeScene(string filepath)
    {
        ChangeScene(GetSceneInstance<GameScene>(filepath));
    }

    /// <summary>
    /// Change subscenes.  
    /// Pop the top most subscene and push a new one on the stack. OnPopped() and OnRemoved() is called on the old subscene and OnActivated() on the new one.
    /// </summary>
    public static void ChangeSubScene(GameScene scene)
    {
        PopSubScene();
        PushSubScene(scene);
    }

    /// <summary> Change subscenes. </summary>
    public static void ChangeSubScene(string filepath)
    {
        ChangeSubScene(GetSceneInstance<GameScene>(filepath));
    }

    /// <summary> Print the names of all the scenes on the stack for debugging. </summary>
    public static void PrintSceneStack()
    {
        GD.Print("-------");
        foreach (var scene in Stack)
        {
            GD.Print(scene.SceneName);
        }
    }

    #endregion


    #region Resource Loading

    /// <summary>
    /// Loads and returns a PackedScene. Returns null if a PackedScene cannot be found from the given filepath.
    /// See also GetSceneInstance() to get an instance of a PackedScene.
    ///</summary>
    public static PackedScene GetPackedScene(string filepath)
    {
        var scene = ResourceLoader.Load(filepath);
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
    public static void SetPause(bool pause) { Tree.Paused = pause; }

    #endregion

}
