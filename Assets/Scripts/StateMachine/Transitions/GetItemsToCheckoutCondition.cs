using StateMachine.Graph.Model;
using UnityEngine;

namespace StateMachine.Transitions
{
    [CreateAssetMenu(fileName = "GetItemsToCheckout Condition", menuName = "configs/StateMachine/Conditions/GetItemsToCheckout")]
    public class GetItemsToCheckoutCondition : BaseCondition
    {
        public override bool IsCondition(StateMachineContext context)
            => context.QueueIndex is 0 && Vector3.Distance(context.NavMeshAgent.transform.position,
                                                           context.CheckoutController.ByersQueue.GetBuyerPosition(0)) < context.DistanceToTarget;
    }
}