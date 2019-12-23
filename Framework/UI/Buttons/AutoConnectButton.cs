using Godot;
using System;

/// <summary>
/// This button is meant to optimize and aid connecting a UI Button's signals to a callback in the scene's root script, depending on the
/// export bools that are set. This button expects the format "On[ButtonName]Button[SignalName]" to be used in the scene's root script.
///
/// For example, if you create a button called "TitleScreen" and set "OnPressed = true", when this button enters the scene tree it will try
/// to connect its "OnPressed" signal to a method in the scene's root script called "OnTitleScreenButtonPressed()". Be sure the button's
/// name in the scene tree does not have spaces in it; the corresponding method name obviously cannot have spaces and the names must match.
///
/// This button helps reduce boilerplate code because you no longer need to write GetNode("buttonPath") and Button.Connect() for every
/// button in a scene.
///
/// Note: this button relies on its Owner reference being set. If you instantiate an AutoConnectButton in code it will not work unless you
/// set the Owner manually. If you place the button in the scene in the editor everything should work normally.
/// </summary>
public class AutoConnectButton : Button
{
    [Export] public bool OnPressed = true;      // Connect OnPressed signal to On[ButtonName]ButtonPressed() method in the scene's root script
    [Export] public bool OnButtonDown = false;  // Connect OnButtonDown signal to On[ButtonName]ButtonDown() method in the scene's root script
    [Export] public bool OnButtonUp = false;    // Connect OnButtonUp signal to On[ButtonName]ButtonUp() method in the scene's root script
    [Export] public bool OnToggled = false;     // Connect OnToggled signal to On[ButtonName]ButtonToggled() method in the scene's root script

    public override void _EnterTree()
    {
        if (Owner == null)  { return; }
        if (OnPressed    && Owner.HasMethod($"On{Name}ButtonPressed"))  { Connect("pressed", Owner, $"On{Name}ButtonPressed"); }
        if (OnButtonDown && Owner.HasMethod($"On{Name}ButtonDown"))     { Connect("button_down", Owner, $"On{Name}ButtonDown"); }
        if (OnButtonUp   && Owner.HasMethod($"On{Name}ButtonUp"))       { Connect("button_up", Owner, $"On{Name}ButtonUp"); }
        if (OnToggled    && Owner.HasMethod($"On{Name}ButtonToggled"))  { Connect("toggled", Owner, $"On{Name}ButtonToggled"); }    // Requires one bool parameter about toggled state
    }

}
