using System.Linq;
using Inventory.Item;
using UniRx;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Inventory.ObjectInventory
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class MaterialHolderInventory : IInventory
    {
        public ReactiveCollection<ItemHolder> Items { get; private set; } = new();

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
        public bool CanAdd(ItemConfig config) => Items.Count < maxCount && config.Equals(itemConfig);
        public bool CanGet(ItemConfig config) => Items.Count is not 0;
        public ItemConfig GetConfig() => itemConfig;

        public void Add(ItemConfig config, Matrix4x4 position)
        {
            if (CanAdd(config))
            {
                var handItem = resolver.Instantiate(config.HandPrefab);
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

        public ItemHolder Get(ItemConfig config)
        {
            var itemHolder = Items.Last();
            Items.Remove(itemHolder);
            return itemHolder;
        }

        public void Remove(ItemHolder itemHolder)
        {
            Items.Remove(itemHolder);
        }
        public bool HaveItem => Items.Count > 0;
    }
}