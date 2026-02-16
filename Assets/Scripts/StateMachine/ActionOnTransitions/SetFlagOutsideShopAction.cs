    using StateMachine.Graph.Model;
using UnityEngine;

namespace StateMachine.ActionOnTransitions
{
    [CreateAssetMenu(fileName = "SetFlagOutsideShop Action",
                        menuName = "configs/StateMachine/Actions/SetFlagOutsideShop")]
    public class SetFlagOutsideShopAction : ActionOnTransitionBase
    {
        public override void DoAction(StateMachineContext context)
        {
            context.BuyerLifetimeScope.IsInsideShop.Value = false;
        }
    }
}