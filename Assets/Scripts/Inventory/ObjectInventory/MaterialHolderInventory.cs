using System.Collections.Generic;
using System.Linq;
using Inventory.Item;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Inventory.ObjectInventory
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class MaterialHolderInventory : IInventory
    {
        public List<ItemHolder> Items { get; private set; } = new();

        private readonly IObjectResolver resolver;
        private readonly ItemConfig itemConfig;
        private readonly int maxCount;
        
        public MaterialHolderInventory
            (
                IObjectResolver resolver,
                ItemConfig itemConfig,
                int maxCount
            )
        {
            this.resolver = resolver;
            this.itemConfig = itemConfig;
            this.maxCount = maxCount;
        }
        
        public bool HaveFreePlace() => Items.Count < maxCount;
        public bool CanAdd(ItemConfig config) => Items.Count < maxCount && config == itemConfig;
        public ItemConfig GetConfig() => itemConfig;
        public bool CanGet() => Items.Count is not 0;

        public void Add(ItemConfig newItemConfig, Matrix4x4 position)
        {
            if (CanAdd(newItemConfig))
            {
                var handItem = resolver.Instantiate(newItemConfig.HandPrefab);
                handItem.transform.SetPositionAndRotation(position.GetPosition(), position.rotation);
                Items.Add(handItem);
            }
        }

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