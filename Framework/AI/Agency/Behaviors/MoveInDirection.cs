using Godot;
using System;

/// <summary> Moves the owning agent in the defined direction (in degrees) for the specified amount of time.
/// A duration less than 0 will last forever. </summary>
public class MoveInDirection : Behavior
{
    [Export] private float Direction; // In degrees.
    [Export] private float Speed = 100.0f;
    [Export] private float Duration = 5.0f;

    private FloatParameter _Direction;
    private FloatParameter _Speed;
    private FloatParameter _Duration;

    private Vector2 DirectionVector;
    private float Timer = -1.0f;

    #region Initialization

        public override void Setup()
        {
            base.Setup();
            if (_Direction == null) { _Direction = new FloatLiteral(Direction); }
            if (_Speed == null)     { _Speed     = new FloatLiteral(Speed); }
            if (_Duration == null)  { _Duration  = new FloatLiteral(Duration); }
        }

        /// <summary> Called when a float parameter is detected. Override to assign to the proper variable. </summary>
        /// count tells how many float parameters have been identified so far and can be used to identify them.
        protected override void ReceiveFloatParameter(FloatParameter floatParameter, int count)
        {
            switch(count)
            {
                case 1: _Direction = floatParameter; break;
                case 2: _Speed     = floatParameter; break;
                case 3: _Duration  = floatParameter; break;
                default:                             break;
            }
        }

        public override void OnBegin()
        {
            float Dir = _Direction.Evaluate();
            DirectionVector = new Vector2(Mathf.Cos(Mathf.Deg2Rad(Dir)), Mathf.Sin(Mathf.Deg2Rad(Dir)));
            Timer = _Duration.Evaluate();
        }

    #endregion

    #region Control

        public override void Process(float delta)
        {
            base.Process(delta);
            
            if (ParentAgent != null)
            {
                ParentAgent.GlobalPosition += DirectionVector * _Speed.Evaluate() * delta;
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
