using System.Collections.Generic;
using System.Linq;
using Interactable;
using Inventory;
using Inventory.Item;
using Inventory.Movers;
using Inventory.ObjectInventory;
using UI.Hover;
using UI.Hover.PopupLogics.Popups;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Shelf
{
    public class ShelfOfGoodsLifetimeScope : LifetimeScope
    {
        [field: SerializeField] public ItemConfig ItemConfig { get; private set; } = null!;

        private readonly List<GameObject> children = new();
        private readonly List<(Vector3, Quaternion)> places = new();
        private readonly List<Transform> placesForBuyer = new();

        protected override void Configure(IContainerBuilder builder)
        {
            foreach (Transform child in gameObject.transform)
            {
                children.Add(child.gameObject);
            }
            var placesParent = children.First(c =>
                                                  new string(c.name
                                                              .Where(ch => !char.IsWhiteSpace(ch))
                                                              .ToArray())
                                                     .ToUpperInvariant() ==
                                                  new string("Places"
                                                            .Where(ch => !char.IsWhiteSpace(ch))
                                                            .ToArray())
                                                     .ToUpperInvariant());
            var placesForBuyerParent = children.First(c =>
                                                          new string(c.name
                                                                      .Where(ch => !char.IsWhiteSpace(ch))
                                                                      .ToArray())
                                                             .ToUpperInvariant() ==
                                                          new string("PlacesForBuyer"
                                                                    .Where(ch => !char.IsWhiteSpace(ch))
                                                                    .ToArray())
                                                             .ToUpperInvariant());
            foreach (Transform child in placesParent.transform)
            {
                var t = child.transform;
                places.Add((t.position, t.rotation));
                Destroy(child.gameObject);
            }
            foreach (Transform child in placesForBuyerParent.transform)
            {
                placesForBuyer.Add(child.transform);
            }
            var interactable = gameObject.AddComponent<Interactable.Interactable>();
            var hoverTrigger = gameObject.AddComponent<HoverTrigger>();
            
            builder.RegisterInstance(interactable);
            builder.RegisterInstance(hoverTrigger).AsSelf();
            builder.RegisterInstance(ItemConfig).AsSelf();
            builder.RegisterInstance(transform); 
            builder.RegisterInstance(transform.position).Keyed("StartPosition"); 
            builder.RegisterInstance(places).Keyed("places"); 
            builder.RegisterInstance(placesForBuyer).Keyed("placesForBuyer"); 
            builder.RegisterInstance(places.Count).Keyed("placesCount"); 
            
            builder.Register<PlacesInventory>(Lifetime.Scoped)
                   .As<IInventory>()
                   .AsSelf();
            builder.Register<ItemTaker>(Lifetime.Scoped)
                   .AsSelf();
            builder.Register<PlacesItemMover>(Lifetime.Scoped)
                   .AsSelf();
            builder.Register<ShelfPopup>(Lifetime.Scoped)
                   .As<IObjectPopup>()
                   .AsSelf();
             builder.UseEntryPoints(ep =>
                                   {
                                       ep.Add<ItemTaker>().AsSelf();
                                       ep.Add<PlacesItemMover>().AsSelf();
                                       ep.Add<InfoAboutShelfForBuyerGenerator>().AsSelf();
                                   });
             builder.RegisterBuildCallback(container =>
                                           {
                                               container.Inject(hoverTrigger);
                                           });
        }
    }
}
