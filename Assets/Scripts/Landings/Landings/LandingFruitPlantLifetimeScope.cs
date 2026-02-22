using BuildingsAndGrid.Buildings;
using Inventory.ObjectInventory;
using Landings.Plants;
using Landings.Plants.PlantConfigs;
using MessagePipe;
using Messages;
using Objects;
using Sounds;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using UI.Hover;
using UI.Hover.PopupLogics.Popups;

namespace Landings.Landings
{
    public class LandingFruitPlantLifetimeScope : LifetimeScope
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
            builder.RegisterInstance(Center);
            building.SetContent(Center);
            
            builder.RegisterInstance(gameObject);

            // === Local MessagePipe ===
            var options = builder.RegisterMessagePipe();
            builder.RegisterMessageBroker<PlantHasGrownMessage>(options);
            builder.RegisterMessageBroker<PlantHasFinishedGrownMessage>(options);
            builder.RegisterMessageBroker<ItemGivenFromInventory>(options);

            builder.Register<FruitPlantInventory>(Lifetime.Scoped)
                   .AsSelf();
            builder.Register<LandingFruitPlantPopup>(Lifetime.Scoped)
                   .As<IObjectPopup>()
                   .AsSelf();
            
            builder.RegisterEntryPoint<PlantGrowerByUpper>()
                   .As<IGrower>()
                   .AsSelf()
                   .Keyed(nameof(PlantGrowerByUpper));   
            builder.RegisterEntryPoint<PlantGrowerByStages>()
                   .As<IGrower>()
                   .AsSelf()
                   .Keyed(nameof(PlantGrowerByStages));
            builder.RegisterEntryPoint<FruitGrower>().AsSelf();
            builder.RegisterEntryPoint<FruitGiver>().AsSelf();
            builder.RegisterEntryPoint<LandingFruitPlantController>().AsSelf();
            builder.RegisterEntryPoint<ItemGiverFromInventorySoundPlayer>().AsSelf();
            // builder.RegisterEntryPoint<InventoryDisposable>().AsSelf();
            
            builder.RegisterBuildCallback(container =>
                                          {
                                              container.Inject(hoverTrigger);
                                          });
        }
    }
}