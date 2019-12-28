using Godot;
using System;

/// <summary>
/// A list of constant strings to be used as action names. Use these constants when binding keys to actions and when checking
/// for player input, such as with Input.IsActionPressed(). See also Bind class.
/// </summary>
public static partial class InputActions
{
     /* Note: for the Godot default actions it might be important to keep using snake_case in the strings to make sure they
        continue to work with some of Godot's default behavior; for everything else the strings just need to be unique. */

    /* Godot defaults */
    public const string UI_ACCEPT = "ui_accept";
    public const string UI_SELECT = "ui_select";
    public const string UI_CANCEL = "ui_cancel";
    public const string UI_FOCUS_NEXT = "ui_focus_next";
    public const string UI_FOCUS_PREV = "ui_focus_prev";
    public const string UI_UP = "ui_up";
    public const string UI_DOWN = "ui_down";
    public const string UI_LEFT = "ui_left";
    public const string UI_RIGHT = "ui_right";
    public const string UI_PAGE_UP = "ui_page_up";
    public const string UI_PAGE_DOWN = "ui_page_down";
    public const string UI_HOME = "ui_home";
    public const string UI_END = "ui_end";

    /* Application */
    public const string TOGGLE_FULLSCREEN = "toggle_fullscreen";
    public const string QUIT = "quit";

    /* Common debug commands */
    public const string RELOAD_CURRENT_SCENE = "reload_current_scene";
    public const string TOGGLE_GOD_MODE = "toggle_god_mode";
    public const string TOGGLE_NO_CLIP = "toggle_no_clip";
    public const string PRINT_SCENE_STACK = "print_scene_stack";

    /* Camera debug controls */
    public const string ZOOM_IN_DEBUG_CAMERA = "zoom_in_debug_cam";
    public const string ZOOM_OUT_DEBUG_CAMERA = "zoom_out_debug_cam";
    public const string MOVE_DEBUG_CAMERA_UP = "move_debug_camera_up";
    public const string MOVE_DEBUG_CAMERA_DOWN = "move_debug_camera_down";
    public const string MOVE_DEBUG_CAMERA_LEFT = "move_debug_camera_left";
    public const string MOVE_DEBUG_CAMERA_RIGHT = "move_debug_camera_right";
    public const string RESET_DEBUG_CAMERA = "reset_debug_camera";

}
