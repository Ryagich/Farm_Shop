using StateMachine.Graph.Model;
using UnityEngine;

namespace StateMachine.Conditions
{
    [CreateAssetMenu(fileName = "TakeAllItemsFromCheckout Condition",
                     menuName = "configs/StateMachine/Conditions/TakeAllItemsFromCheckout")]
    public class TakeAllItemsFromCheckoutCondition : BaseCondition
    {
            public override bool IsCondition(StateMachineContext context)
            {
                if (!context.CheckoutController.CanGet())
                {
                    context.CheckoutController.ByersQueue.Buyers.Remove(context);
                    context.CheckoutController.ByersQueue.BuyersCount--;
                    foreach (var buyer in context.CheckoutController.ByersQueue.Buyers)
                    {
                        buyer.QueueIndex--;
                    }
                    context.SetLongDistanceToTarget();
                    context.TargetPoint = context.BuyerSpawnPoints.SpawnPoints[Random.Range(0, context.BuyerSpawnPoints.SpawnPoints.Count - 1)];
                    context.TP = context.TargetPoint.Target.position;
                }
                return !context.CheckoutController.CanGet();
            }
    }
}