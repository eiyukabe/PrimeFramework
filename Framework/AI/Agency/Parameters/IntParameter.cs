using Godot;
using System;

/// <summary> Class intended to represent an int parameter value for a behavior. </summary>
public abstract class IntParameter : Parameter
{
    public abstract int Evaluate();
}
