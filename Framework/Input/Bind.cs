using Godot;
using System;

/// <summary> Contains methods for binding keys to input actions. See InputActions for a list of actions you can bind to. </summary>
public static partial class Bind
{
    /// <summary>
    /// Bind WASD keys to fire Godot's default UI up/down/left/right actions, so WASD can be used to navigate Godot's UI controls.
    ///</summary>
    public static void BindUIWASDKeys()
    {
        BindKey(KeyList.W, InputActions.UI_UP);
        BindKey(KeyList.A, InputActions.UI_LEFT);
        BindKey(KeyList.S, InputActions.UI_DOWN);
        BindKey(KeyList.D, InputActions.UI_RIGHT);
    }

    /// <summary> Bind a key to fire an action. </summary>
    public static void BindKey(KeyList key, string actionName)
    {
        InputEventKey inputEvent = new InputEventKey();
        inputEvent.Scancode = (uint) key;
        if (!InputMap.HasAction(actionName))
        {
            InputMap.AddAction(actionName);
        }
        InputMap.ActionAddEvent(actionName, inputEvent);
    }

    /// <summary> Bind a mouse button to fire an action. </summary>
    public static void BindMouseButton(ButtonList mouseButton, string actionName)
    {
        InputEventMouseButton inputEvent = new InputEventMouseButton();
        inputEvent.ButtonIndex = (int) mouseButton;
        if (!InputMap.HasAction(actionName))
        {
            InputMap.AddAction(actionName);
        }
        InputMap.ActionAddEvent(actionName, inputEvent);
    }

    /// <summary> Unbind a key from firing an action. </summary>
    public static void UnbindKey(KeyList key, string actionName)
    {
        if (InputMap.HasAction(actionName))
        {
            foreach(InputEvent inputEvent in InputMap.GetActionList(actionName))
            {
                if (inputEvent is InputEventKey)    // It could also be an InputEventMouseButton
                {
                    InputEventKey keyEvent = (InputEventKey) inputEvent;
                    if (keyEvent.Scancode == (int) key)
                    {
                        InputMap.ActionEraseEvent(actionName, inputEvent);
                        return;
                    }
                }
            }
        }
    }

    /// <summary> Unbind a mouse button from firing an action. </summary>
    public static void UnbindMouseButton(ButtonList mouseButton, string actionName)
    {        
        if (InputMap.HasAction(actionName))
        {
            foreach(InputEvent inputEvent in InputMap.GetActionList(actionName))
            {
                if (inputEvent is InputEventMouseButton)    // It could also be an InputEventKey
                {
                    InputEventMouseButton mbEvent = (InputEventMouseButton) inputEvent;
                    if (mbEvent.ButtonIndex == (int) mouseButton)
                    {
                        InputMap.ActionEraseEvent(actionName, inputEvent);
                        return;
                    }
                }
            }
        }
    }
    
}
