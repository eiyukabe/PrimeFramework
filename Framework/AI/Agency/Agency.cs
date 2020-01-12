using Godot;
using System;
using System.Collections.Generic;

///
public class Agency : PrimeNode
{
    private List<Behavior> ChildBehaviors = new List<Behavior>();
    private bool Halted = false;

    private List<AgencyState> States = new List<AgencyState>();


    #region Initialization

        public override void _Ready()
        {
            base._Ready();

            ChildBehaviors = GetChildren<Behavior>();
            foreach (Behavior Child in ChildBehaviors)
            {
                Child.Setup();
                AddNeededStates(Child);
            }
        }

        public virtual void Begin()
        {
            foreach (Behavior Child in ChildBehaviors)
            {
                if (!(Child is Event) && !(Child is Duration))
                {
                    Child.Begin();
                }
            }
        }

        private void AddNeededStates(Behavior behavior)
        {
            AddState(behavior.GetNeededState());

            if (!(behavior is Event))
            {
                foreach (Behavior Child in behavior.ChildBehaviors)
                {
                    AddNeededStates(Child);
                }
            }
        }

        private void AddState(Type stateType)
        {
            // We will only add a new instance of this state type if no such instance already exists.
            Boolean StateAlreadyExists = false;
            foreach (AgencyState state in States)
            {
                if (state.GetType() == stateType)
                {
                    StateAlreadyExists = true;
                    break;
                }
            }

            if (!StateAlreadyExists)
            {
                AgencyState state = (AgencyState)Activator.CreateInstance(stateType);
                States.Add(state);
                AddChild(state);
            }
        }

    #endregion


    #region Control

        public virtual void Process(float delta)
        {
            if (Halted) { return; }

            // Update child behaviors.
            foreach (Behavior Child in ChildBehaviors)
            {
                Child.InactiveProcess(delta);
                if (!(Child is Event) && Child.Active)
                {
                    Child.Process(delta);
                }
            }
        }

        public virtual void End()
        {
        }

        public void Halt()
        {
            Halted = true;
        }

        public void Resume()
        {
            Halted = false;
        }

    #endregion


    #region Durations

        public void StartDuration()
        {
            if (Halted) { return; }

            foreach (Behavior Child in ChildBehaviors)
            {
                Child.StartDuration();
            }
        }

        public void StopDuration()
        {
            if (Halted) { return; }

            foreach (Behavior Child in ChildBehaviors)
            {
                Child.StopDuration();
            }
        }

        public bool IsDurationActive(Duration duration)
        {
            Type TheStateClass = duration.GetStateClass();

            AgencyState TheState = GetStateOfType(TheStateClass);
            
            if (TheState == null)
            {
                return false;
            }
            else
            {
                return TheState.Active;
            }
        }

    #endregion


    #region States

        private AgencyState GetStateOfType(Type stateClass)
        {
            if (stateClass != null)
            {
                foreach (AgencyState State in States)
                {
                    if (State.GetType() == stateClass)
                    {
                        return State;
                    }
                }
            }
            return null;
        }

    #endregion

}