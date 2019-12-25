using Godot;
using System;

public class GameScene : PrimeNode2D
{
    public bool ShowWhileDeactivated = false;
    public bool ProcessWhileDeactivated = false;
    public bool ProcessInputWhileDeactivated = false;
    
    public virtual void OnActivated() {}    // Called every time this state becomes the currently active state
    public virtual void OnDeactivated() {}  // Called right before another state is pushed on top of this state
    public virtual void OnPopped() {}       // Called right before this state is popped from the state stack
    public virtual void OnRemoved() {}      // Use for clean up; called whenever this state is removed from the state stack, either from being popped or cleared
}
