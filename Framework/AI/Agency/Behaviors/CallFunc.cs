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
            ParentAgent?.Call(FunctionName);
        }

    #endregion
}
