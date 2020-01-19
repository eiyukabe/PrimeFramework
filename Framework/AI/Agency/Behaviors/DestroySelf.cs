using Godot;
using System;

public class DestroySelf : Behavior
{
    #region Initialization

        public override void Setup()
        {
            Instantaneous = true;
            base.Setup();
        }

        public override void OnBegin()
        {
            base.OnBegin();

            ParentAgent?.QueueFree();
        }

    #endregion

}
