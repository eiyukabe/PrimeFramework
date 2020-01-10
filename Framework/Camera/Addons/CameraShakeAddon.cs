using Godot;
using System;

public class CameraShakeAddon : Node
{
    [Export] public float MaxHorizontalShake = 1f;          // Max distance the camera can shake horizontally.
    [Export] public float MaxVerticalShake = 1f;            // Max distance the camera can shake vertically.
    [Export] public float MaxRotationalShake = 0.01f;       // Max distance the camera can rotate when shaking.
    [Export] public float ShakeDecreasePerSecond = 0.5f;
    
    private Camera2D Parent;


    #region Shake Properties

    public float Shake
    {
        get { return _shake; }
        set
        {
            _shake = Mathf.Clamp(value, 0f, 1f);
            _horizontalShake = _shake;
            _verticalShake = _shake;
            _rotationalShake = _shake;
        }
    }
    private float _shake = 0f;

    public float HorizontalShake
    {
        get { return _horizontalShake; }
        set { _horizontalShake = Mathf.Clamp(value, 0f, 1f); }
    }
    private float _horizontalShake = 0f;

    public float VerticalShake
    {
        get { return _verticalShake; }
        set { _verticalShake = Mathf.Clamp(value, 0f, 1f); }
    }
    private float _verticalShake = 0f;

    public float RotationalShake
    {
        get { return _rotationalShake; }
        set { _rotationalShake = Mathf.Clamp(value, 0f, 1f); }
    }
    private float _rotationalShake = 0f;

    #endregion


    public override void _EnterTree()
    {
        if (GetParent() is Camera2D) { Parent = (Camera2D) GetParent(); }
        else                         { QueueFree(); return; }

        Parent.SetRotating(true);
    }

    public override void _Input(InputEvent ev)
    {
        if (ev.IsActionPressed(InputActions.UI_HOME))
        {
            Shake = 1f;
        }
    }

    public override void _Process(float delta)
    {
        if (Shake > 0)
        {
            Shake -= ShakeDecreasePerSecond * delta;
        }

        ManageHorizontalShake(delta);
        ManageVerticalShake(delta);
        ManageRotationalShake(delta);
    }


    #region Shake Management

    private void ManageHorizontalShake(float delta)
    {
        if (_horizontalShake > 0)       { _horizontalShake -= ShakeDecreasePerSecond * delta; }
        if (_horizontalShake > 0.1f)    { Parent.OffsetH = MaxHorizontalShake * _horizontalShake * _horizontalShake * (float) GD.RandRange(-1f, 1f); }
        else                            { _horizontalShake = 0f; Parent.OffsetH = 0f; }
    }

    private void ManageVerticalShake(float delta)
    {
        if (_verticalShake > 0)         { _verticalShake -= ShakeDecreasePerSecond * delta; }
        if (_verticalShake > 0.1f)      { Parent.OffsetV = MaxVerticalShake * _verticalShake * _verticalShake * (float) GD.RandRange(-1f, 1f); }
        else                            { _verticalShake = 0f; Parent.OffsetV = 0f; }
    }

    private void ManageRotationalShake(float delta)
    {
        if (_rotationalShake > 0)       { _rotationalShake -= ShakeDecreasePerSecond * delta; }
        if (_rotationalShake >= 0.6f)   { Parent.Rotation = MaxRotationalShake * _rotationalShake * _rotationalShake * (float) GD.RandRange(-1f, 1f); }
        else                            { _rotationalShake = 0f; Parent.Rotation = 0f; }
    }

    #endregion

}
