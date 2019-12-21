using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// GameStateStack is meant to be managed by the Prime class, not used directly. See also GameState and Prime classes.
/// </summary>
public class GameStateStack
{
    private Stack<GameState> Stack = new Stack<GameState>();

    public void Push(GameState newState)
    {
        if (Stack.Count > 0)
        {
            Deactivate(Stack.Peek());
        }

        Stack.Push(newState);
        Activate(newState);
    }

    public void Pop()
    {
        if (Stack.Count == 0)
        {
            return;
        }

        GameState state = Stack.Pop();
        state.OnPopped();
        state.OnRemoved();
        state.QueueFree();

        if (Stack.Count > 0)
        {
            Activate(Stack.Peek());
        }
    }

    public void Clear()
    {
        if (Stack.Count == 0)
        {
            return;
        }
        
        foreach(GameState state in Stack)
        {
            state.OnRemoved();
            state.QueueFree();
        }

        Stack.Clear();
    }

    private void Activate(GameState state)
    {
        /* For GameStateManager's internal use only. This is called every time a state becomes the active state */
        
        // Clear input when visiting a new state so input doesn't carry over from one state to another
        state.GetTree().SetInputAsHandled();

        if (!state.Visible)
        {
            state.Show();
        }
        state.SetProcess(true);
        state.SetProcessInput(true);
        state.SetPhysicsProcess(true);
        state.OnActivated();
    }

    private void Deactivate(GameState state)
    {
        /* For GameStateManager's internal use only. This is called every time another state is pushed on top of this state */
        state.SetVisible(state.ShowWhileDeactivated);
        state.SetProcess(state.ProcessWhileDeactivated);
        state.SetPhysicsProcess(state.ProcessWhileDeactivated);
        state.SetProcessInput(state.ProcessInputWhileDeactivated);
        state.OnDeactivated();
    }
}
