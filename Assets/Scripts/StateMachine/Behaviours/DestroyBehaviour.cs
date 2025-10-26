using Messages;
using StateMachine.Graph.Model;
using UnityEngine;

namespace StateMachine.Behaviours
{
    [CreateAssetMenu(fileName = "Destroy Behaviour", menuName = "configs/StateMachine/Behaviours/Destroy")]
    public class DestroyBehaviour : BaseBehaviour
    {
        public override void Enter(StateMachineContext context)
        {
            while (context.Inventory.CanGet())
            {
                var item = context.Inventory.Get();
                Destroy(item.gameObject);
            }
            context.BuyerIsOverPublisher.Publish(new BuyerIsOverMessage(context.BuyerLifetimeScope));
        }
        
        public override void Logic(StateMachineContext context) { }
        public override void Exit(StateMachineContext context) { }
    }
}