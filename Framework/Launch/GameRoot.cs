using Godot;
using System;

/// <summary>
/// GameRoot's purpose is to serve as a root container for the Game's top-level scenes in the scene tree, so it's children
/// can be cleared when changing scenes without deleting autoloaded nodes or any other nodes we might want to always be
/// loaded at the SceneTree root. See also the Prime class, it's the class responsible for clearing out the GameRoot when
/// changing scenes.
///
/// GameRoot should be autoloaded to work correctly.
/// <summary>
public class GameRoot : Node
{
    public override void _EnterTree()
    {
        Prime.GameRoot = this;
        GetTree().CurrentScene = this;      // This is used by Godot's ReloadCurrentScene()
    }
}
