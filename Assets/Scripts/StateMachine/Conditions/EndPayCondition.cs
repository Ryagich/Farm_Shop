using StateMachine.Graph.Model;
using UnityEngine;

namespace StateMachine.Transitions
{
    [CreateAssetMenu(fileName = "EndPay Condition",
                     menuName = "configs/StateMachine/Conditions/EndPay")]
    public class EndPayCondition : BaseCondition
    {
        public override bool IsCondition(StateMachineContext context)
        {
            return context.Costs.Count <= 0;
        }
    }
}