using UnityEngine;
using UnityEngine.AI;
using VContainer.Unity;

namespace Buyer
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class BuyerAnimationController : IFixedTickable
    {
        private readonly BuyerSettings buyerSettings;
        private readonly Animator animator;
        private readonly NavMeshAgent agent;

        public BuyerAnimationController
            (
                BuyerSettings buyerSettings,
                Animator animator,
                NavMeshAgent agent
            )
        {
            this.buyerSettings = buyerSettings;
            this.animator = animator;
            this.agent = agent;
        }

        public void FixedTick()
        {
            animator.SetBool(buyerSettings.MovingName, agent.velocity is not {x: 0, y: 0});
        }
    }
}