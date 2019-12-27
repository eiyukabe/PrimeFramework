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

    private static SceneStack Stack = new SceneStack();


    #region Game Scene Management

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
        Stack.Push(scene, TreeRoot);
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
        Stack.Push(scene, TreeRoot);
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
    public static void PopTopScene()
    {
        Stack.Pop();
        Stack.TopScene?.Visit();
    }

    /// <summary>
    /// Pop the topmost main scene off the stack and any subscenes above it. Noop if there's no main scene on the stack.  
    /// Callbacks:
    /// - oldSub.OnRemove()
    /// - oldMain.OnPop()
    /// - oldMain.OnRemove()
    /// - newTopScene.OnRevisit()
    /// - newTopScene.OnVisit()
    /// </summary>
    public static void PopScene()
    {
        Stack.PopMain();
        Stack.TopScene?.Visit();
    }

    /// <summary>
    /// Pop the topmost subscene from the stack. Noop if the top scene is a main scene.  
    /// Callbacks:
    /// - sub.OnPop()
    /// - sub.OnRemove()
    /// - newTopScene.OnRevisit()
    /// - newTopScene.OnVisit()
    /// </summary>
    public static void PopSubScene()
    {
        if (!Stack.TopScene.IsMain) { Stack.Pop(); }
        Stack.TopScene?.Visit();
    }

    /// <summary>
    /// Clear all scenes on the stack.  
    /// Callbacks:
    /// - scenes.OnRemove()
    /// </summary>
    public static void ClearScenes()
    {
        while(Stack.TopScene != null)
        {
            Stack.RemoveTop();
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
        while(Stack.TopScene != null && !Stack.TopScene.IsMain)
        {
            Stack.RemoveTop();
        }
        Stack.TopScene?.Visit();
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
    public static void ChangeScene(GameScene scene)
    {
        Stack.PopMain();
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
    public static void ChangeScene(string filepath)
    {
        ChangeScene(GetSceneInstance<GameScene>(filepath));
    }

    /// <summary>
    /// Swap topmost subscene for a new one. (Shortcut for PopSubScene(); PushSubScene())  
    /// Callbacks:
    /// - oldSub.OnPop()
    /// - oldSub.OnRemove()
    /// - scene.OnFirstVisit()
    /// - scene.OnVisit()
    /// </summary>
    public static void ChangeSubScene(GameScene scene)
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
    public static void ChangeSubScene(string filepath)
    {
        ChangeSubScene(GetSceneInstance<GameScene>(filepath));
    }

    /// <summary> Print the name of all scenes on the stack for debugging. </summary>
    public static void PrintSceneStack()
    {
        Stack.PrintSceneStack();
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
