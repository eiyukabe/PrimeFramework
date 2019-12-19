using Godot;
using System;

public class GameState : PrimeNode2D
{
    public bool ShowWhileDeactivated = false;
    public bool ProcessWhileDeactivated = false;
    public bool ProcessInputWhileDeactivated = false;
    
    public virtual void OnActivate() {}    // Called every time this state becomes the currently active state
    public virtual void OnDeactivate() {}  // Called right before another state is pushed on top of this state
    public virtual void OnPop() {}          // Called right before this state is popped from the state stack
    public virtual void OnRemove() {}       // Use for clean up; called whenever this state is removed from the state stack, either from being popped or cleared
}
