using Messages;
using StateMachine.Graph.Model;
using UnityEngine;

namespace StateMachine.Transitions
{
    [CreateAssetMenu(fileName = "ToDestroyTransition", menuName = "configs/StateMachine/Transitions/ToDestroy")]
    public class ToDestroyCondition : BaseCondition
    {
        public override bool IsCondition(StateMachineContext context)
        {
            if (context.CheckDistanceToTarget())
            {
                while (context.Inventory.CanGet())
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