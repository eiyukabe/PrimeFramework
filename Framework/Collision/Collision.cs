using Godot;
using System;

/// <summary>
/// Use Collision<T> to say when 'caller' collides with 'T' call 'this method' on 'caller' with 'T' as a parameter.
/// When instantiated, nodes of this class will be added as a child of the caller's Area2D node.
/// 
/// Example usage: new Collision<Enemy>(this, OnEnemyCollision, "Area2D", "../../");
/// The caller must then have this method defined: OnEnemyCollision(Enemy enemy)
/// </summary>
public class Collision<T> : Node where T : Node
{
    private Action<T> Callback;

    /// <summary>
    /// Parameters:
    /// - caller: The node creating this Collision.
    /// - callback: Method that will be called when the caller detects a collision with the given type. This method must have
    ///   one parameter with type <T>. 
    /// - area2DPath: Path to the caller's Area2D node that's detecting this collision.
    /// </summary>
    public Collision(Node caller, Action<T> callback, string area2DPath = "Area2D")
    {
        Area2D area2D = caller.GetNodeOrNull<Area2D>(area2DPath);
        if (area2D == null)
        {
            return;
        }

        Callback = callback;
        area2D.Connect(CollisionSignals.AREA_ENTERED, this, nameof(OnCollision));
        area2D.AddChild(this);
    }

    private void OnCollision(Area2D otherArea)
    {
        T thingWeCollidedWith = Prime.GetAncestorOfType<T>(otherArea);
        if (thingWeCollidedWith == null)
        {
            return;
        }
        else
        {
            Callback?.Invoke(thingWeCollidedWith);
        }
    }
}
