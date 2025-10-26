using StateMachine.Graph.Model;
using UnityEngine;

namespace StateMachine.Transitions
{
    [CreateAssetMenu(fileName = "TakeAllItemsFromCheckout Transition",
                     menuName = "configs/StateMachine/Transitions/TakeAllItemsFromCheckout")]
    public class TakeAllItemsFromCheckoutCondition : BaseCondition
    {
            public override bool IsCondition(StateMachineContext context)
            {
                if (!context.CheckoutController.CanGet())
                {
                    context.CheckoutController.ByersQueue.Buyers.Remove(context);
                    foreach (var buyer in context.CheckoutController.ByersQueue.Buyers)
                    {
                        buyer.QueueIndex--;
                    }
                    context.SetLongDistanceToTarget();
                    context.TargetPosition = context.SpawnPointsForBuyers[Random.Range(0, context.SpawnPointsForBuyers.Count - 1)].position;
                }
                return !context.CheckoutController.CanGet();
            }
    }
}