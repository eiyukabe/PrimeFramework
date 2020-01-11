using Godot;
using System;
using System.Collections.Generic;

///
public class Agency : PrimeNode
{
    private List<Behavior> ChildBehaviors = new List<Behavior>();
    private bool Halted = false;

    public override void _Ready()
    {
        base._Ready();

        ChildBehaviors = GetChildren<Behavior>();
        foreach (Behavior Child in ChildBehaviors)
        {
            Child.Setup();
        }
    }

    public virtual void Begin()
    {
        foreach (Behavior Child in ChildBehaviors)
        {
            if (!(Child is Event) && !(Child is Duration))
            {
                Child.Begin();
            }
        }
    }

    public virtual void Process(float Delta)
    {
        if (Halted) { return; }

        // Update child behaviors.
        foreach (Behavior Child in ChildBehaviors)
        {
            Child.InactiveProcess(Delta);
            if (!(Child is Event) && Child.Active)
            {
                Child.Process(Delta);
            }
        }
    }

    public virtual void End()
    {
    }

    public void Halt()
    {
	    Halted = true;
    }

    public void Resume()
    {
	    Halted = false;
    }

#region Durations

    public void StartDuration()
    {
        if (Halted) { return; }
    }

    public void StopDuration()
    {
        if (Halted) { return; }
    }

    public bool IsDurationActive(Duration TheDuration)
    {
        //TODO: Implement this.
        return false;
    }

#endregion

}