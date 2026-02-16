using System.Collections.Generic;
using Inventory.Item;
using UniRx;
using UnityEngine;

namespace Inventory
{
    public interface IInventory
    {
        public ReactiveCollection<ItemHolder> Items { get;}
        public bool CanAdd(ItemConfig config);
        public void Add(ItemConfig config, Matrix4x4 position);
        public bool CanGet(ItemConfig config);
        public ItemConfig GetConfig();
        public ItemHolder Get();
        public ItemHolder Get(ItemConfig config);
        public void Remove(ItemHolder itemHolder);
        public bool HaveItem => Items.Count > 0;
    }
}