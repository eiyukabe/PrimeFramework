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

    #region Game Scenes

        #region Getters

            /// <summary>
            /// Returns a GameScene from the scene stack. 'name' is the name of the root node in the .tscn file.
            /// Returns null if not found.
            /// </summary>
            public static GameScene GetScene(string name)
            {
                foreach(var scene in Stack)
                {
                    if (scene.Name == name)
                    {
                        return scene;
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
            public static bool StackIsEmpty
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

            /// <summary> Clear all scenes off the stack (if any) and push a new main scene. </summary>
            public static void SetScene(GameScene scene)
            {
                Prime.Unpause();
                if (!StackIsEmpty)
                {
                    ClearScenes();
                }

                VisitScene(scene, true);
            }

            /// <summary> Clear all scenes off the stack (if any) and push a new main scene. </summary>
            public static void SetScene(string filepath)
            {
                SetScene(GetSceneInstance<GameScene>(filepath));
            }

        #endregion

        #region Visit

            /// <summary> Push a scene onto the stack. </summary>
            public static void VisitScene(GameScene scene, bool isMainScene, bool hideSceneBelow = true)
            {
                if (scene == null) { return; }
                SuspendTopScene(hideSceneBelow);
                scene.IsMain = isMainScene;
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
                
                Tree.SetInputAsHandled();   // Clear input so it doesn't carry over from one scene to another.
                scene.OnVisit();
            }

            public static void VisitScene(string filepath, bool isMain, bool hideSceneBelow = true)
            {
                VisitScene(GetSceneInstance<GameScene>(filepath), isMain, hideSceneBelow);
            }

            /// <summary>
            /// Push a scene that's already in the scenetree onto the stack as a main scene.  
            /// This should only be required when launching the game with F6 for debugging.
            /// </summary>
            public static void VisitSceneForF6Launch(GameScene scene)
            {
                Stack.Add(scene);
            }

        #endregion

        #region Leave
        
            /// <summary> Pop the topmost scene off the stack. Noop if the stack is empty. </summary>
            public static void Leave()
            {
                if (StackIsEmpty) { return; }
                var scene = TopScene;
                Stack.RemoveAt(Stack.Count - 1);
                scene.OnLeave();
                QueueFreeScene(scene);
                ResumeTopScene();
            }

            /// <summary> Pop the topmost main scene off the stack and any subscenes above it. Noop if there's no main scene on the stack. </summary>
            public static void LeaveScene()
            {
                int j = TopMainSceneIndex;
                if (j == -1) { return; }    // No main scene to pop

                for (int i = Stack.Count - 1; i > j; i--)
                {
                    BaseRemoveTop();        // Remove all subscenes above the main scene
                }

                Leave();                    // Pop main scene
                ResumeTopScene();
            }

            /// <summary> Pop the topmost subscene off the stack. Noop if the top scene is a main scene. </summary>
            public static void LeaveSubScene()
            {
                if (!StackIsEmpty && !TopScene.IsMain) { Leave(); }
                ResumeTopScene();
            }

        #endregion

        #region Remove

            /// <summary> Remove the topmost scene on the stack. </summary>
            private static void BaseRemoveTop()
            {
                var scene = TopScene;
                Stack.RemoveAt(Stack.Count - 1);
                scene.OnClear();
                QueueFreeScene(scene);
            }
            
            /// <summary> Remove the topmost main scene on the stack and any subscenes above it. Noop if there's no main scene on the stack. </summary>
            private static void BaseRemoveMain()
            {
                int j = TopMainSceneIndex;
                if (j == -1) { return; }    // No main scene to remove

                for (int i = Stack.Count - 1; i >= j; i--)
                {
                    BaseRemoveTop();        // Remove all subscenes and then the main scene
                }
            }

            private static void QueueFreeScene(GameScene scene)
            {
                if (scene.AttachToViewport) { scene.GetParent().GetParent().QueueFree(); }
                else                        { scene.QueueFree(); }
            }

        #endregion

        #region Clear

            /// <summary> Clear all scenes on the stack. </summary>
            public static void ClearScenes()
            {
                while(TopScene != null)
                {
                    BaseRemoveTop();
                }
            }

            /// <summary> Clear all subscenes on the stack until reaching a main scene or the stack is empty. </summary>
            public static void ClearSubScenes()
            {
                while(!StackIsEmpty && !TopScene.IsMain)
                {
                    BaseRemoveTop();
                }
            }

        #endregion

        #region Reload

            /// <summary> Reload the topmost main scene on the stack. </summary>
            public static void ReloadScene()
            {
                var i = TopMainSceneIndex;
                if (i == -1) { return; }    // No main scene to reload.
                
                var mainFilepath = Stack[i].Filename;
                BaseRemoveMain();
                VisitScene(GetSceneInstance<GameScene>(mainFilepath));
            }

            /// <summary> Reload the topmost subscene on the stack. Noop if the topmost scene is a main scene. </summary>
            public static void ReloadSubScene()
            {
                if (!StackIsEmpty && !TopScene.IsMain)
                {
                    var filepath = TopScene.Filename;
                    BaseRemoveTop();
                    VisitSubScene(GetSceneInstance<GameScene>(filepath));
                }
            }

            /// <summary> Reload the entire scene stack. </summary>
            public static void ReloadSceneStack()
            {
                if (StackIsEmpty) { return; }

                var sceneFilepaths = new List<string>();
                var sceneTypes = new List<bool>();
                var showSceneBelow = new List<bool>();

                foreach (var scene in Stack)
                {
                    sceneFilepaths.Add(scene.Filename);
                    sceneTypes.Add(scene.IsMain);
                    showSceneBelow.Add(scene.IsShowingSceneBelow);
                }

                ClearScenes();

                for (int i = 0; i < sceneFilepaths.Count; i++)
                {
                    var scene = GetSceneInstance<GameScene>(sceneFilepaths[i]);
                    if (sceneTypes[i]) { VisitScene(scene, showSceneBelow[i]); }
                    else               { VisitSubScene(scene, showSceneBelow[i]); }
                }
            }

        #endregion

        #region Resume/Suspend

            /// <summary> Called on a game scene when the scene above it is popped. </summary>
            private static void ResumeTopScene()
            {
                var scene = TopScene;
                if (scene == null) { return; }
                if (scene.Active)  { return; }
                
                SetSceneVisibility(scene, true);
                scene.SetProcess(true);
                scene.SetProcessInput(true);
                scene.SetPhysicsProcess(true);
                scene.Active = true;
            }

            /// <summary> Called on the topmost game scene when another game scene is pushed on top of it. </summary>
            private static void SuspendTopScene(bool hide)
            {
                var scene = TopScene;
                if (scene == null) { return; }
                if (!scene.Active) { return; }

                if (hide)
                {
                    SetSceneVisibility(scene, false);
                }
                scene.SetProcess(false);
                scene.SetPhysicsProcess(false);
                scene.SetProcessInput(false);
                scene.Active = false;

                scene.OnSuspend();
            }

            private static void SetSceneVisibility(GameScene scene, bool isVisible)
            {
                if (scene.AttachToViewport)
                {
                    var parent = scene.GetParent();
                    if (parent is Control)
                    {
                        var control = (Control) parent;
                        control.Visible = isVisible;
                    }
                }
                else
                {
                    scene.Visible = isVisible;
                }
            }

        #endregion

        #region Stack Control
            
            /// <summary> Show or hide the entire scene stack. </summary>
            public static void SetStackVisibility(bool isVisible)
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
        /// Loads and returns a PackedScene. Returns null if a PackedScene cannot be found from the given filepath.
        /// See also GetSceneInstance() to get an instance of a PackedScene.
        ///</summary>
        public static PackedScene GetPackedScene(string filepath)
        {
            var scene = ResourceLoader.Load(filepath);
            if (scene == null) { return null; }
            
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
            if (scene == null) { return null; }

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

        /// <summary>
        /// Loads a PackedScene and then returns an instance of it. Returns null if a PackedScene cannot be found
        /// from the given filepath. See also Prime.GetPackedScene() to load a scene without instancing it yet.
        /// </summary>
        public static Node GetSceneInstance(string filepath)
        {
            var scene = ResourceLoader.Load(filepath);
            if (scene == null) { return null; }

            if (scene is PackedScene)
            {
                var packedScene = (PackedScene) scene;
                return packedScene.Instance();
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

}
