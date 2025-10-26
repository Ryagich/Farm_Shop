using Interactable;
using Inventory;
using Inventory.Movers;
using Inventory.ObjectInventory;
using VContainer;
using VContainer.Unity;

namespace Container.ObjectContainers
{
    public class DeleterLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(transform);

            var interactable = gameObject.AddComponent<Interactable.Interactable>();
            builder.RegisterInstance(interactable);
         
            builder.Register<UnlimitedInventory>(Lifetime.Scoped)
                   .As<IInventory>()
                   .AsSelf();
            builder.Register<ItemTaker>(Lifetime.Scoped)
                   .AsSelf();
            builder.Register<DeleterItemMover>(Lifetime.Scoped)
                   .AsSelf();
            builder.UseEntryPoints(ep =>
                                   {
                                       ep.Add<UnlimitedInventory>().AsSelf();
                                       ep.Add<ItemTaker>().AsSelf();
                                       ep.Add<DeleterItemMover>().AsSelf();
                                   });
        }
    }
}