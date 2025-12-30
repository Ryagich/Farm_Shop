using System.Linq;
using StateMachine.Graph.Model;
using UnityEngine;

namespace StateMachine.Conditions
{
    [CreateAssetMenu(fileName = "GoAwayOfLongQueue Condition", menuName = "configs/StateMachine/Conditions/GoAwayOfLongQueue")]
    public class GoAwayOfLongQueueCondition : BaseCondition
    {
        public override bool IsCondition(StateMachineContext context)
        {
            var checkouts = context.CheckoutsController.Checkouts.Where(c => c.IsInteractable).ToList();
            if (checkouts.Count is 0)
                return true;
            var checkoutWithMinQueue = context.CheckoutsController.Checkouts.First(c => c.IsInteractable);
            var min = checkoutWithMinQueue.ByersQueue.Buyers.Count;
            
            foreach (var checkout in checkouts)
            {
                if (checkout.ByersQueue.Buyers.Count < min)
                {
                    min = checkout.ByersQueue.Buyers.Count;
                    checkoutWithMinQueue = checkout;
                }
            }

            var extraBuyers = Mathf.Clamp(checkoutWithMinQueue.ByersQueue.Buyers.Count 
                                        - context.BuyerSettings.CriticalCountOfQueue, 
                                          0, 
                                          checkoutWithMinQueue.ByersQueue.Buyers.Count);
            var chanceToLeave = extraBuyers * context.BuyerSettings.ChanceToLeaveForExtraBuyer;
            var result = Random.Range(.0f, 1.0f) <= chanceToLeave;

            if (checkoutWithMinQueue.ByersQueue.Buyers.Count 
             >= checkoutWithMinQueue.ByersQueue.queuePoints.Count)
            {
                return false;
            }
            
            return result;
        }
    }
}