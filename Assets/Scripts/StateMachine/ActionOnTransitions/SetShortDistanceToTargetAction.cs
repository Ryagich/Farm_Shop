using StateMachine.Graph.Model;
using UnityEngine;

namespace StateMachine.ActionOnTransitions
{
    [CreateAssetMenu(fileName = "SetShortDistanceToTarget Action", menuName = "configs/StateMachine/Actions/SetShortDistanceToTarget")]
    public class SetShortDistanceToTargetAction : ActionOnTransitionBase
    {
        public override void DoAction(StateMachineContext context)
        {
            context.SetShortDistanceToTarget();
        }
    }
}