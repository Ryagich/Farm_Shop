using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Inventory.Item;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Inventory.ObjectInventory
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class SimpleInventory : IInventory
    {
        public List<ItemHolder> Items { get; private set; } = new();
        private readonly IObjectResolver resolver;

        [SuppressMessage("ReSharper", "ParameterHidesMember")]
        public SimpleInventory(IObjectResolver resolver)
        {
            this.resolver = resolver;
        }
        
        public bool CanAdd(ItemConfig config) => true;

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