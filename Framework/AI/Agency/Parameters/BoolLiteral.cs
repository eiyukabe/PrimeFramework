using Godot;
using System;

public class BoolLiteral : BoolParameter
{
    private bool Value;

    public BoolLiteral(bool value)
    {
        Value = value;
    }

    public override bool Evaluate()
    {
        return Value;
    }
}
