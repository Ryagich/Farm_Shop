using System.Linq;
using Inventory.Item;
using UniRx;
using UnityEngine;

namespace Inventory.ObjectInventory
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class PlacesInventory : IInventory
    {
        public ReactiveCollection<ItemHolder> Items { get; private set; } = new();
        
        private ItemConfig itemConfig;
        public readonly int MaxItems;
        
        public PlacesInventory
            (
                ItemConfig itemConfig,
                int maxItems
            )
        {
            this.itemConfig = itemConfig;
            this.MaxItems = maxItems;
        }

        // ReSharper disable once ParameterHidesMember
        public void ChangeItemConfig(ItemConfig itemConfig)
        {
            this.itemConfig = itemConfig;
        }

        public bool CanAdd(ItemConfig config)
        {
            if (itemConfig is null)
                return false;
            return Items.Count < MaxItems
                && itemConfig.Id.Equals(config.Id);
        }

        public bool CanGet(ItemConfig config) => Items.Count is not 0 
                                              && itemConfig.Id.Equals(config.Id);

        public ItemConfig GetConfig() => itemConfig;

        public void Add(ItemConfig config, Matrix4x4 position)
        {
            var handItem = Object.Instantiate(config.HandPrefab);
            handItem.transform.SetPositionAndRotation(position.GetPosition(), position.rotation);
            Items.Add(handItem);
        }

        public void Add(ItemHolder itemHolder)
        {
            Items.Add(itemHolder);
        }
        
        public ItemHolder Get()
        {
            var itemHolder = Items.Last();
            Items.Remove(itemHolder);
            return itemHolder;
        }

        public ItemHolder Get(ItemConfig config)
        {
            throw new System.NotImplementedException();
        }

        public void Remove(ItemHolder itemHolder)
        {
            Items.Remove(itemHolder);
        }
    }
}