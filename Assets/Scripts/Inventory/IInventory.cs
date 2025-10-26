using System.Collections.Generic;
using Inventory.Item;
using UnityEngine;

namespace Inventory
{
    public interface IInventory
    {
        public List<ItemHolder> Items { get;}
        public bool CanAdd(ItemConfig config);
        public void Add(ItemConfig itemConfig, Matrix4x4 position);
        public bool CanGet();
        public ItemConfig GetConfig();
        public ItemHolder Get();
        public void Remove(ItemHolder itemHolder);
    }
}