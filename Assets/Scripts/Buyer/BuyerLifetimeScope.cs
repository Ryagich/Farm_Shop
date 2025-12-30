using Inventory;
using Inventory.Movers;
using Inventory.ObjectInventory;
using StateMachine.Graph.Model;
using UI.Hover;
using UI.Hover.PopupLogics.Popups;
using UniRx;
using UnityEngine;
using UnityEngine.AI;
using VContainer;
using VContainer.Unity;

namespace Buyer
{
    public class BuyerLifetimeScope : LifetimeScope
    {
        [field: SerializeField] public State CurrentState = null!;
        [field: SerializeField] public Vector3 TP;
        [field: SerializeField] public float DistanceToTarget;
        [field: SerializeField] public int QueueIndex;
        [field: SerializeField] public Transform targetPoint;
        [field: SerializeField] public ReactiveProperty<bool> IsInsideShop;

        [SerializeField] private Animator animator = null!;

        protected override void Configure(IContainerBuilder builder)
        {
            var hoverTrigger = gameObject.AddComponent<HoverTrigger>();
            var hand = transform.Find("Hand");
            var agent = GetComponent<NavMeshAgent>();
            var buyerLook = GetComponent<BuyerLookController>();
            
            agent.avoidancePriority = Random.Range(0, 99);
            builder.RegisterInstance(buyerLook).AsSelf();
            builder.RegisterInstance(hoverTrigger).AsSelf();
            builder.RegisterInstance(agent).AsSelf();
            builder.RegisterInstance(animator).AsSelf();
            builder.RegisterInstance(hand).As<Transform>().Keyed("Hand"); 

            builder.Register<SimpleInventory>(Lifetime.Scoped).As<IInventory>().AsSelf();
            builder.Register<BuyerPopup>(Lifetime.Scoped)
                   .As<IObjectPopup>()
                   .AsSelf(); 
            // builder.Register<BuyerController>(Lifetime.Scoped).AsSelf();
            builder.RegisterEntryPoint<BuyerController>().AsSelf();
            builder.UseEntryPoints(ep =>
                                   {
                                       // ep.Add<BuyerController>();
                                       ep.Add<NPCHandItemMover>();
                                       // ep.Add<BuyerAnimationController>();
                                   }); 
            builder.RegisterBuildCallback(container =>
                                          {
                                              container.Inject(hoverTrigger);
                                          });
            
        }
    }
}