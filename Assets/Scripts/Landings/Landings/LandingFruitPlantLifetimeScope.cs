using Inventory.ObjectInventory;
using Landings.Plants;
using Landings.Plants.PlantConfigs;
using MessagePipe;
using Messages;
using Objects;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using UI.Hover;
using UI.Hover.PopupLogics.Popups;

namespace Landings.Landings
{
    public class LandingFruitPlantLifetimeScope : LifetimeScope
    {
        [field: SerializeField] public FruitPlantConfig PlantConfig { get; private set; } = null!;

        protected override void Configure(IContainerBuilder builder)
        {
            var interactable = gameObject.AddComponent<Interactable.Interactable>();
            var hoverTrigger = gameObject.AddComponent<HoverTrigger>();

            builder.RegisterInstance(interactable);
            builder.RegisterInstance(hoverTrigger).AsSelf();
            builder.RegisterInstance(transform);
            builder.RegisterInstance(gameObject);
            builder.RegisterInstance(PlantConfig)
                   .AsSelf()
                   .As<PlantConfig>();

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
            
            builder.UseEntryPoints(ep =>
                                   {
                                       ep.Add<PlantGrowerByUpper>()
                                         .As<IGrower>()
                                         .AsSelf()
                                         .Keyed(nameof(PlantGrowerByUpper));   
                                       ep.Add<PlantGrowerByStages>()
                                         .As<IGrower>()
                                         .AsSelf()
                                         .Keyed(nameof(PlantGrowerByStages));
                                       ep.Add<FruitGrower>().AsSelf();
                                       ep.Add<FruitGiver>().AsSelf();
                                       ep.Add<LandingFruitPlantController>().AsSelf();
                                   });
            
            builder.RegisterBuildCallback(container =>
                                          {
                                              container.Inject(hoverTrigger);
                                          });
        }
    }
}