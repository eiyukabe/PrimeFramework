using Godot;
using System;

public class GameScene : PrimeNode2D
{
    public bool ShowWhileDeactivated = false;
    public bool ProcessWhileDeactivated = false;
    public bool ProcessInputWhileDeactivated = false;
    public bool IsMain = true;
    public bool Active = false;
    
    public virtual void OnFirstActivated() {}   // Called the first time this scene is pushed onto the stack.
    public virtual void OnActivated() {}        // Called every time this scene becomes the top most scene on the stack.
    public virtual void OnDeactivated() {}      // Called right before another scene is pushed on top of this scene.
    public virtual void OnPopped() {}           // Called right before this scene is popped from the stack.
    public virtual void OnRemoved() {}          // Use for clean up; called whenever this scene is removed from the stack, either from being popped or cleared.

    /// <summary> Called externally by the SceneStack class when this scene becomes the top most scene on the stack. </summary>
    public void Activate(bool justPushed = false)
    {
        if (Active) { return; }
        
        if (!Visible) { Show(); }
        GetTree().SetInputAsHandled();      // Clear input when activating a new scene so input doesn't carry over from one scene to another.
        SetProcess(true);
        SetProcessInput(true);
        SetPhysicsProcess(true);
        Active = true;
        
        if (justPushed) { OnFirstActivated(); }
        OnActivated();
    }

    /// <summary> Called externally by the SceneStack class when another game scene is pushed on top of this one. </summary>
    public void Deactivate()
    {
        if (!Active) { return; }

        SetVisible(ShowWhileDeactivated);
        SetProcess(ProcessWhileDeactivated);
        SetPhysicsProcess(ProcessWhileDeactivated);
        SetProcessInput(ProcessInputWhileDeactivated);
        Active = false;
        
        OnDeactivated();
    }

    /// <summary> Get the name of this scene for debug printing. </summary>
    public string SceneName
    {
        get { return IsMain ? $"- (Main) {Name}" : $"   - (sub) {Name}"; }
    }
}
