using System.Linq;
using StateMachine.Graph.Model;
using UnityEngine;

namespace StateMachine.ActionOnTransitions
{
    [CreateAssetMenu(fileName = "SetPositionInQueue Action", menuName = "configs/StateMachine/Actions/SetPositionInQueue")]
    public class SetPositionInQueueAction : ActionOnTransitionBase
    {
        public override void DoAction(StateMachineContext context)
        {
            var checkoutWithMinQueue = context.CheckoutsController.Checkouts.First();
            var min = checkoutWithMinQueue.ByersQueue.Buyers.Count;
            
            foreach (var checkout in context.CheckoutsController.Checkouts)
            {
                if (checkout.ByersQueue.Buyers.Count < min)
                {
                    min = checkout.ByersQueue.Buyers.Count;
                    checkoutWithMinQueue = checkout;
                }
            }
            context.CheckoutController = checkoutWithMinQueue;
            context.QueueIndex = checkoutWithMinQueue.ByersQueue.BuyersCount;
            var pos = checkoutWithMinQueue.ByersQueue.GetBuyerPosition(context.QueueIndex);
            context.TargetPoint = pos.Item1;
            context.TP = context.TargetPoint.Target.position;
            pos.Item2 = false;
            checkoutWithMinQueue.ByersQueue.Buyers.Add(context);
            checkoutWithMinQueue.ByersQueue.BuyersCount++;
        }
    }
}