using BuildingsAndGrid.Buildings;
using Interactable;
using Inventory;
using Inventory.Movers;
using Inventory.ObjectInventory;
using UI.Hover;
using UI.Hover.PopupLogics.Popups;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Container.ObjectContainers
{
    public class DeleterLifetimeScope : LifetimeScope
    {
        [field: SerializeField] public Transform Center { get; private set; } = null!;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(transform);

            var interactable = gameObject.AddComponent<Interactable.Interactable>();
            var hoverTrigger = gameObject.AddComponent<HoverTrigger>();
            var building = gameObject.AddComponent<Building>();
            building.SetContent(Center);

            builder.RegisterInstance(interactable).AsSelf();
            builder.RegisterInstance(hoverTrigger).AsSelf();
            builder.RegisterInstance(building);
            
            builder.Register<UnlimitedInventory>(Lifetime.Scoped)
                   .As<IInventory>()
                   .AsSelf();
            builder.Register<ItemTaker>(Lifetime.Scoped)
                   .AsSelf();
            builder.Register<DeleterItemMover>(Lifetime.Scoped)
                   .AsSelf();
            builder.Register<BuildingInteractableFlag>(Lifetime.Scoped).AsSelf();
            builder.Register<InteractableSimpleBuildingPopup>(Lifetime.Scoped).As<IObjectPopup>().AsSelf();

            builder.UseEntryPoints(ep =>
                                   {
                                       ep.Add<UnlimitedInventory>().AsSelf();
                                       ep.Add<ItemTakerWithBuildingInteractableFlag>().AsSelf();
                                       ep.Add<DeleterItemMover>().AsSelf();
                                   });
            builder.RegisterEntryPoint<InventoryDisposable>().AsSelf();
            
            builder.RegisterBuildCallback(c => { c.Inject(hoverTrigger); });
        }
    }
}