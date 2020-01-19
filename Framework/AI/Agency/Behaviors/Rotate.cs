using Godot;
using System;

/// <summary>Behavior for rotating the agent</summary>
public class Rotate : Behavior
{
    [Export] private bool Clockwise = true;
    [Export] private float DegreesPerSecond = 360.0f;

    private float RadiansPerSecond = 0.0f;

    #region Initialization

        public override void Setup()
        {
            base.Setup();
            if (!Clockwise) { DegreesPerSecond = -DegreesPerSecond; }
            RadiansPerSecond = Mathf.Deg2Rad(DegreesPerSecond);
        }

    #endregion

    #region Control

        public override void Process(float delta)
        {
            base.Process(delta);
            
            if (ParentAgent != null)
            {
                ParentAgent.Rotation += RadiansPerSecond * delta;
            }
        }

    #endregion

}
