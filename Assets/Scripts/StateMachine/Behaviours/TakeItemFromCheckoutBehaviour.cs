using StateMachine.Graph.Model;
using UnityEngine;

namespace StateMachine.Behaviours
{
    [CreateAssetMenu(fileName = "TakeItemFromCheckout Behaviour", menuName = "configs/StateMachine/Behaviours/TakeItemFromCheckout")]
    public class TakeItemFromCheckoutBehaviour : BaseBehaviour
    {
        public override void Enter(StateMachineContext context)
        {
            SetDefaultParameters(context);
        }

        public override void Logic(StateMachineContext context)
        {
            if (context.TimeBetweenIterations > context.T)
            {
                context.T += context.DeltaTime;
                return;
            }
            if (context.CheckoutController.CanGet())
            {
                var itemHolder = context.CheckoutController.Get();
                context.Inventory.Add(itemHolder.Config, itemHolder.transform.localToWorldMatrix);
                Destroy(itemHolder.gameObject);
                SetDefaultParameters(context);
            }
        }

        public override void Exit(StateMachineContext context)
        {
            SetDefaultParameters(context);
        }
        
        private void SetDefaultParameters(StateMachineContext context)
        {
            context.TimeBetweenIterations = Random.Range(context.BuyerSettings.FastTimeBetweenInteraction.x,
                                                         context.BuyerSettings.FastTimeBetweenInteraction.y);
            context.T = .0f;
        }
    }
}