using Godot;
using System;
using System.Collections.Generic;

public class PrimeNode : Node
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
        var ancestor = GetParent();
        while (ancestor != null)
        {
            if (ancestor is T)
            {
                return (T)ancestor;
            }
            ancestor = ancestor.GetParent();
        }
        return null;
    }

}
