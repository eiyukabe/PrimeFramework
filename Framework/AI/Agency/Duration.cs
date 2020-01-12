using Godot;
using System;


///
public abstract class Duration : Behavior
{
    public const uint ACTIVATED_STATE = 0;
    public const int HAS_CHILD_ENEMIES = 1;
    public const int FRAMEWORK_DURATION_END = 2;

    //

    public const int GAME_DURATION_1 = FRAMEWORK_DURATION_END + 1;

    private AgencyState AssociatedState = null;


    #region Initialization

        public override void OnBegin()
        {
            base.OnBegin();
            foreach (Behavior Child in ChildBehaviors)
            {
                if (Child is Duration)
                {
                    ((Duration)Child).ConsiderBeginning();
                }
                else if (!(Child is Event))
                {
                    Child.Begin();
                }
            }
        }

        private void ConsiderBeginning()
        {
            if (IsDurationActive())
            {
                Begin();
            }
        }

    #endregion
    

    #region Control

        public override void Process(float delta)
        {
            base.Process(delta);
            foreach (Behavior Child in ChildBehaviors)
            {
                if (!(Child is Event) && Child.Active)
                {
                    Child.Process(delta);
                }
            }
        }

        public override void InactiveProcess(float delta)
        {
            foreach (Behavior Child in ChildBehaviors)
            {
                Child.InactiveProcess(delta);
            }
        }

        public override void Stop()
        {
            base.Stop();
            foreach (Behavior Child in ChildBehaviors)
            {
                Child.Stop();
            }
        }

    #endregion


    #region Duration

        private bool IsDurationActive()
        {
            if (ParentAgency != null && ParentAgency.IsDurationActive(this))
            {
                return true;
            }
            return false;
        }

        public override void StartDuration(Type durationType)
        {
            if (GetType() == durationType)
            {
                Begin();
            }
            if (Active)
            {
                foreach (Behavior Child in ChildBehaviors)
                {
                    Child.StartDuration(durationType);
                }
            }
        }

        public override void StopDuration(Type durationType)
        {
            if (Active)
            {
                foreach (Behavior Child in ChildBehaviors)
                {
                    Child.StopDuration(durationType);
                }
                if (GetType() == durationType)
                {
                    Stop();
                }
            }
        }

    #endregion


    #region States

        public override abstract Type GetNeededState();
        //{
        //   return typeof(AgencyState);
        //}

    #endregion
}
