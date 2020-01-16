using Godot;
using System;

public class SequentialBehavior : Behavior
{
    [Export] private bool Loop = false;
    [Export] private int LoopCount = -1; /// <summary> Only used if Loop is set to true. </summary> 
    [Export] private bool Persistent = false; /// <summary> If true, then this sequence will pick up where it left off when it is resumed. </summary> 
    [Export] private bool Atomic = false; /// <summary> If true, then this sequence will finish executing all of its children even if interrupted. </summary> 

    private int CurrentBehaviorIndex = -1;
    private Behavior CurrentBehavior = null;

    private bool FinishingAtomicExecution = false;
    private bool AtomicExecutionFinished = false;

    private int CurrentLoopCount = 0;

    /// Used to track how many children we have looped through this tick. If greater than our child count, we will cancel our loop.
    private int ExecuteNextChildCallDepth = 0; 


    #region Initialization

        public override void OnBegin()
        {
            ExecuteNextChildCallDepth = 0;

            if (!FinishingAtomicExecution)
            {
                if (Persistent)
                {
                    if (CurrentBehaviorIndex == -1)
                    {
                        ExecuteNextChild();
                    }
                }
                else
                {
                    if (ChildBehaviors.Count > 0)
                    {
                        CurrentBehaviorIndex = -1;
                        ExecuteNextChild();
                    }
                }
                CurrentLoopCount = 0;
            }
            else
            {
                FinishingAtomicExecution = false;
                AtomicExecutionFinished = false;
            }
        }

    #endregion


    #region Control

        public override void Process(float delta)
        {
            base.Process(delta);
            CurrentBehavior?.Process(delta);

            if (FinishingAtomicExecution && AtomicExecutionFinished)
            {
                Stop();
            }
        }

        public override void InactiveProcess(float delta)
        {
            base.InactiveProcess(delta);
            if (FinishingAtomicExecution)
            {
                Process(delta);
            }
        }

        protected override void StopSelf()
        {
            if (Atomic && AtomicExecutionFinished)
            {
                FinishingAtomicExecution = true;
            }
            else
            {
                FinishingAtomicExecution = false;
            }
        }

        private void ExecuteNextChild()
        {
            ExecuteNextChildCallDepth++;
            if (ExecuteNextChildCallDepth > ChildBehaviors.Count)
            {
                StopSelf();
                return; // This can happen if all children are disabled. Don't want an infinite loop.
            }

            if (CurrentBehaviorIndex < ChildBehaviors.Count)
            {
                AtomicExecutionFinished = true;
            }
            else
            {
                if (Loop && (CurrentLoopCount < LoopCount - 1 || LoopCount < 0))
                {
                    // This is a looping behavior. Roll around to the first behavior.
                    CurrentBehaviorIndex = 0;
                    CurrentLoopCount++;
                }
                else
                {
                    // This is not a looping behavior. End after the last behavior has ended.
                    StopSelf();
                    ExecuteNextChildCallDepth--;
                    return;
                }
            }

            if (CurrentBehaviorIndex >= 0 && CurrentBehaviorIndex < ChildBehaviors.Count)
            {
                CurrentBehavior = ChildBehaviors[CurrentBehaviorIndex];
                if (CurrentBehavior != null)
                {
                    if (CurrentBehavior.Disabled /*|| CurrentBehavior is Condition*/)
                    {
                        ExecuteNextChild();
                    }
                    else
                    {
                        CurrentBehavior.Begin();
                    }
                }
            }

            ExecuteNextChildCallDepth--;
        }

        protected override void OnChildStop(Behavior child)
        {
            ExecuteNextChild();
        }

    #endregion

    
}
