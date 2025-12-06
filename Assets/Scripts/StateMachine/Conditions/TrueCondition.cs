using StateMachine.Graph.Model;
using UnityEngine;

namespace StateMachine.Conditions
{
    [CreateAssetMenu(fileName = "True Condition", menuName = "configs/StateMachine/Conditions/True")]
    public class TrueCondition : BaseCondition
    {
        public override bool IsCondition(StateMachineContext context)
        {
            return true;
        }
    }
}