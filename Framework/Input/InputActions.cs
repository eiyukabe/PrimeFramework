using Godot;
using System;

/// <summary> A list of constant strings to be used as action names. Use these constants when binding keys to actions and when checking for player input,
///           such as with Input.IsActionPressed(). See also Bind class. </summary>
public static partial class InputActions
{
     /* Note: for the Godot default actions it might be important to keep using snake_case in the strings to make sure they
        continue to work with some of Godot's default behavior; for everything else the strings just need to be unique. */

    /* Godot defaults */
    public static string ACCEPT = "ui_accept";
    public static string SELECT = "ui_select";
    public static string CANCEL = "ui_cancel";
    public static string FOCUS_NEXT = "ui_focus_next";
    public static string FOCUS_PREV = "ui_focus_prev";
    public static string UP = "ui_up";
    public static string DOWN = "ui_down";
    public static string LEFT = "ui_left";
    public static string RIGHT = "ui_right";
    public static string PAGE_UP = "ui_page_up";
    public static string PAGE_DOWN = "ui_page_down";
    public static string HOME = "ui_home";
    public static string END = "ui_end";

    /* Application */
    public static string TOGGLE_FULLSCREEN = "toggle_fullscreen";
    public static string QUIT = "quit";

    /* Common debug commands */
    public static string RELOAD_CURRENT_SCENE = "reload_current_scene";
    public static string TOGGLE_GOD_MODE = "toggle_god_mode";
    public static string TOGGLE_NO_CLIP = "toggle_no_clip";

    /* Camera debug controls */
    public static string ZOOM_IN_DEBUG_CAMERA = "zoom_in_debug_cam";
    public static string ZOOM_OUT_DEBUG_CAMERA = "zoom_out_debug_cam";
    public static string MOVE_DEBUG_CAMERA_UP = "move_debug_camera_up";
    public static string MOVE_DEBUG_CAMERA_DOWN = "move_debug_camera_down";
    public static string MOVE_DEBUG_CAMERA_LEFT = "move_debug_camera_left";
    public static string MOVE_DEBUG_CAMERA_RIGHT = "move_debug_camera_right";
    public static string RESET_DEBUG_CAMERA = "reset_debug_camera";

}
