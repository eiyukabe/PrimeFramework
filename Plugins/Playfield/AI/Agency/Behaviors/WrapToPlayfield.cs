using Godot;
using System;

// Behavior for wrapping an agent to the playfield. Can have offsets on all sides to make the clamping stricter or looser.
public class WrapToPlayfield : Behavior
{

    [Export] public float LeftEdgeOffset = 0;
    [Export] public float RightEdgeOffset = 0;
    [Export] public float TopEdgeOffset = 0;
    [Export] public float BottomEdgeOffset = 0;


    #region Control

        public override void _Process(float delta)
        {
            base._Process(delta);

            if (ParentAgent != null)
            {
                float LeftEdge = Game.PlayfieldX + LeftEdgeOffset;
                float RightEdge = LeftEdge + Game.PlayfieldWidth + RightEdgeOffset;
                float TopEdge = Game.PlayfieldY + TopEdgeOffset;
                float BottomEdge = TopEdge + Game.PlayfieldHeight + BottomEdgeOffset;

                float AgentX = ParentAgent.GlobalPosition.x;
                float AgentY = ParentAgent.GlobalPosition.y;

                float WrapWidth = Game.PlayfieldWidth + RightEdgeOffset - LeftEdgeOffset;
                float WrapHeight = Game.PlayfieldWidth + RightEdgeOffset - LeftEdgeOffset;

                // Horizontal Wrapping
                while (AgentX < LeftEdge) { AgentX += WrapWidth; }
                while (AgentX >= RightEdge) { AgentX -= WrapWidth; }

                // Vertical Wrapping
                while (AgentY < TopEdge) { AgentY += WrapHeight; }
                while (AgentY >= BottomEdge) { AgentY += WrapHeight; }

                ParentAgent.GlobalPosition = new Vector2(AgentX, AgentY);
            }
        }

    #endregion

}
