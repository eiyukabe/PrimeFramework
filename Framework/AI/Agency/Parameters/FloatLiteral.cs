using Godot;
using System;

public class FloatLiteral : FloatParameter
{
    private float Value;

    public FloatLiteral(float value)
    {
        Value = value;
    }

    public override float Evaluate()
    {
        return Value;
    }
}
