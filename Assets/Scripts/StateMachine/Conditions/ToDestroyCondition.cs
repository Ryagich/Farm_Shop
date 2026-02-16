using Messages;
using StateMachine.Graph.Model;
using UnityEngine;

namespace StateMachine.Conditions
{
    [CreateAssetMenu(fileName = "ToDestroy Condition", menuName = "configs/StateMachine/Conditions/ToDestroy")]
    public class ToDestroyCondition : BaseCondition
    {
        public override bool IsCondition(StateMachineContext context)
        {
            if (context.CheckDistanceToTarget())
            {
                while (context.Inventory.HaveItem)
                {
                    var item = context.Inventory.Get();
                    Destroy(item.gameObject);
                }
                context.BuyerIsOverPublisher.Publish(new BuyerIsOverMessage(context.BuyerLifetimeScope));
            }
            return false;
        }
    }
}