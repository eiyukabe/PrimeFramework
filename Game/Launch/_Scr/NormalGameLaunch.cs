using Godot;
using System;

/// <summary>
/// NormalGameLaunch is for initialization code that only runs when the game is launched normally or by using f5.
/// See also GlobalGameLaunch.
/// Note: GlobalGameLaunch runs before NormalGameLaunch because GlobalGameLaunch is autoloaded.
/// </summary>
public class NormalGameLaunch : Node
{
   public override void _Ready()
   {
       /* Init game */
       CallDeferred("GoToFirstScene");
       QueueFree();
   }

   private void GoToFirstScene()
   {
       // Prime.SetScene(Scenes.TITLE_SCREEN);
       Prime.SetScene(Scenes.PLAY);
   }
}
