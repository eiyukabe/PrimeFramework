using Godot;
using System;

/// <summary>
/// Use Collision<T> to say when 'caller' collides with 'T' call 'this method' on 'caller' with 'T' as a parameter.
/// When instantiated, nodes of this class will be added as a child of the caller's Area2D node.
/// 
/// Example usage: new Collision<Enemy>(this, "Area2D", OnEnemyCollision);
/// The caller must then have this method defined: OnEnemyCollision(Enemy enemy)
/// </summary>
public class Collision<T> : Node where T : Node
{
    private Action<T> Callback;

    /// <summary>
    /// Parameters:
    /// - caller: The node creating this Collision.
    /// - area2DPath: Path to an Area2D node that will detect this collision.
    /// - callback: Method that will be called when the caller detects a collision with the given type. This method must have
    /// one parameter with type <T>. 
    /// </summary>
    public Collision(Node caller, string area2DPath, Action<T> callback)
    {
        var collisionArea2D = caller.GetNodeOrNull<Area2D>(area2DPath);
        if (collisionArea2D == null)
        {
            return;
        }

        Callback = callback;
        collisionArea2D.Connect(Signals.AREA_ENTERED, this, nameof(OnCollision));
        collisionArea2D.AddChild(this);
    }

    private void OnCollision(Area2D otherArea)
    {
        var other = otherArea.GetParentOrNull<T>();
        if (other == null)
        {
            return;
        }

        Callback?.Invoke(other);
    }
}
