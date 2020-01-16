using Godot;
using System;

public class Wait : Behavior
{
    [Export] private float WaitDuration = 1.0f;

    private float WaitTimer = 0.0f;


    #region Initialization

        public override void OnBegin()
        {
            WaitTimer = WaitDuration;
        }

    #endregion


    #region Control

        public override void Process(float delta)
        {
            base.Process(delta);
            if (WaitTimer > 0.0)
            {
                WaitTimer -= delta;
            }

            if (WaitTimer <= 0.0)
            {
                StopSelf();
            }
        }

    #endregion

}
