using Godot;
using System;

///
public class AgencyState : PrimeNode
{
    public bool Uninitialized = true;
    public bool Active = false;

    public bool UpdateState(Node Agent)
    {
        bool FirstInitialization = Uninitialized;
        Uninitialized = false;
        bool OldStateActive = Active;
        Active = Evaluate(Agent);
        return FirstInitialization || (OldStateActive != Active);
    }

    /// Override in child classes to return true when the agent is in this state.
    public virtual bool Evaluate(Node Agent)
    {
        return false;
    }
    
}
