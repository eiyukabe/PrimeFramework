using Godot;
using System;

///
public abstract class Event : Behavior
{

    public override void OnBegin()
    {
        
    }

    public override void InactiveProcess(float Delta)
    {
        base.InactiveProcess(Delta);
    }

}
