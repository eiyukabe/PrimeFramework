using Godot;
using System;
using System.Collections.Generic;

/// A self-contained instruction for an agent to follow. Behaviors belong to agencies.
public abstract class Behavior : PrimeNode
{
    public List<Behavior> ChildBehaviors = new List<Behavior>();
    
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
    private Node2D ParentAgent = null;

    private Behavior ParentBehavior = null;

    [Export] public bool Disabled = false;             /// Can disable a behavior (and its children) without removing them. They won't be processed.
    [Export] public String DebugName = "";

    public bool IsActive() { return Active; }

    public Node2D GetAgent()
    {
        return ParentAgency?.GetParent() as Node2D;
    }


    #region Initialization

        public override void _Ready()
        {
            base._Ready();
            if (!Disabled)
            {
                ParentBehavior = GetParent() as Behavior;
                InitializeChildBehaviors();
            }
        }

        private void InitializeChildBehaviors()
        {
            foreach (Node child in GetChildren())
            {
                if (child is Behavior)
                {
                    Behavior ChildBehavior = (Behavior)child;
                    ChildBehaviors.Add(ChildBehavior);
                    ChildBehavior.Setup();
                }
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

    #endregion


    #region Control

        /// <summary> Called every tick this behavior is active. </summary>
        public virtual void Process(float delta)
        {

        }

        /// <summary> Called every tick regardless of whether or not this behavior is active. </summary>
        public virtual void InactiveProcess(float delta)
        {
            if (Active)
            {
                foreach (Behavior Child in ChildBehaviors)
                {
                    Child.InactiveProcess(delta);
                }
            }
        }

        /// <summary> Called internally when the behavior determines that it needs to end. </summary>
        /// stop() should *not* be overridden in child classes to handle behavior finalization. Override Stop() instead.
        protected virtual void StopSelf()
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
        protected virtual void OnChildStop(Behavior child)
        {

        }

    #endregion


    #region Durations

        public virtual void StartDuration(Type durationType)
        {
            if (Active && !Disabled)
            {
                foreach (Behavior Child in ChildBehaviors)
                {
                    if (Child.Active)
                    {
                        Child.StartDuration(durationType);
                    }
                }
            }
        }

        public virtual void StopDuration(Type durationType)
        {
            if (Active && !Disabled)
            {
                foreach (Behavior Child in ChildBehaviors)
                {
                    if (Child.Active)
                    {
                        Child.StopDuration(durationType);
                    }
                }
            }
        }

    #endregion


    #region Events

        public virtual void ExecuteEvent(Type eventType)
        {
            if (Active && !Disabled)
            {
                foreach (Behavior Child in ChildBehaviors)
                {
                    if (Child.Active)
                    {
                        Child.ExecuteEvent(eventType);
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
