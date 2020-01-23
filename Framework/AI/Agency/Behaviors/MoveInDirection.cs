using Godot;
using System;

/// <summary> Moves the owning agent in the defined direction (in degrees) for the specified amount of time.
/// A duration less than 0 will last forever. </summary>
public class MoveInDirection : Behavior
{
    [Export] private float Direction; // In degrees.
    [Export] private float Speed = 100.0f;
    [Export] private float Duration = 5.0f;

    private Vector2 DirectionVector;
    private float Timer = -1.0f;

    #region Initialization

        public override void OnBegin()
        {
            DirectionVector = new Vector2(Mathf.Cos(Mathf.Deg2Rad(Direction)), Mathf.Deg2Rad(Mathf.Sin(Direction)));
            Timer = Duration;
        }

    #endregion

    #region Control

        public override void Process(float delta)
        {
            base.Process(delta);
            
            if (ParentAgent != null)
            {
                ParentAgent.GlobalPosition += DirectionVector * Speed * delta;
            }

            if (Timer > 0.0f)
            {
                Timer -= delta;
                if (Timer <= 0.0f)
                {
                    StopSelf();
                }
            }
        }

    #endregion

}
