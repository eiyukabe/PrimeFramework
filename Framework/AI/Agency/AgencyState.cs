using Godot;
using System;

///
public abstract class AgencyState : PrimeNode
{
    public bool Uninitialized = true;
    public bool Active = false;

    public bool UpdateState(Node2D Agent)
    {
        bool FirstInitialization = Uninitialized;
        Uninitialized = false;
        bool OldStateActive = Active;
        Active = Evaluate(Agent);
        return FirstInitialization || (OldStateActive != Active);
    }

    /// Override in child classes to return true when the agent is in this state.
    public virtual bool Evaluate(Node2D Agent)
    {
        return false;
    }


    #region Durations

        public abstract Type GetDurationClass();

    #endregion
    
}
