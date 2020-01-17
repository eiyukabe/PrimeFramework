using Godot;
using System;

/// <summary>
/// GameScenes represent the different states a game can be in -- states such as being on the title screen, playing a level, pausing, being on the overworld map, etc.  
/// GameScenes can be stacked on top of each other, swapped, reloaded, and removed. They are managed by the Prime class; Prime will execute different callbacks
/// depending on what it's doing with a GameScene.  
/// GameScenes can be classified as a main scene or a subscene. The idea is to classify your important base GameScenes as main scenes and scenes that belong to those as
/// subscenes.  
/// GameScenes should not be nested in the scenetree; the nested scenes would not have their callbacks executed. Use a combination of main and subscenes instead of nesting.  
/// Note the difference between OnPop() and OnRemove(): OnPop() is called when a scene is removed from the stack IF the scene under it is going to be visited. OnPop() is
/// for code that should only run when returning to a previous scene, like playing a sound when closing a menu. OnRemove() is called every time a scene is removed from the
/// stack. Any time OnPop() is called OnRemoved() is also called (but not the other way around).  
/// See Framework/Prime class to see where GameScenes are managed.
/// </summary>
public class GameScene : PrimeNode2D
{
    public bool Active = false;
    public bool IsMain = true;
    
    public virtual void OnFirstVisit() {}   // Called the first time this scene is pushed onto the stack.
    public virtual void OnVisit() {}        // Called every time this scene becomes the topmost scene on the stack.
    public virtual void OnRevisit() {}      // Called every time the scene above this one is popped and this scene becomes the topmost scene on the stack again.
    public virtual void OnSuspend() {}      // Called right before another scene is pushed on top of this one.
    public virtual void OnPop() {}          // Called right before this scene is popped from the stack. Will not be called when this scene is cleared from the stack.
    public virtual void OnRemove() {}       // Called every time this scene is removed from the stack, either from being popped or cleared.

    public override void _Ready()
    {
        /* If this scene is in the tree (_Ready() is called) but the scene stack is empty, that probably means the game was launched using f6.
        This is a special case that should only happen while debugging. We'll handle it by just pushing this scene as a main scene.
        If this is happening during a normal game launch it means no initial scenes are being pushed when the game starts. You can fix this
        by going to NormalGameLaunch.cs and calling Prime.SetScene() or Prime.PushScene() in _Ready() or _EnterTree(). */
        if (Prime.StackIsEmpty)
        {
            Prime.PushSceneForF6Launch(this);
        }
    }

    /// <summary> Called externally by the Prime class when this scene becomes the topmost scene on the stack. </summary>
    public void Visit(bool justPushed = false)
    {
        if (Active) { return; }
        
        if (!Visible) { Show(); }
        GetTree().SetInputAsHandled();      // Clear input when activating a new scene so input doesn't carry over from one scene to another.
        SetProcess(true);
        SetProcessInput(true);
        SetPhysicsProcess(true);
        Active = true;
        
        if (justPushed) { OnFirstVisit(); }
        else            { OnRevisit(); }
        OnVisit();
    }

    /// <summary> Called externally by the Prime class when another game scene is pushed on top of this one. </summary>
    public void Suspend()
    {
        if (!Active) { return; }

        SetVisible(false);
        SetProcess(false);
        SetPhysicsProcess(false);
        SetProcessInput(false);
        Active = false;
        
        OnSuspend();
    }

    /// <summary> Get the name of this scene for debug printing. </summary>
    public string DebugPrintName
    {
        get { return IsMain ? $"- (Main) {Name}" : $"   - (sub) {Name}"; }
    }
}
