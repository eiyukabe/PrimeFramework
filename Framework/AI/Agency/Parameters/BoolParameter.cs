using Godot;
using System;

/// <summary> Class intended to represent a bool parameter value for a behavior. </summary>
public abstract class BoolParameter : Parameter
{
    public abstract bool Evaluate();
}
