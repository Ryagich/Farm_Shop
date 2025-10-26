using System.Linq;
using StateMachine.Graph.Model;
using UnityEngine;

namespace StateMachine.Behaviours
{
    [CreateAssetMenu(fileName = "Pay Behaviour", menuName = "configs/StateMachine/Behaviours/Pay")]
    public class PayBehaviour : BaseBehaviour
    {
        public override void Enter(StateMachineContext context)
        {
            foreach (var position in context.BuyPositions)
            {
                for (var i = 0; i < position.Count; i++)
                {
                    context.Costs.Add(position.Config.Price);
                }
            }
            SetDefaultParameters(context);
        }

        public override void Logic(StateMachineContext context)
        {
            if (context.TimeBetweenIterations > context.T 
             || context.Costs.Count <= 0)
            {
                context.T += context.DeltaTime;
                return;
            }
            if (context.CheckoutController.CanPay)
            {
                var cost = context.Costs.First();
                context.Costs.Remove(cost);
                context.CheckoutController.MoneyTaker.Purchase(context.Hand.localToWorldMatrix, cost);
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