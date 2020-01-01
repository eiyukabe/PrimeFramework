using Godot;
using System;

/// <summary>
/// Attach a node with this script to a Camera2D to let the player move the camera.
/// </summary>
public class CameraMovementAddon : Node
{
    [Export] public int MoveSpeed = 600;
    [Export] public Vector2 ZoomStepRate = new Vector2(1.5f, 1.5f);     // How much to zoom in/out each frame when zooming

    private Camera2D Parent;
    private Vector2 StartingPosition;   // Used when resetting the camera
    private bool KeysBound = false;

    public override void _EnterTree()
    {
        if (GetParent() is Camera2D) { Parent = (Camera2D) GetParent(); }
        else                         { QueueFree(); return; }
        
        StartingPosition = Parent.GlobalPosition;
        
        if (!KeysBound)
        {
            Bind.BindCameraDebugKeys();
            KeysBound = true;
        }
    }

    public override void _Process(float delta)
    {
        /* Zoom Controls */
        if      (Input.IsActionPressed(InputActions.ZOOM_IN_DEBUG_CAMERA))   { Parent.Zoom -= Parent.Zoom * ZoomStepRate * delta; }
        else if (Input.IsActionPressed(InputActions.ZOOM_OUT_DEBUG_CAMERA))  { Parent.Zoom += Parent.Zoom * ZoomStepRate * delta; }
        else if (Input.IsActionPressed(InputActions.RESET_DEBUG_CAMERA))
        {
            Parent.Zoom = new Vector2(1, 1);
            Parent.GlobalPosition = StartingPosition;
        }

        /* Movement Controls */
        if      (Input.IsActionPressed(InputActions.MOVE_DEBUG_CAMERA_DOWN))  { Parent.GlobalPosition += new Vector2(0, MoveSpeed * delta); }
        else if (Input.IsActionPressed(InputActions.MOVE_DEBUG_CAMERA_UP))    { Parent.GlobalPosition -= new Vector2(0, MoveSpeed * delta); }
        else if (Input.IsActionPressed(InputActions.MOVE_DEBUG_CAMERA_LEFT))  { Parent.GlobalPosition -= new Vector2(MoveSpeed * delta, 0); }
        else if (Input.IsActionPressed(InputActions.MOVE_DEBUG_CAMERA_RIGHT)) { Parent.GlobalPosition += new Vector2(MoveSpeed * delta, 0); }
    }
}
