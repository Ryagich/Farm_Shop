using Inventory;
using Inventory.ObjectInventory;
using Landings.Plants;
using Landings.Plants.PlantConfigs;
using MessagePipe;
using Messages;
using Objects;
using UI.Hover;
using UI.Hover.PopupLogics.Popups;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Landings.Landings
{
    public class LandingPlantIsItemLifetimeScope : LifetimeScope
    {
        [field: SerializeField] public PlantConfig PlantConfig { get; private set; } = null!;

        protected override void Configure(IContainerBuilder builder)
        {
            var interactable = gameObject.AddComponent<Interactable.Interactable>();
            var hoverTrigger = gameObject.AddComponent<HoverTrigger>();

            builder.RegisterInstance(interactable);
            builder.RegisterInstance(hoverTrigger).AsSelf();
            builder.RegisterInstance(transform);
            builder.RegisterInstance(gameObject);
            builder.RegisterInstance(PlantConfig);

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
                                        ep.Add<ItemGiverFromInventory>().AsSelf();
                                        ep.Add<LandingPlantIsItemController>().AsSelf();
                                   });
            
            builder.RegisterBuildCallback(container =>
                                          {
                                                 container.Inject(hoverTrigger);
                                          });
        }
    }
}