using Godot;
using System;

/// <summary> ## Behavior for linearly interpolating the agent's alpha over a given time. To set the alpha immediately, see set_alpha.
/// The "interp_type" determines how the alpha is interpolated and defaults to linear. If sin or cos are used, the "duration" variable
/// refers to the wave phase and the alpha variable refers to the alpha at the peak of the wave (the starting alpha is the alpha at the
/// trough of the wave). If you want a sine or cosine interpolation to repeat, put it in a sequential_behavior that loops.</summary>
public class LerpAlpha : Behavior
{
    //TODO: Named types
    //export(int, "Linear", "Sin", "Cos") var interp_type = 0
    [Export] private int InterpType = 0; /// <summary> 0 = Linear, 1 = Sin, 2 = Cos </summary>

    [Export] private float Alpha = 1.0f;
    [Export] private float Duration = 1.0f;
    
    private float StartingAlpha = 1.0f;
    private float Timer = 0.0f;

    #region Initialization

        public override void OnBegin()
        {
            if (Duration <= 0.0f)
            {
                // No Duration, just set the alpha immediately.
                if (ParentAgent != null)
                {
                    ParentAgent.Modulate = new Color(ParentAgent.Modulate.r, ParentAgent.Modulate.g, ParentAgent.Modulate.b, Alpha);
                }
                StopSelf();
            }
            else
            {
                if (ParentAgent != null)
                {
                    StartingAlpha = ParentAgent.Modulate.a;
                    Timer = Duration;
                }
            }
        }

    #endregion


    #region Control

        public override void Process(float delta)
        {
            base.Process(delta);
            
            if (ParentAgent != null)
            {
                switch(InterpType)
                {
                    case 0: // Linear Interpolation
                    {
                        Color NewColor = ParentAgent.Modulate;
                        float ChangeFactor = delta/Duration;
                        ChangeFactor = Mathf.Clamp(ChangeFactor, ChangeFactor, 1.0f);
                        float ChangeAmount = (Alpha - StartingAlpha) * ChangeFactor;
                        NewColor.a += ChangeAmount;
                        ParentAgent.Modulate = NewColor;
                        break;
                    }
                    case 1: // Sine Interpolation
                    {
                        float AlphaRange = StartingAlpha - Alpha;
                        Color NewColor = ParentAgent.Modulate;
                        NewColor.a = StartingAlpha - AlphaRange/2 + AlphaRange/2 * Mathf.Sin((Duration - Timer) * 2 * Mathf.Pi/Duration);
                        ParentAgent.Modulate = NewColor;
                        break;
                    }
                    case 2: // Cosine Interpolation
                    {
                        float AlphaRange = StartingAlpha - Alpha;
                        Color NewColor = ParentAgent.Modulate;
                        NewColor.a = StartingAlpha - AlphaRange/2 + AlphaRange/2 * Mathf.Cos((Duration - Timer) * 2 * Mathf.Pi/Duration);
                        ParentAgent.Modulate = NewColor;
                        break;
                    }
                }
            }
            Timer -= delta;

            if (Timer <= 0.0f)
            {
                StopSelf();
            }
        }


        public override void Stop()
        {
            base.Stop();
            switch(InterpType)
            {
                case 1: // Sine Interpolation
                case 2: // Cosine Interpolation
                    if (ParentAgent != null)
                    {
                        Color NewColor = ParentAgent.Modulate;
                        NewColor.a = StartingAlpha;
                        ParentAgent.Modulate = NewColor;
                    }
                    break;
            }
        }

    #endregion

}
