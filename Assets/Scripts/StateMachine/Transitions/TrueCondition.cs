using StateMachine.Graph.Model;
using UnityEngine;

namespace StateMachine.Transitions
{
    [CreateAssetMenu(fileName = "True Condition", menuName = "configs/StateMachine/Transitions/True")]
    public class TrueCondition : BaseCondition
    {
        public override bool IsCondition(StateMachineContext context)
        {
            return true;
        }
    }
}