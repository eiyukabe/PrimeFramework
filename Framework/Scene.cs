using Godot;
using System;
using System.Collections.Generic;

public static class Scene
{
    public static SceneTree Tree;   // Set by TreeMonitor
    public static Node TreeRoot;    // Set by TreeMonitor

    private static List<GameScene> Stack = new List<GameScene>();

    #region Getters

        /// <summary> Returns a scene from the stack that matches the given type. Returns null if not found. </summary>
        public static T GetScene<T>() where T : GameScene
        {
            foreach(var scene in Stack)
            {
                if (scene is T)
                {
                    return (T) scene;
                }
            }
            return null;
        }

        /// <summary>
        /// Returns a GameScene from the scene stack. Index 0 is the bottom of the stack; Prime.SceneCount - 1 is the top.
        /// Returns null if index is out of range.
        /// </summary>
        public static GameScene GetScene(int index)
        {
            if (index > Stack.Count - 1 || index < 0) { return null; }
            return Stack[index];
        }

        /// <summary> Returns true if the scene stack is empty. </summary>
        public static bool IsStackEmpty
        {
            get { return Stack.Count == 0; }
        }

        /// <summary> Returns the number of scenes on the stack. </summary>
        public static int SceneCount
        {
            get { return Stack.Count; }
        }

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
                if (Stack.Count > 0)
                {
                    for (int i = Stack.Count - 1; i >= 0; i--)
                    {
                        if (Stack[i].IsMain) { return i; }
                    }
                }
                return -1;
            }
        }

    #endregion

    #region Set

        /// <summary> Clear all scenes off the stack (if any) and push a new scene. </summary>
        public static void Set(GameScene scene)
        {
            Prime.Unpause();
            Tree.SetInputAsHandled();

            if (!IsStackEmpty)
            {
                ClearAll();
            }

            Push(scene, true);
        }

        /// <summary> Clear all scenes off the stack (if any) and push a new scene. </summary>
        public static void Set(string filepath)
        {
            Set(Prime.GetSceneInstance<GameScene>(filepath));
        }

    #endregion

    #region Push

        /// <summary> Push a new scene onto the stack. </summary>
        public static void Push(GameScene scene, bool hideSceneBelow = true)
        {
            if (scene == null) { return; }
            scene.IsHidingSceneBelow = hideSceneBelow;
            SuspendTopScene(hideSceneBelow);
            Stack.Add(scene);
            
            if (scene.AttachToViewport)
            {
                var canvasLayer = new CanvasLayer();
                canvasLayer.Name = $"{scene.Name} (CanvasLayer)";

                var control = new Control();
                control.Name = $"{scene.Name} (Control)";

                control.AddChild(scene);
                canvasLayer.AddChild(control);
                TreeRoot.AddChild(canvasLayer);
            }
            else
            {
                TreeRoot.AddChild(scene);
            }
            
            Tree.SetInputAsHandled();
            scene.OnPushed();
        }

        /// <summary> Push a new scene onto the stack. </summary>
        public static void Push(string filepath, bool hideSceneBelow = true)
        {
            Push(Prime.GetSceneInstance<GameScene>(filepath), hideSceneBelow);
        }

        /// <summary> Push a scene that's already in the scenetree onto the stack. This should only be required when launching the game with F6 for debugging. </summary>
        public static void PushForF6Launch(GameScene scene)
        {
            Stack.Add(scene);
        }

    #endregion

    #region Pop
    
        /// <summary> Pop the topmost scene off the stack whether it's a main scene or subscene. Noop if the stack is empty. </summary>
        public static void Pop()
        {
            if (IsStackEmpty) { return; }
            var scene = TopScene;
            Stack.RemoveAt(Stack.Count - 1);
            scene.OnPopped();
            QueueFreeScene(scene);
            Tree.SetInputAsHandled();
            ResumeTopScene();
        }

        /// <summary> Pop the topmost main scene off the stack and any subscenes above it. Noop if there's no main scene on the stack. </summary>
        public static void PopMain()
        {
            int j = TopMainSceneIndex;
            if (j == -1) { return; }    // No main scene to pop

            for (int i = Stack.Count - 1; i > j; i--)
            {
                RemoveTopmost();        // Remove all subscenes above the main scene
            }

            Pop();                  // Pop main scene
            ResumeTopScene();
        }

        /// <summary> Pop the topmost subscene off the stack. Noop if the top scene is a main scene. </summary>
        public static void PopSub()
        {
            if (!IsStackEmpty && !TopScene.IsMain) { Pop(); }
            ResumeTopScene();
        }

    #endregion

    #region Clear

        /// <summary> Clear all scenes on the stack. </summary>
        public static void ClearAll()
        {
            while(TopScene != null)
            {
                RemoveTopmost();
            }
        }

        /// <summary> Clear all subscenes on the stack until reaching a main scene or the stack is empty. </summary>
        public static void ClearSubs()
        {
            while(!IsStackEmpty && !TopScene.IsMain)
            {
                RemoveTopmost();
            }
        }

        /// <summary> Remove the topmost scene on the stack. </summary>
        private static void RemoveTopmost()
        {
            var scene = TopScene;
            Stack.RemoveAt(Stack.Count - 1);
            scene.OnCleared();
            QueueFreeScene(scene);
        }
        
        /// <summary> Remove the topmost main scene on the stack and any subscenes above it. Noop if there's no main scene on the stack. </summary>
        private static void RemoveTopmostMain()
        {
            int j = TopMainSceneIndex;
            if (j == -1) { return; }    // No main scene to remove.

            for (int i = Stack.Count - 1; i >= j; i--)
            {
                RemoveTopmost();   // Remove all subscenes and then the main scene.
            }
        }

        private static void QueueFreeScene(GameScene scene)
        {
            if (scene.AttachToViewport) { scene.GetParent().GetParent().QueueFree(); }
            else                        { scene.QueueFree(); }
        }

    #endregion

    #region Reload

        /// <summary> Reload the topmost scene on the stack. </summary>
        public static void Reload()
        {
            if (!IsStackEmpty)
            {
                var filepath = TopScene.Filename;
                var hideSceneBelow = TopScene.IsHidingSceneBelow;
                RemoveTopmost();
                Push(Prime.GetSceneInstance<GameScene>(filepath), hideSceneBelow);
            }
        }


        /// <summary> Reload the topmost main scene on the stack. </summary>
        public static void ReloadMain()
        {
            var i = TopMainSceneIndex;
            if (i == -1) { return; }    // No main scene to reload.
            
            var filepath = Stack[i].Filename;
            var hideSceneBelow = Stack[i].IsHidingSceneBelow;
            RemoveTopmostMain();
            Push(Prime.GetSceneInstance<GameScene>(filepath), hideSceneBelow);
        }

        /// <summary> Reload the topmost subscene on the stack. Noop if the topmost scene is a main scene. </summary>
        public static void ReloadSub()
        {
            if (!IsStackEmpty && !TopScene.IsMain)
            {
                var filepath = TopScene.Filename;
                var hideSceneBelow = TopScene.IsHidingSceneBelow;
                RemoveTopmost();
                Push(Prime.GetSceneInstance<GameScene>(filepath), hideSceneBelow);
            }
        }

        /// <summary> Reload the entire scene stack. </summary>
        public static void ReloadAll()
        {
            if (IsStackEmpty) { return; }

            var sceneFilepaths = new List<string>();
            var hideSceneBelow = new List<bool>();

            foreach (var scene in Stack)
            {
                sceneFilepaths.Add(scene.Filename);
                hideSceneBelow.Add(scene.IsHidingSceneBelow);
            }

            ClearAll();

            for (int i = 0; i < sceneFilepaths.Count; i++)
            {
                var scene = Prime.GetSceneInstance<GameScene>(sceneFilepaths[i]);
                Push(scene, hideSceneBelow[i]);
            }
        }

    #endregion

    #region Resume/Suspend

        /// <summary> Called on a game scene when the scene above it is popped. </summary>
        private static void ResumeTopScene()
        {
            var scene = TopScene;
            if (scene == null) { return; }
            
            SetSceneVisibility(scene, true);
            scene.SetProcess(true);
            scene.SetProcessInput(true);
            scene.SetPhysicsProcess(true);
        }

        /// <summary> Called on the topmost game scene when another game scene is pushed on top of it. </summary>
        private static void SuspendTopScene(bool hide)
        {
            var scene = TopScene;
            if (scene == null) { return; }

            if (hide)
            {
                SetSceneVisibility(scene, false);
            }
            scene.SetProcess(false);
            scene.SetPhysicsProcess(false);
            scene.SetProcessInput(false);

            scene.OnSuspended();
        }

        private static void SetSceneVisibility(GameScene scene, bool isVisible)
        {
            if (scene.AttachToViewport)
            {
                var parent = (CanvasItem) scene.GetParent();
                parent.Visible = isVisible;
            }
            scene.Visible = isVisible;
        }

    #endregion

    #region Stack Control
        
        /// <summary> Show or hide the entire scene stack. </summary>
        public static void SetAllVisibility(bool isVisible)
        {
            foreach(var scene in Stack)
            {
                scene.SetVisible(isVisible);
            }
        }
        
    #endregion

    #region Debug
    
        /// <summary> Print the name of all scenes on the stack for debugging. </summary>
        public static void PrintSceneStack()
        {
            GD.Print("--- Scene Stack ---");
            foreach (var scene in Stack)
            {
                GD.Print(scene.DebugPrintName);
            }
        }

    #endregion
}
