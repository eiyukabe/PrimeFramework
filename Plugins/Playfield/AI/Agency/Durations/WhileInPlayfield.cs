using Godot;
using System;

public class WhileInPlayfield : Duration
{

    public override Type GetNeededState()
    {
        return typeof(IsInPlayfield);
    }
}