using System.Collections.Generic;
using System.Linq;
using BuildingsAndGrid.Buildings;
using Interactable;
using Inventory;
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
        [field: SerializeField] public Transform Center { get; private set; }
        [field: SerializeField] public List<GoodPlacesOnShelf> GoodPlacesOnShelf { get; private set; }

        private readonly List<Transform> placesForBuyer = new();
        protected override void Configure(IContainerBuilder builder)
        {
            var buyerPlacesParent = FindDeepChild(transform, "PlacesForBuyer");

            if (buyerPlacesParent == null)
                throw new System.Exception($"ShelfOfGoodsLifetimeScope: Не найден PlacesForBuyer в префабе {gameObject.name}");

            foreach (Transform child in buyerPlacesParent)
            {
                placesForBuyer.Add(child);
            }

            var interactable = gameObject.AddComponent<Interactable.Interactable>();
            var hoverTrigger = gameObject.AddComponent<HoverTrigger>();
            var building = gameObject.AddComponent<Building>();
            building.SetContent(Center);

            builder.RegisterInstance(interactable);
            builder.RegisterInstance(hoverTrigger).AsSelf();
            builder.RegisterInstance(building);

            builder.RegisterInstance(transform);
            builder.RegisterInstance(placesForBuyer).Keyed("placesForBuyer");
            builder.RegisterInstance(GoodPlacesOnShelf);
            
            builder.Register<ItemTaker>(Lifetime.Scoped).AsSelf();
            builder.Register<ShelfPopup>(Lifetime.Scoped).As<IObjectPopup>().AsSelf();
            builder.Register<BuildingInteractableFlag>(Lifetime.Scoped).AsSelf();
            
            builder.RegisterEntryPoint<ShelfInventory>(Lifetime.Scoped).As<IInventory>().AsSelf();
            builder.RegisterEntryPoint<InventoryDisposable>().AsSelf();
            // builder.RegisterEntryPoint<ShelfDisposable>().AsSelf();
            builder.RegisterEntryPoint<ShelfInfoRecorder>().AsSelf();
            builder.RegisterEntryPoint<ItemTaker>().AsSelf();
            
            builder.RegisterBuildCallback(c => { c.Inject(hoverTrigger); });
        }

        // =====================================================================
        // UNIVERSAL DEEP SEARCH — ищет Transform по имени на ЛЮБОЙ глубине
        // =====================================================================
        private Transform FindDeepChild(Transform parent, string target)
        {
            var normalizedTarget = NormalizeName(target);

            foreach (var child in parent.GetComponentsInChildren<Transform>(true))
            {
                if (NormalizeName(child.name) == normalizedTarget)
                    return child;
            }

            return null;
        }

        private string NormalizeName(string s)
            => new string(s.Where(ch => !char.IsWhiteSpace(ch)).ToArray()).ToUpperInvariant();
    }
}
