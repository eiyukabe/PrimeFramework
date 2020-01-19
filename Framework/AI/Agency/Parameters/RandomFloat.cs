using Godot;
using System;

/// <summary> Class intended to represent a float parameter value for a behavior. </summary>
public abstract class RandomFloat : FloatParameter
{
    [Export] float MinFloat = -1.0f;
    [Export] float MaxFloat = 1.0f;

    private float Value = 0.0f;

    public override void _Ready()
    {
        Value = Prime.GetRandomFloat(MinFloat, MaxFloat);
    }

    public override float Evaluate()
    {
        return Value;
    }
}
