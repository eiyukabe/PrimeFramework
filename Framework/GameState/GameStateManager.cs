using Godot;
using System;
using System.Collections.Generic;

public static class GameStateManager
{
    private static Stack<GameState> Stack = new Stack<GameState>();

    public static void PushState(GameState newState, Node stateParent)
    {
        /* Note: stateParent can be null if pushing a state that's already in the SceneTree. */

        if (Stack.Count > 0)
        {
            Deactivate(Stack.Peek());
        }
        
        if (stateParent != null)
        {
            stateParent.AddChild(newState);
        }

        Stack.Push(newState);
        Activate(newState);
    }

    public static void PopState()
    {
        GameState state = Stack.Pop();
        state.OnPop();
        state.OnRemove();
        state.QueueFree();

        if (Stack.Count > 0)
        {
            Activate(Stack.Peek());
        }
    }

    public static void ClearStack()
    {
        foreach(GameState state in Stack)
        {
            state.OnRemove();
            state.QueueFree();
        }

        Stack.Clear();
    }

    public static void ClearAndPush(GameState newState, Node stateParent)
    {
        ClearStack();
        PushState(newState, stateParent);
    }

    private static void Activate(GameState state)
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
        state.OnActivate();
    }

    private static void Deactivate(GameState state)
    {
        /* For GameStateManager's internal use only. This is called every time another state is pushed on top of this state */
        state.SetVisible(state.ShowWhileDeactivated);
        state.SetProcess(state.ProcessWhileDeactivated);
        state.SetPhysicsProcess(state.ProcessWhileDeactivated);
        state.SetProcessInput(state.ProcessInputWhileDeactivated);
        state.OnDeactivate();
    }
}
