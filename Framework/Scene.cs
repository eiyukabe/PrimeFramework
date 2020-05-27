using Godot;
using System;
using System.Collections.Generic;

public static class Scene
{
    public static SceneTree Tree;   // Set by TreeMonitor
    public static Node TreeRoot;    // Set by TreeMonitor

    private static List<PrimeScene> Stack = new List<PrimeScene>();

    #region Getters

        /// <summary> Returns a scene from the stack that matches the given type. Returns null if not found. </summary>
        public static T GetScene<T>() where T : PrimeScene
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

        /// <summary> Returns the number of scenes on the stack. </summary>
        public static int SceneCount
        {
            get { return Stack.Count; }
        }

        /// <summary> Get the topmost scene on the stack. Check Stack.Count > 0 before calling this. </summary>
        private static PrimeScene TopScene
        {
            get { return Stack[Stack.Count - 1]; }
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
        public static void Set(PrimeScene scene)
        {
            Prime.Unpause();
            Tree.SetInputAsHandled();
            ClearAll();
            Push(scene, true);
        }

        /// <summary> Clear all scenes off the stack (if any) and push a new scene. </summary>
        public static void Set(string filepath)
        {
            Set(Prime.GetNewInstance<PrimeScene>(filepath));
        }

    #endregion

    #region Push

        /// <summary> Push a new scene onto the stack. </summary>
        public static void Push(PrimeScene scene, bool hideSceneBelow = true)
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
            scene.OnCurrent();
        }

        /// <summary> Push a new scene onto the stack. </summary>
        public static void Push(string filepath, bool hideSceneBelow = true)
        {
            Push(Prime.GetNewInstance<PrimeScene>(filepath), hideSceneBelow);
        }

        /// <summary> Push a scene that's already in the scenetree onto the stack. This should only be required when launching the game with F6 for debugging. </summary>
        public static void PushForF6Launch(PrimeScene scene)
        {
            Stack.Add(scene);
            scene.OnPushed();
            scene.OnCurrent();
        }

    #endregion

    #region Pop
    
        /// <summary> Pop the topmost scene off the stack whether it's a main scene or subscene. Noop if the stack is empty. </summary>
        public static void Pop()
        {
            if (Stack.Count > 0)
            {
                var scene = TopScene;
                Stack.RemoveAt(Stack.Count - 1);
                scene.OnPopped();
                scene.OnCleared();
                QueueFreeScene(scene);
                Tree.SetInputAsHandled();
                ResumeTopScene();
            }
        }

        /// <summary> Pop the topmost main scene off the stack and any subscenes above it. Noop if there's no main scene on the stack. </summary>
        public static void PopMain()
        {
            int j = TopMainSceneIndex;
            if (j == -1) { return; }    // No main scene to pop

            for (int i = Stack.Count - 1; i > j; i--)
            {
                Clear();         // Remove all subscenes above the main scene
            }

            Pop();                      // Pop main scene
            ResumeTopScene();
        }

        /// <summary> Pop the topmost subscene off the stack. Noop if there is not a subscene on the top of the stack. </summary>
        public static void PopSub()
        {
            if (Stack.Count > 0 && !TopScene.IsMain) { Pop(); }
            ResumeTopScene();
        }

    #endregion

    #region Clear
    
        /// <summary> Clear the topmost scene on the stack. </summary>
        private static void Clear()
        {
            var scene = TopScene;
            Stack.RemoveAt(Stack.Count - 1);
            scene.OnCleared();
            QueueFreeScene(scene);
        }

        /// <summary> Clear all scenes on the stack. </summary>
        public static void ClearAll()
        {
            while (Stack.Count > 0)
            {
                Clear();
            }
        }

        /// <summary> Clear the topmost main scene on the stack. </summary>
        public static void ClearMain()
        {
            int i = TopMainSceneIndex;
            if (i == -1) { return; }    // No main scene to remove.

            for (int j = Stack.Count - 1; j >= i; j--)
            {
                Clear();                // Remove all subscenes and then the main scene.
            }
            
            ResumeTopScene();
        }

        /// <summary> Clear all subscenes on the stack until reaching a main scene or the stack is empty. </summary>
        public static void ClearSubs()
        {
            while (Stack.Count > 0 && !TopScene.IsMain)
            {
                Clear();
            }
            ResumeTopScene();
        }

    #endregion

    #region Reload

        /// <summary> Reload the topmost scene on the stack. </summary>
        public static void Reload()
        {
            if (Stack.Count > 0)
            {
                var filepath = TopScene.Filename;
                var hideSceneBelow = TopScene.IsHidingSceneBelow;
                Clear();
                Push(Prime.GetNewInstance<PrimeScene>(filepath), hideSceneBelow);
            }
        }

        /// <summary> Reload the entire scene stack. </summary>
        public static void ReloadAll()
        {
            if (Stack.Count > 0)
            {
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
                    var scene = Prime.GetNewInstance<PrimeScene>(sceneFilepaths[i]);
                    Push(scene, hideSceneBelow[i]);
                }
            }
        }

        /// <summary> Reload the topmost main scene on the stack. </summary>
        public static void ReloadMain()
        {
            var i = TopMainSceneIndex;
            if (i == -1) { return; }    // No main scene to reload.
            
            var filepath = Stack[i].Filename;
            var hideSceneBelow = Stack[i].IsHidingSceneBelow;

            for (int j = Stack.Count - 1; j >= i; j--)
            {
                Clear();                // Remove all subscenes and then the main scene.
            }
            
            Push(Prime.GetNewInstance<PrimeScene>(filepath), hideSceneBelow);
        }

        /// <summary> Reload the topmost subscene on the stack. Noop if the topmost scene is a main scene. </summary>
        public static void ReloadSub()
        {
            if (Stack.Count > 0 && !TopScene.IsMain)
            {
                var filepath = TopScene.Filename;
                var hideSceneBelow = TopScene.IsHidingSceneBelow;
                Clear();
                Push(Prime.GetNewInstance<PrimeScene>(filepath), hideSceneBelow);
            }
        }

    #endregion

    #region Resume/Suspend

        /// <summary> Called on a PrimeScene when the scene above it is popped. </summary>
        private static void ResumeTopScene()
        {
            if (Stack.Count > 0)
            {
                var scene = TopScene;
                SetSceneVisibility(scene, true);
                scene.SetProcess(true);
                scene.SetProcessInput(true);
                scene.SetPhysicsProcess(true);
                scene.OnCurrent();
            }
        }

        /// <summary> Called on the topmost PrimeScene when another PrimeScene is pushed on top of it. </summary>
        private static void SuspendTopScene(bool hide)
        {
            if (Stack.Count > 0)
            {
                var scene = TopScene;
                if (hide)
                {
                    SetSceneVisibility(scene, false);
                }
                scene.SetProcess(false);
                scene.SetPhysicsProcess(false);
                scene.SetProcessInput(false);
                scene.OnSuspended();
            }
        }

        private static void SetSceneVisibility(PrimeScene scene, bool isVisible)
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
                scene.Visible = isVisible;
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
    
    private static void QueueFreeScene(PrimeScene scene)
    {
        if (scene.AttachToViewport) { scene.GetParent().GetParent().QueueFree(); }
        else                        { scene.QueueFree(); }
    }
}
