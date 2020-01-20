using Godot;
using System;

/// <summary>
/// GameScenes represent the different states a game can be in -- states such as being on the title screen, playing a level, pausing, being on the overworld map, etc.
/// GameScenes can be stacked on top of each other. They are managed by the Prime class; Prime will execute different callbacks depending on what it's doing with a GameScene.
/// GameScenes can be classified as a main scene or a subscene. The idea is to classify your important base GameScenes as main scenes and scenes that belong to those as subscenes.
/// GameScenes should not be nested in the scenetree; the nested scenes would not have their callbacks executed. Use a combination of main and subscenes instead of nesting.  
/// See Framework/Prime class to see where GameScenes are managed.
/// </summary>
public class GameScene : PrimeNode2D
{
    [Export] public bool AttachToViewport = true;   // Set if this game scene will be attached to the viewport.

    public bool Active = false;
    public bool IsMain = true;
    public bool IsShowingSceneBelow;        // This will be set by the Prime class depending on how this scene is pushed. It is used when reloading the entire scene stack.
    
    public virtual void OnVisit() {}        // Called the first time this scene is pushed onto the stack.
    public virtual void OnSuspend() {}      // Called right before another scene is pushed on top of this one.
    public virtual void OnLeave() {}        // Called right before this scene is popped from the stack. Will not be called when this scene is cleared from the stack.
    public virtual void OnClear() {}        // Called every time this scene is cleared from the stack.

    public override void _Ready()
    {
        /* If this scene is in the tree (_Ready() is called) but the scene stack is empty, that probably means the game was launched using f6.
        This is a special case that should only happen while debugging. We'll handle it by just pushing this scene as a main scene.
        If this is happening during a normal game launch it means no initial scenes are being pushed when the game starts. You can fix this
        by going to NormalGameLaunch.cs and calling Prime.SetScene() or Prime.PushScene() in _Ready() or _EnterTree(). */
        if (Prime.StackIsEmpty)
        {
            Prime.VisitSceneForF6Launch(this);
        }
    }

    /// <summary> Get the name of this scene for debug printing. </summary>
    public string DebugPrintName
    {
        get { return IsMain ? $"- (Main) {Name}" : $"   - (sub) {Name}"; }
    }
}
