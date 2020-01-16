using Godot;
using System;

/// <summary> Behavior for linearly interpolating the agent's scale over a given time. To set the scale immediately, see set_scale. </summary>
public class LerpScale : Behavior
{
    [Export] private float Scale = 1.0f;
    [Export] private float Duration = 1.0f;

    [Export] private bool ScaleWidth = true;
    [Export] private bool ScaleHeight = true;

    private Vector2 StartingScale;
    private Vector2 TargetScale;
    private float StartingScaleY = 1.0f;
    private float Timer = 0.0f;

    #region Initialization

        public override void OnBegin()
        {
            if (Duration <= 0.0f)
            {
                Node2D Agent = GetAgent();
                if (Agent != null)
                {
                    Vector2 NewScale = new Vector2(Agent.Scale.x, Agent.Scale.y);
                    if (ScaleWidth) { NewScale.x = Scale; }
                    if (ScaleHeight) { NewScale.y = Scale; }
                    Agent.SetScale(NewScale);
                }
                StopSelf();
            }
            else
            {
                Node2D Agent = GetAgent();
                if (Agent != null)
                {
                    StartingScale = Agent.Scale;
                    TargetScale = new Vector2(Scale, Scale);
                    Timer = Duration;
                }
            }
        }

    #endregion


    #region Control

        public override void Process(float delta)
        {
            base.Process(delta);
            
            Node2D Agent = GetAgent();
            if (Agent != null)
            {
                float ChangeFactor = delta/Duration;
                ChangeFactor = Mathf.Clamp(ChangeFactor, ChangeFactor, 1.0f);
                Vector2 ChangeAmount = (TargetScale - StartingScale) * ChangeFactor;
                Vector2 NewScale = new Vector2(Agent.Scale.x, Agent.Scale.y);
                if (ScaleWidth) { NewScale.x = Agent.Scale.x + ChangeAmount.x; }
                if (ScaleHeight) { NewScale.y = Agent.Scale.y + ChangeAmount.y; }
                Agent.SetScale(NewScale);
            }
        }

    #endregion

}
