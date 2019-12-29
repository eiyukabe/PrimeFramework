using Godot;
using System;

public class GameScene : PrimeNode2D
{
    public bool Active = false;
    public bool IsMain = true;
    public bool ShowWhileSuspended = false;
    public bool ProcessWhileSuspended = false;
    public bool ProcessInputWhileSuspended = false;
    
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
        by going to NormalGameLaunch.cs and calling Prime.SetScene() (or any variant that pushes to the stack) on _Ready() or _EnterTree(). */
        if (Prime.StackIsEmpty)
        {
            Prime.F6LaunchPushScene(this);
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

        SetVisible(ShowWhileSuspended);
        SetProcess(ProcessWhileSuspended);
        SetPhysicsProcess(ProcessWhileSuspended);
        SetProcessInput(ProcessInputWhileSuspended);
        Active = false;
        
        OnSuspend();
    }

    /// <summary> Get the name of this scene for debug printing. </summary>
    public string SceneName
    {
        get { return IsMain ? $"- (Main) {Name}" : $"   - (sub) {Name}"; }
    }
}
