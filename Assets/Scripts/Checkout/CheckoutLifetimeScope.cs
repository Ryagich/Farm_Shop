using Inventory;
using Inventory.Movers;
using Inventory.ObjectInventory;
using UI.Hover;
using UI.Hover.PopupLogics.Popups;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Checkout
{
    public class CheckoutLifetimeScope : LifetimeScope
    {
        [field: SerializeField] private Transform rawHand = null!;
        [field: SerializeField] private Transform completeHand = null!;
        [field: SerializeField] private Transform queuePoint = null!;
        [field: SerializeField] private Transform placeForMoney = null!;

        protected override void Configure(IContainerBuilder builder)
        {
            var interactable = gameObject.AddComponent<Interactable.Interactable>();
            var hoverTrigger = gameObject.AddComponent<HoverTrigger>();

            builder.RegisterInstance(interactable);
            builder.RegisterInstance(hoverTrigger).AsSelf();
            builder.RegisterInstance(rawHand).Keyed("RawHand"); 
            builder.RegisterInstance(completeHand).Keyed("CompleteHand");
            builder.RegisterInstance(queuePoint).Keyed("QueuePoint");
            builder.RegisterInstance(placeForMoney).Keyed("PlaceForMoney");

            builder.Register<SimpleInventory>(Lifetime.Scoped).As<IInventory>().Keyed("RawInventory");
            builder.Register<SimpleInventory>(Lifetime.Scoped).As<IInventory>().Keyed("CompleteInventory");
            builder.Register<ByersQueue>(Lifetime.Scoped);
            builder.Register<MoneyTaker>(Lifetime.Scoped);
            builder.Register<CheckoutPopup>(Lifetime.Scoped)
                   .As<IObjectPopup>()
                   .AsSelf();
            builder.UseEntryPoints(ep =>
                                   {
                                       ep.Add<CheckoutController>();
                                       ep.Add<MoneyTaker>().AsSelf();
                                       ep.Add<StackItemsMover>()
                                         .WithParameter(ctx => ctx.Resolve<IInventory>("RawInventory"))
                                         .WithParameter(ctx => ctx.Resolve<Transform>("RawHand"))
                                         .Keyed("RawItemsMover");
                                       ep.Add<StackItemsMover>()
                                         .WithParameter(ctx => ctx.Resolve<IInventory>("CompleteInventory"))
                                         .WithParameter(ctx => ctx.Resolve<Transform>("CompleteHand"))
                                         .Keyed("CompleteItemsMover");
                                   });
            builder.RegisterBuildCallback(container =>
                                          {
                                              container.Inject(hoverTrigger);
                                          });
        }
    }
}