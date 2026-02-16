using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Inventory.Item;
using UniRx;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Inventory.ObjectInventory
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class SimpleInventory : IInventory
    {
        public ReactiveCollection<ItemHolder> Items { get; private set; } = new();
        private readonly IObjectResolver resolver;

        [SuppressMessage("ReSharper", "ParameterHidesMember")]
        public SimpleInventory(IObjectResolver resolver)
        {
            this.resolver = resolver;
        }
        
        public bool CanAdd(ItemConfig itemConfig) => true;

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
            throw new NotImplementedException();
        }

        public void Remove(ItemHolder itemHolder) => Items.Remove(itemHolder);
    }
}