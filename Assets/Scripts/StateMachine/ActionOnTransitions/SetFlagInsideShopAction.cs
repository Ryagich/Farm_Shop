using StateMachine.Graph.Model;
using UnityEngine;

namespace StateMachine.ActionOnTransitions
{
    [CreateAssetMenu(fileName = "SetFlagInsideShop Action",
                     menuName = "configs/StateMachine/Actions/SetFlagInsideShop")]
    public class SetFlagInsideShopAction : ActionOnTransitionBase
    {
        public override void DoAction(StateMachineContext context)
        {
            context.BuyerLifetimeScope.IsInsideShop.Value = true;
        }
    }
}