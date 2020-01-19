using Godot;
using System;

public class IntLiteral : IntParameter
{
    private int Value;

    public IntLiteral(int value)
    {
        Value = value;
    }

    public override int Evaluate()
    {
        return Value;
    }
}
