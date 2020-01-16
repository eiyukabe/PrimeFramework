using Godot;
using System;

public class CallFunc : Behavior
{
    [Export] private String FunctionName = "";


    #region Initialization

        public override void Setup()
        {
            base.Setup();
            Instantaneous = true;
        }

        public override void OnBegin()
        {
            base.OnBegin();

            Node2D Agent = GetAgent();
            if (Agent != null)
            {
                Agent.Call(FunctionName);
            }
        }

    #endregion
}
