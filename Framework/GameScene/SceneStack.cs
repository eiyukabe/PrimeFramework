using Godot;
using System;
using System.Collections.Generic;

public class SceneStack
{
    private List<GameScene> Stack = new List<GameScene>();

    /// <summary> Get the topmost scene on the stack. Returns null if the stack is empty. </summary>
    public GameScene TopScene
    {
        get
        {
            if (Stack.Count > 0) { return Stack[Stack.Count - 1]; }
            return null;
        }
    }

    /// <summary> Get the topmost main scene index. Returns -1 if there is no main scene on the stack. </summary>
    public int TopMainSceneIndex
    {
        get
        {
            for (int i = Stack.Count - 1; i >= 0; i--)
            {
                if (Stack[i].IsMain) { return i; }
            }
            return -1;
        }
    }

    /// <summary> Push a scene onto the stack. </summary>
    public void Push(GameScene scene, Node parent)
    {
        TopScene?.Suspend();
        Stack.Add(scene);
        parent.AddChild(scene);
        scene.Visit(justPushed: true);
    }

    /// <summary> Pop the topmost scene off the stack. </summary>
    public void Pop()
    {
        var scene = TopScene;
        if (scene == null) { return; }
        Stack.RemoveAt(Stack.Count - 1);
        scene.OnPop();
        scene.OnRemove();
        scene.QueueFree();
    }

    /// <summary> Pop the topmost main scene off the stack and any subscenes above it. Noop if there's no main scene on the stack. </summary>
    public void PopMain()
    {
        int j = TopMainSceneIndex;
        if (j == -1)
        {
            return;         // No main scene to pop
        }

        for (int i = Stack.Count - 1; i > j; i--)
        {
            RemoveTop();    // Remove all subscenes above the main scene
        }
        
        Pop();              // Pop main scene
    }

    /// <summary> Remove the topmost scene on the stack. </summary>
    public void RemoveTop()
    {
        var scene = TopScene;
        Stack.RemoveAt(Stack.Count - 1);
        scene.OnRemove();
        scene.QueueFree();
    }

    /// <summary> Print the name of all scenes on the stack for debugging. </summary>
    public void PrintSceneStack()
    {
        GD.Print("--- Scene Stack ---");
        foreach (var scene in Stack)
        {
            GD.Print(scene.SceneName);
        }
    }
}
