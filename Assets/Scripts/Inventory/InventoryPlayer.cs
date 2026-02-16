using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Inventory.Item;
using UniRx;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Inventory
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class InventoryPlayer : IInventory
    {
        public ReactiveCollection<ItemHolder> Items { get; private set; } = new();

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
            || (Items.Count < inventoryConfig.MaxCount && Items.First().Config.Id.Equals(itemConfig.Id));


        public void Add(ItemConfig config, Matrix4x4 position)
        {
            if (CanAdd(config))
            {
                var handItem = resolver.Instantiate(config.HandPrefab);
                handItem.transform.SetPositionAndRotation(position.GetPosition(), position.rotation);
                Items.Add(handItem);
            }
        }

        public bool CanGet(ItemConfig itemConfig) => Items.Count is not 0;
        public ItemConfig GetConfig() => Items.Last().Config;
       
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
        
        public void Remove(ItemHolder itemHolder) => Items.Remove(itemHolder);
    }
}