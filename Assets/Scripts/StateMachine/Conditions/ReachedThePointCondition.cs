using StateMachine.Graph.Model;
using UnityEngine;

namespace StateMachine.Conditions
{
    [CreateAssetMenu(fileName = "ReachedThePoint Condition", menuName = "configs/StateMachine/Conditions/ReachedThePoint")]
    public class ReachedThePointCondition : BaseCondition
    {
        public override bool IsCondition(StateMachineContext context)
            => context.CheckDistanceToTarget();
    }
}