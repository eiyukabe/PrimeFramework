using Godot;
using System;

public class IsInPlayfield : AgencyState
{
    public override bool Evaluate(Node2D Agent)
    {
        return false;
    }

    public override Type GetDurationClass()
    {
        return typeof(WhileInPlayfield);
    }
}