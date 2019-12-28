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

    private static List<GameScene> Stack = new List<GameScene>();


    #region Game Scene Management - Private

    /// <summary> Get the topmost scene on the stack. Returns null if the stack is empty. </summary>
    private static GameScene TopScene
    {
        get
        {
            if (Stack.Count > 0) { return Stack[Stack.Count - 1]; }
            return null;
        }
    }

    /// <summary> Get the topmost main scene index. Returns -1 if there is no main scene on the stack. </summary>
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
    
    /// <summary> Push a scene onto the stack. </summary>
    private static void BasePush(GameScene scene)
    {
        TopScene?.Suspend();
        Stack.Add(scene);
        TreeRoot.AddChild(scene);
        scene.Visit(justPushed: true);
    }

    /// <summary> Pop the topmost scene off the stack. </summary>
    private static void BasePop()
    {
        var scene = TopScene;
        if (scene == null) { return; }
        Stack.RemoveAt(Stack.Count - 1);
        scene.OnPop();
        scene.OnRemove();
        scene.QueueFree();
    }

    /// <summary> Pop the topmost main scene off the stack and any subscenes above it. Noop if there's no main scene on the stack. </summary>
    private static void BasePopMain()
    {
        int j = TopMainSceneIndex;
        if (j == -1)
        {
            return;             // No main scene to pop
        }

        for (int i = Stack.Count - 1; i > j; i--)
        {
            BaseRemoveTop();    // Remove all subscenes above the main scene
        }
        
        BasePop();              // Pop main scene
    }

    /// <summary> Remove the topmost scene on the stack. </summary>
    private static void BaseRemoveTop()
    {
        var scene = TopScene;
        Stack.RemoveAt(Stack.Count - 1);
        scene.OnRemove();
        scene.QueueFree();
    }

    #endregion


    #region Game Scene Management - Public

    /// <summary>
    /// Clear all scenes off the stack (if any) and push a new main scene.  
    /// Callbacks:
    /// - oldScenes.OnRemoved()
    /// - scene.OnFirstVisit()
    /// - scene.OnVisit()
    /// </summary>
    public static void SetScene(GameScene scene)
    {
        ClearScenes();
        PushScene(scene);
    }

    /// <summary>
    /// Clear all scenes off the stack (if any) and push a new main scene.  
    /// Callbacks:
    /// - oldScenes.OnRemoved()
    /// - scene.OnFirstVisit()
    /// - scene.OnVisit()
    /// </summary>
    public static void SetScene(string filepath)
    {
        SetScene(GetSceneInstance<GameScene>(filepath));
    }

    /// <summary>
    /// Push a main scene onto the stack.  
    /// Callbacks:
    /// - oldScene.OnSuspend()
    /// - scene.OnFirstVisit()
    /// - scene.OnVisit()
    /// </summary>
    public static void PushScene(GameScene scene)
    {
        if (scene == null) { return; }
        scene.IsMain = true;
        BasePush(scene);
    }

    /// <summary>
    /// Push a main scene onto the stack.  
    /// Callbacks:
    /// - oldScene.OnSuspend()
    /// - scene.OnFirstVisit()
    /// - scene.OnVisit()
    /// </summary>
    public static void PushScene(string filepath)
    {
        PushScene(GetSceneInstance<GameScene>(filepath));
    }

    /// <summary>
    /// Push a subscene onto the stack.  
    /// Callbacks:
    /// - oldScene.OnSuspend()
    /// - scene.OnFirstVisit()
    /// - scene.OnVisit()
    /// </summary>
    public static void PushSubScene(GameScene scene)
    {
        if (scene == null) { return; }
        scene.IsMain = false;
        BasePush(scene);
    }

    /// <summary>
    /// Push a subscene onto the stack.  
    /// Callbacks:
    /// - oldScene.OnSuspend()
    /// - scene.OnFirstVisit()
    /// - scene.OnVisit()
    /// </summary>
    public static void PushSubScene(string filepath)
    {
        PushSubScene(GetSceneInstance<GameScene>(filepath));
    }

    /// <summary>
    /// Pop the topmost main scene or subscene off the stack.  
    /// Callbacks:
    /// - scene.OnPop()
    /// - scene.OnRemove()
    /// - newTopScene.OnRevisit()
    /// - newTopScene.OnVisit()
    /// </summary>
    public static void PopTop()
    {
        BasePop();
        TopScene?.Visit();
    }

    /// <summary>
    /// Pop the topmost main scene off the stack and any subscenes above it. Noop if there's no main scene on the stack.  
    /// Callbacks:
    /// - oldSubs.OnRemove()
    /// - oldMain.OnPop()
    /// - oldMain.OnRemove()
    /// - newTopScene.OnRevisit()
    /// - newTopScene.OnVisit()
    /// </summary>
    public static void PopScene()
    {
        BasePopMain();
        TopScene?.Visit();
    }

    /// <summary>
    /// Pop the topmost subscene off the stack. Noop if the top scene is a main scene.  
    /// Callbacks:
    /// - sub.OnPop()
    /// - sub.OnRemove()
    /// - newTopScene.OnRevisit()
    /// - newTopScene.OnVisit()
    /// </summary>
    public static void PopSubScene()
    {
        if (!TopScene.IsMain) { BasePop(); }
        TopScene?.Visit();
    }

    /// <summary>
    /// Clear all scenes on the stack.  
    /// Callbacks:
    /// - scenes.OnRemove()
    /// </summary>
    public static void ClearScenes()
    {
        while(TopScene != null)
        {
            BaseRemoveTop();
        }
    }

    /// <summary>
    /// Clear all subscenes on the stack until reaching a main scene or the stack is empty.  
    /// Callbacks:
    /// - subs.OnRemove()
    /// - newTopMain.OnRevisit()
    /// - newTopMain.OnVisit()
    /// </summary>
    public static void ClearSubScenes()
    {
        while(TopScene != null && !TopScene.IsMain)
        {
            BaseRemoveTop();
        }
        TopScene?.Visit();
    }

    /// <summary>
    /// Swap topmost main scene for a new one. (Shortcut for PopScene(); PushScene())  
    /// Callbacks:
    /// - oldSubs.OnRemove()
    /// - oldMain.OnPop()
    /// - oldMain.OnRemove()
    /// - scene.OnFirstVisit()
    /// - scene.OnVisit()
    /// </summary>
    public static void SwapScene(GameScene scene)
    {
        BasePopMain();
        PushScene(scene);
    }

    /// <summary>
    /// Swap topmost main scene for a new one. (Shortcut for PopScene(); PushScene())  
    /// Callbacks:
    /// - oldSubs.OnRemove()
    /// - oldMain.OnPop()
    /// - oldMain.OnRemove()
    /// - scene.OnFirstVisit()
    /// - scene.OnVisit()
    /// </summary>
    public static void SwapScene(string filepath)
    {
        SwapScene(GetSceneInstance<GameScene>(filepath));
    }

    /// <summary>
    /// Swap topmost subscene for a new one. (Shortcut for PopSubScene(); PushSubScene())  
    /// Callbacks:
    /// - oldSub.OnPop()
    /// - oldSub.OnRemove()
    /// - scene.OnFirstVisit()
    /// - scene.OnVisit()
    /// </summary>
    public static void SwapSubScene(GameScene scene)
    {
        PopSubScene();
        PushSubScene(scene);
    }

    /// <summary>
    /// Swap topmost subscene for a new one. (Shortcut for PopSubScene(); PushSubScene())  
    /// Callbacks:
    /// - oldSub.OnPop()
    /// - oldSub.OnRemove()
    /// - scene.OnFirstVisit()
    /// - scene.OnVisit()
    /// </summary>
    public static void SwapSubScene(string filepath)
    {
        SwapSubScene(GetSceneInstance<GameScene>(filepath));
    }

    /// <summary> Print the name of all scenes on the stack for debugging. </summary>
    public static void PrintSceneStack()
    {
        GD.Print("--- Scene Stack ---");
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
