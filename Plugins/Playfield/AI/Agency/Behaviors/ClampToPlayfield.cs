using Godot;
using System;

// Behavior for clamping an agent to the playfield. Can have offsets on all sides to make the clamping stricter or looser.
public class ClampToPlayfield : Behavior
{

    [Export] public float LeftEdgeOffset = 0;
    [Export] public float RightEdgeOffset = 0;
    [Export] public float TopEdgeOffset = 0;
    [Export] public float BottomEdgeOffset = 0;


    #region Control

        public override void _Process(float delta)
        {
            base._Process(delta);

            Node2D Agent = GetAgent();
            if (Agent != null)
            {
                float LeftEdge = Game.PlayfieldX + LeftEdgeOffset;
                float RightEdge = LeftEdge + Game.PlayfieldWidth + RightEdgeOffset;
                float TopEdge = Game.PlayfieldY + TopEdgeOffset;
                float BottomEdge = TopEdge + Game.PlayfieldHeight + BottomEdgeOffset;

                float AgentX = Mathf.Clamp(Agent.GlobalPosition.x, LeftEdge, RightEdge);
                float AgentY = Mathf.Clamp(Agent.GlobalPosition.y, TopEdge, BottomEdge);

                Agent.SetGlobalPosition(new Vector2(AgentX, AgentY));
            }
        }

    #endregion

}
