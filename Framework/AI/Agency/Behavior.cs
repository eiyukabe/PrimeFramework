using Godot;
using System;
using System.Collections.Generic;

/// A self-contained instruction for an agent to follow. Behaviors belong to agencies.
public abstract class Behavior : PrimeNode
{
    public List<Behavior> ChildBehaviors = new List<Behavior>();
    
    public bool Active { private set; get; } = false;  /// Inactive behaviors don't process.
    protected bool Instantaneous = false;              /// Instantaneous behaviors yield to the next behavior immediately, on the same frame.

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

    public Node2D ParentAgent
    {
        get
        {
            if (_ParentAgent == null)
            {
                _ParentAgent = ParentAgency?.GetParent() as Node2D;
                if (_ParentAgent == null)
                {
                    Node Parent = GetParent();
                    if (Parent is Node2D)        { _ParentAgent = Parent as Node2D; }
                    else if (Parent is Behavior) { _ParentAgent = ((Behavior)Parent).ParentAgent; }
                }
            }
            return _ParentAgent;
        }
    }
    private Node2D _ParentAgent = null;

    private Behavior ParentBehavior = null;

    private bool ProcessSelf = false; ///<summary>Will be set to true for behaviors with no agencies ancestors so they can process themselves.</summary>

    [Export] public bool Disabled = false; /// <summary>Can disable a behavior (and its children) without removing them. They won't be processed.</summary>
    [Export] public String DebugName = "";

    public bool IsActive() { return Active; }


    #region Initialization

        public override void _Ready()
        {
            base._Ready();
            if (!Disabled)
            {
                ParentBehavior = GetParent() as Behavior;
                InitializeChildBehaviors();

                if (ParentAgency == null)
                {
                    ProcessSelf = true;
                    if (ParentBehavior == null)
                    {
                        Setup();
                    }
                }
            }
        }

        private void InitializeChildBehaviors()
        {
            int BoolParameterCount = 0;
            int IntParameterCount = 0;
            int FloatParameterCount = 0;
            foreach (Node Child in GetChildren())
            {
                if (Child is Behavior)
                {
                    Behavior ChildBehavior = (Behavior)Child;
                    ChildBehaviors.Add(ChildBehavior);
                    ChildBehavior.Setup();
                }
                else if (Child is BoolParameter)
                {
                    BoolParameterCount++;
                    ReceiveBoolParameter(Child as BoolParameter, BoolParameterCount);
                }
                else if (Child is IntParameter)
                {
                    IntParameterCount++;
                    ReceiveIntParameter(Child as IntParameter, IntParameterCount);
                }
                else if (Child is FloatParameter)
                {
                    FloatParameterCount++;
                    ReceiveFloatParameter(Child as FloatParameter, FloatParameterCount);
                }
            }
        }

        /// <summary> Called when a bool parameter is detected. Override to assign to the proper variable. </summary>
        /// count tells how many bool parameters have been identified so far and can be used to identify them.
        protected virtual void ReceiveBoolParameter(BoolParameter boolParameter, int count)
        {

        }

        /// <summary> Called when an int parameter is detected. Override to assign to the proper variable. </summary>
        /// count tells how many int parameters have been identified so far and can be used to identify them.
        protected virtual void ReceiveIntParameter(IntParameter intParameter, int count)
        {

        }

        /// <summary> Called when a float parameter is detected. Override to assign to the proper variable. </summary>
        /// count tells how many float parameters have been identified so far and can be used to identify them.
        protected virtual void ReceiveFloatParameter(FloatParameter floatParameter, int count)
        {

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

        public override void _Process(float delta)
        {
            base._Process(delta);
            if (ProcessSelf) 
            { 
                Process(delta);
            }
        }

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
