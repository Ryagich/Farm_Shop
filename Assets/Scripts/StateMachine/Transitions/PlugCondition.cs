using StateMachine.Graph.Model;
using UnityEngine;

namespace StateMachine.Transitions
{
    [CreateAssetMenu(fileName = "PlugTransition", menuName = "configs/StateMachine/Transitions/Plug")]
    public class PlugCondition : BaseCondition
    {
        public override bool IsCondition(StateMachineContext context) => true;
    }
}