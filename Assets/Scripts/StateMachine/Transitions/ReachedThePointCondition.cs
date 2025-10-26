using StateMachine.Graph.Model;
using UnityEngine;

namespace StateMachine.Transitions
{
    [CreateAssetMenu(fileName = "ReachedThePoint Condition", menuName = "configs/StateMachine/Transitions/ReachedThePoint")]
    public class ReachedThePointCondition : BaseCondition
    {
        public override bool IsCondition(StateMachineContext context)
            => context.CheckDistanceToTarget();
    }
}