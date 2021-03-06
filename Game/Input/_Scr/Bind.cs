using Godot;
using System;

/// <summary> Contains methods for binding keys to input actions. See InputActions for a list of actions you can bind to. </summary>
public static partial class Bind
{
    /// <summary> Bind common debug keys. </summary>
    public static void BindCommonDebugKeys()
    {
        BindKey(KeyList.F1, InputActions.RELOAD_SCENE);
        BindKey(KeyList.F2, InputActions.TOGGLE_GOD_MODE);
        BindKey(KeyList.F3, InputActions.TOGGLE_NO_CLIP);
        BindKey(KeyList.Slash, InputActions.PRINT_SCENE_STACK);
    }

    /// <summary> Bind camera debug keys. A debug camera must be in the scene for this to do anything. </summary>
    public static void BindCameraDebugKeys()
    {
        BindKey(KeyList.KpAdd, InputActions.ZOOM_IN_DEBUG_CAMERA);
        BindKey(KeyList.KpSubtract, InputActions.ZOOM_OUT_DEBUG_CAMERA);
        BindKey(KeyList.Kp8, InputActions.MOVE_DEBUG_CAMERA_UP);
        BindKey(KeyList.Kp2, InputActions.MOVE_DEBUG_CAMERA_DOWN);
        BindKey(KeyList.Kp4, InputActions.MOVE_DEBUG_CAMERA_LEFT);
        BindKey(KeyList.Kp6, InputActions.MOVE_DEBUG_CAMERA_RIGHT);
        BindKey(KeyList.Kp0, InputActions.RESET_DEBUG_CAMERA);
    }
}
