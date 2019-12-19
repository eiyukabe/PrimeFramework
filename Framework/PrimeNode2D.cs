using Godot;
using System;
using System.Collections.Generic;

public class PrimeNode2D : Node2D
{
    public List<T> GetChildren<T>() where T : Node
    {
        var results = new List<T>();

        foreach (Node node in GetChildren())
        {
            if (node is T)
            {
                results.Add((T)node);
            }
        }

        return results;
    }

    public T GetAncestorOfType<T>() where T : Node
    {
        var parent = GetParent();
        if (parent is T)
        {
            return (T)parent;
        }
        else if (parent is PrimeNode2D)
        {
            return ((PrimeNode2D)parent).GetAncestorOfType<T>();
        }
        return null;
    }

}
