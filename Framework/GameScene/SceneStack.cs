using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// A stack of GameScenes; meant to be managed by the Prime class, not used directly. See also GameScene and Prime classes.
/// </summary>
public class SceneStack
{
    private Stack<GameScene> Stack = new Stack<GameScene>();

    public void Push(GameScene newState)
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

        GameScene state = Stack.Pop();
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
        
        foreach(GameScene state in Stack)
        {
            state.OnRemoved();
            state.QueueFree();
        }

        Stack.Clear();
    }

    private void Activate(GameScene state)
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

    private void Deactivate(GameScene state)
    {
        /* For GameStateManager's internal use only. This is called every time another state is pushed on top of this state */
        state.SetVisible(state.ShowWhileDeactivated);
        state.SetProcess(state.ProcessWhileDeactivated);
        state.SetPhysicsProcess(state.ProcessWhileDeactivated);
        state.SetProcessInput(state.ProcessInputWhileDeactivated);
        state.OnDeactivated();
    }
}
