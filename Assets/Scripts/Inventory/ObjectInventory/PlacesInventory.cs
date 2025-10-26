using System.Collections.Generic;
using System.Linq;
using Inventory.Item;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Inventory.ObjectInventory
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class PlacesInventory : IInventory
    {
        public List<ItemHolder> Items { get; private set; } = new();
        
        private readonly ItemConfig itemConfig;
        private readonly int placesCount;
        private readonly IObjectResolver resolver;
        
        public PlacesInventory
            (
                ItemConfig itemConfig,
                IObjectResolver resolver, 
                [Key("placesCount")] int placesCount
            )
        {
            this.itemConfig = itemConfig;
            this.resolver = resolver;
            this.placesCount = placesCount;
        }

        public bool CanAdd(ItemConfig config) => Items.Count < placesCount
                                              && itemConfig == config;  
        public ItemConfig GetConfig() => itemConfig;

        public void Add(ItemConfig newItemConfig, Matrix4x4 position)
        {
            var handItem = resolver.Instantiate(newItemConfig.HandPrefab);
            handItem.transform.SetPositionAndRotation(position.GetPosition(), position.rotation);
            Items.Add(handItem);
        }

        public bool CanGet() => Items.Count is not 0;

        public ItemHolder Get()
        {
            var itemHolder = Items.Last();
            Items.Remove(itemHolder);
            return itemHolder;
        }

        public void Remove(ItemHolder itemHolder)
        {
            Items.Remove(itemHolder);
        }
    }
}