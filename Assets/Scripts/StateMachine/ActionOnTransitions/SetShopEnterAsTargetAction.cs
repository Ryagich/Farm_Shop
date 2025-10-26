using StateMachine.Graph.Model;
using UnityEngine;

namespace StateMachine.ActionOnTransitions
{
    [CreateAssetMenu(fileName = "SetShopEnterAsTarget Action", menuName = "configs/StateMachine/Actions/SetShopEnterAsTarget")]
    public class SetShopEnterAsTargetAction : ActionOnTransitionBase
    {
        public override void DoAction(StateMachineContext context)
        {
            context.TargetPosition = context.ShoppingEnter.position;
            context.SetLongDistanceToTarget();
        }
    }
}