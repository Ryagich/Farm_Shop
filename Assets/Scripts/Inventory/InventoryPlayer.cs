using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Inventory.Item;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Inventory
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class InventoryPlayer : IInventory
    {
        public List<ItemHolder> Items { get; private set; } = new();

        private readonly InventoryConfig inventoryConfig;
        private readonly IObjectResolver resolver;

        [SuppressMessage("ReSharper", "ParameterHidesMember")]
        public InventoryPlayer
            (
                InventoryConfig inventoryConfig,
                IObjectResolver resolver
            )
        {
            this.inventoryConfig = inventoryConfig;
            this.resolver = resolver;
        }

        public bool CanAdd(ItemConfig itemConfig)
            => Items.Count is 0
            || (Items.Count < inventoryConfig.MaxCount && Items.First().Config == itemConfig);


        public void Add(ItemConfig itemConfig, Matrix4x4 position)
        {
            if (CanAdd(itemConfig))
            {
                var handItem = resolver.Instantiate(itemConfig.HandPrefab);
                handItem.transform.SetPositionAndRotation(position.GetPosition(), position.rotation);
                Items.Add(handItem);
            }
        }

        public bool CanGet() => Items.Count is not 0;
        public ItemConfig GetConfig() => Items.Last().Config;

        public ItemHolder Get()
        {
            var itemHolder = Items.Last();
            Items.Remove(itemHolder);
            return itemHolder;
        }
        
        public void Remove(ItemHolder itemHolder) => Items.Remove(itemHolder);
    }
}