using Interactable;
using Inventory;
using Inventory.ObjectInventory;
using MessagePipe;
using Messages;
using Products;
using UI.Hover;
using UI.Hover.PopupLogics.Popups;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Container.ObjectContainers
{
    public class ProductionZoneLifetimeScope : LifetimeScope
    {
        [field: SerializeField] public ProductConfig ProductConfig { get; private set; } = null!;
        [field: SerializeField] public GameObject MaterialHolderPrefab { get; private set; } = null!;
        [field: SerializeField] public Transform CreatorTransform { get; private set; } = null!;

        protected override void Configure(IContainerBuilder builder)
        {
            var interactable = gameObject.AddComponent<Interactable.Interactable>();
            var hoverTrigger = gameObject.AddComponent<HoverTrigger>();

            builder.RegisterInstance(interactable);
            builder.RegisterInstance(hoverTrigger).AsSelf();
            builder.RegisterInstance(transform);
            builder.RegisterInstance(gameObject);
            builder.RegisterInstance(ProductConfig);
            builder.RegisterInstance(MaterialHolderPrefab).Keyed("MaterialHolderPrefab");
            builder.RegisterInstance(CreatorTransform).Keyed("CreatorTransform");
            
            // === Local MessagePipe ===
            var options = builder.RegisterMessagePipe();
            builder.RegisterMessageBroker<ItemHasBeenAddedToInventory>(options);
            builder.RegisterMessageBroker<MaterialsHasBeenMovedToProduction>(options);
            builder.RegisterMessageBroker<ItemGivenFromInventory>(options);
            
            builder.Register<MaterialInventoriesController>(Lifetime.Scoped)
                   .As<IInventory>()
                   .AsSelf();   
            builder.Register<ProductionZonePopup>(Lifetime.Scoped)
                   .As<IObjectPopup>()
                   .AsSelf();
            
            builder.UseEntryPoints(ep =>
                                   {
                                       ep.Add<ProductionZoneController>().AsSelf();
                                       ep.Add<MaterialInventoriesController>().AsSelf();
                                       ep.Add<ItemTaker>().AsSelf();
                                       ep.Add<MaterialComposer>().AsSelf();
                                       ep.Add<MaterialsMoverToProduction>().AsSelf();
                                       ep.Add<ProductCreator>().AsSelf();
                                   });
            builder.RegisterBuildCallback(container =>
                                          {
                                              container.Inject(hoverTrigger);
                                          });
        }
    }
}