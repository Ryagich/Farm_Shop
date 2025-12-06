using StateMachine.Graph.Model;
using UnityEngine;

namespace StateMachine.Conditions
{
    [CreateAssetMenu(fileName = "LoseTargetPoint Condition", menuName = "configs/StateMachine/Conditions/LoseTargetPoint")]
    public class LostTargetPointCondition : BaseCondition
    {
        public override bool IsCondition(StateMachineContext context)
        {
            return context.TargetPoint == null || !context.TargetPoint.Target;
        }
    }
}