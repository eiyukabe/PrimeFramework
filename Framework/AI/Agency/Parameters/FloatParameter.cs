using Godot;
using System;

/// <summary> Class intended to represent a float parameter value for a behavior. </summary>
public abstract class FloatParameter : Parameter
{
    public abstract float Evaluate();
}
