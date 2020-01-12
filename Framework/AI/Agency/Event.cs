using Godot;
using System;

/// A special type of behavior that is fired off when something external to the agency occurs (e.g. the agent dying).
public abstract class Event : Behavior
{

    #region Initialization

        public override void OnBegin()
        {
            foreach (Behavior Child in ChildBehaviors)
            {
                Child.Begin();
            }
        }

    #endregion


    #region Control

        public override void InactiveProcess(float delta)
        {
            base.InactiveProcess(delta);

            foreach (Behavior Child in ChildBehaviors)
            {
                if (!(Child is Event) && Child.Active)
                {
                    Child.Process(delta);
                }
            }
        }

    #endregion


    #region Durations

        public override void StartDuration(Type durationType)
        {
            // Do nothing
        }

        public override void StopDuration(Type durationType)
        {
            // Do nothing
        }

    #endregion


    #region Events

        public override void ExecuteEvent(Type eventType)
        {
            if (GetType() == eventType)
            {
                Begin();
            }
        }

    #endregion

}
