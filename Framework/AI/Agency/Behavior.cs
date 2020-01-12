using Godot;
using System;
using System.Collections.Generic;

/// A self-contained instruction for an agent to follow. Behaviors belong to agencies.
public abstract class Behavior : PrimeNode
{
    public List<Behavior> ChildBehaviors;
    
    public bool Active { private set; get; } = false;  /// Inactive behaviors don't process.
    private bool Instantaneous = false;                /// Instantaneous behaviors yield to the next behavior immediately, on the same frame.

    protected Agency ParentAgency 
    {
        get 
        {
            if (_ParentAgency == null)
            {
                _ParentAgency = GetAncestorOfType<Agency>();
            }
            return _ParentAgency;
        }
    }
    private Agency _ParentAgency = null;

    private Behavior ParentBehavior = null;

    [Export] public bool Disabled = false;             /// Can disable a behavior (and its children) without removing them. They won't be processed.
    [Export] public String DebugName = "";

    public bool IsActive() { return Active; }

    public override void _Ready()
    {
        base._Ready();
        if (!Disabled)
        {
            ParentBehavior = GetParent() as Behavior;
        }
    }

    public virtual void Setup()
    {

    }

    public void Begin()
    {
        Active = true;
        OnBegin();
        if (Instantaneous)
        {
            StopSelf();
        }
    }

    /// <summary> Called each time this behavior starts.virtual </summary>
    public virtual void OnBegin()
    {

    }

    /// <summary> Called every tick this behavior is active. </summary>
    public virtual void Process(float Delta)
    {

    }

    /// <summary> Called every tick regardless of whether or not this behavior is active. </summary>
    public virtual void InactiveProcess(float Delta)
    {
        if (Active)
        {
            foreach (Behavior Child in ChildBehaviors)
            {
                Child.InactiveProcess(Delta);
            }
        }
    }

    /// <summary> Called internally when the behavior determines that it needs to end. </summary>
    /// stop() should *not* be overridden in child classes to handle behavior finalization. Override Stop() instead.
    protected void StopSelf()
    {
       Stop();
       if (ParentBehavior != null && ParentBehavior.Active)
       {
           ParentBehavior.OnChildStop(this);
       }
    }

    /// <summary> Called externally when this behavior ends. Do not call explicitly from within this behavior; instead, if a behavior needs
    /// to end itself, it should call StopSelf(). </summary>
    /// Stop() should be overridden in child classes to handle behavior finalization. Don't override StopSelf() to do this.
    public virtual void Stop()
    {
        Active = false;
    }

    /// Callback from a child node that is called when it stops executing. This is ONLY called from the child's StopSelf method and will
    /// not be called from child.Stop().
    protected virtual void OnChildStop(Behavior Child)
    {

    }


    #region Events

        public virtual void ExecuteEvent()
        {
            if (Active && !Disabled)
            {
                foreach (Behavior Child in ChildBehaviors)
                {
                    if (Child.Active)
                    {
                        Child.ExecuteEvent();
                    }
                }
            }
        }

    #endregion


    #region Durations

        public virtual void StartDuration()
        {
            if (Active && !Disabled)
            {
                foreach (Behavior Child in ChildBehaviors)
                {
                    if (Child.Active)
                    {
                        Child.StartDuration();
                    }
                }
            }
        }

        public virtual void StopDuration()
        {
            if (Active && !Disabled)
            {
                foreach (Behavior Child in ChildBehaviors)
                {
                    if (Child.Active)
                    {
                        Child.StopDuration();
                    }
                }
            }
        }

    #endregion


    #region States

        public virtual Type GetNeededState()
        {
            return null;
        }

    #endregion

}
