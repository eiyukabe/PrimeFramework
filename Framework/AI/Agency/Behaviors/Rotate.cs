using Godot;
using System;

/// <summary>Behavior for rotating the agent</summary>
public class Rotate : Behavior
{
    [Export] private bool Clockwise = true;
    [Export] private float DegreesPerSecond = 360.0f;

    private BoolParameter _Clockwise;
    private FloatParameter _DegreesPerSecond;

    #region Initialization

        public override void Setup()
        {
            base.Setup();
            if (_Clockwise == null)         { _Clockwise = new BoolLiteral(Clockwise); }
            if (_DegreesPerSecond == null)  { _DegreesPerSecond = new FloatLiteral(DegreesPerSecond); }
        }

        /// <summary> Called when a bool parameter is detected. Override to assign to the proper variable. </summary>
        /// count tells how many bool parameters have been identified so far and can be used to identify them.
        protected override void ReceiveBoolParameter(BoolParameter boolParameter, int count)
        {
            switch(count)
            {
                case 1:
                    _Clockwise = boolParameter;
                    break;
                default:
                    break;
            }
        }

        /// <summary> Called when a float parameter is detected. Override to assign to the proper variable. </summary>
        /// count tells how many float parameters have been identified so far and can be used to identify them.
        protected override void ReceiveFloatParameter(FloatParameter floatParameter, int count)
        {
            switch(count)
            {
                case 1:
                    _DegreesPerSecond = floatParameter;
                    break;
                default:
                    break;
            }
        }

    #endregion

    #region Control

        public override void Process(float delta)
        {
            base.Process(delta);
            
            if (ParentAgent != null)
            {
                if (_Clockwise.Evaluate())
                {
                    ParentAgent.Rotation += Mathf.Deg2Rad(_DegreesPerSecond.Evaluate()) * delta;
                }
                else
                {
                    ParentAgent.Rotation -= Mathf.Deg2Rad(_DegreesPerSecond.Evaluate()) * delta;
                }
            }
        }

    #endregion

}
