using StateMachine.Graph.Model;
using UnityEngine;

namespace StateMachine.Conditions
{
    [CreateAssetMenu(fileName = "PlugTransition", menuName = "configs/StateMachine/Conditions/Plug")]
    public class PlugCondition : BaseCondition
    {
        public override bool IsCondition(StateMachineContext context) => true;
    }
}