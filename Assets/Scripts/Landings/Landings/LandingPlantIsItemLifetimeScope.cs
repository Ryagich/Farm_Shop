using BuildingsAndGrid.Buildings;
using Inventory;
using Inventory.ObjectInventory;
using Landings.Plants;
using MessagePipe;
using Messages;
using Objects;
using Sounds;
using UI.Hover;
using UI.Hover.PopupLogics.Popups;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Landings.Landings
{
    public class LandingPlantIsItemLifetimeScope : LifetimeScope
    {
        [field: SerializeField] public Transform Center { get; private set; } = null!;

        protected override void Configure(IContainerBuilder builder)
        {
            var interactable = gameObject.AddComponent<Interactable.Interactable>();
            var hoverTrigger = gameObject.AddComponent<HoverTrigger>();
            var building = gameObject.AddComponent<Building>();
            
            builder.RegisterInstance(interactable);
            builder.RegisterInstance(hoverTrigger);           
            builder.RegisterInstance(building);
            builder.RegisterInstance(gameObject);
            builder.RegisterInstance(Center);
            building.SetContent(Center);

            // === Local MessagePipe ===
            var options = builder.RegisterMessagePipe();
            builder.RegisterMessageBroker<PlantHasGrownMessage>(options);
            builder.RegisterMessageBroker<ItemGivenFromInventory>(options);
            
            builder.Register<SimpleInventory>(Lifetime.Scoped)
                   .As<IInventory>() 
                   .AsSelf();
            builder.Register<ItemGiverFromInventory>(Lifetime.Scoped)
                   .AsSelf();   
            builder.Register<LandingPlantIsItemPopup>(Lifetime.Scoped)
                   .As<IObjectPopup>();
            
            builder.RegisterEntryPoint<PlantGrowerByUpper>()
                   .As<IGrower>()
                   .AsSelf()
                   .Keyed(nameof(PlantGrowerByUpper));
            builder.RegisterEntryPoint<PlantGrowerByStages>()
                   .As<IGrower>()
                   .AsSelf()
                   .Keyed(nameof(PlantGrowerByStages));
            builder.RegisterEntryPoint<LandingPlantIsItemController>().AsSelf();
            
            builder.RegisterEntryPoint<ItemGiverFromInventory>().AsSelf();
            builder.RegisterEntryPoint<ItemGiverFromInventorySoundPlayer>().AsSelf();
            builder.RegisterEntryPoint<InventoryDisposable>().AsSelf();
            
            builder.RegisterBuildCallback(container =>
                                          {
                                                 container.Inject(hoverTrigger);
                                          });
        }
    }
}