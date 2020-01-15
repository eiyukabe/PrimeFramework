using Godot;
using System;

/// <summary> All sub behaviors of this behavior will be processed while it is active. </summary>
public class ParallelBehavior : Behavior
{
    
    #region Initialization

        public override void OnBegin()
        {
            foreach (Behavior Child in ChildBehaviors)
            {
                if (!(Child is Event))
                {
                    Child.Begin();
                }
            }
        }

    #endregion


    #region Control

        public override void Process(float delta)
        {
            base.Process(delta);
            foreach (Behavior Child in ChildBehaviors)
            {
                if (!(Child is Event))
                {
                    if (Child.Active)
                    {
                        Child.Process(delta);
                    }
                }
            }
        }

        public override void Stop()
        {
            base.Stop();
            foreach (Behavior Child in ChildBehaviors)
            {
                if (!(Child is Event))
                {
                    Child.Stop();
                }
            }
        }

        protected override void OnChildStop(Behavior child)
        {
            foreach (Behavior Child in ChildBehaviors)
            {
                if (!(Child is Event))
                {
                    if (Child.Active)
                    {
                        return; // Won't stop if we have running children.
                    }
                }
            }
            StopSelf(); // Stop when no more children are running.
        }

    #endregion

}
