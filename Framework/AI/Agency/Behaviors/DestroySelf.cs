using Godot;
using System;

public class DestroySelf : Behavior
{
    [Export] private float WaitDuration = 1.0f;

    private float WaitTimer = 0.0f;


    #region Initialization

        public override void Setup()
        {
            Instantaneous = true;
            base.Setup();
        }

        public override void OnBegin()
        {
            base.OnBegin();

            Node2D Agent = GetAgent();
            Agent?.QueueFree();
        }

    #endregion

}
