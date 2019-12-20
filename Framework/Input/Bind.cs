using Godot;
using System;

/// <summary> Contains methods for binding keys to input actions. See InputActions for a list of actions you can bind to. </summary>
public static partial class Bind
{
    /// <summary> Bind WASD keys to fire up/left/down/right input events; same as the arrow keys do by default. </summary>
    public static void BindWASDKeys()
    {
        BindKey(KeyList.W, InputActions.UP);
        BindKey(KeyList.A, InputActions.LEFT);
        BindKey(KeyList.S, InputActions.DOWN);
        BindKey(KeyList.D, InputActions.RIGHT);
    }

    /// <summary> Bind a key to fire an action. </summary>
    public static void BindKey(KeyList key, string actionName)
    {
        var inputEvent = new InputEventKey();
        inputEvent.Scancode = (int) key;
        if (!InputMap.HasAction(actionName)) { InputMap.AddAction(actionName); }
        InputMap.ActionAddEvent(actionName, inputEvent);
    }

    /// <summary> Bind a mouse button to fire an action. </summary>
    public static void BindMouseButton(ButtonList mouseButton, string actionName)
    {
        var inputEvent = new InputEventMouseButton();
        inputEvent.ButtonIndex = (int) mouseButton;
        if (!InputMap.HasAction(actionName)) { InputMap.AddAction(actionName); }
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
                    var keyEvent = (InputEventKey) inputEvent;
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
                    var mbEvent = (InputEventMouseButton) inputEvent;
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
