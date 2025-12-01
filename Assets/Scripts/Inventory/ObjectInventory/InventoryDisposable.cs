using System;
using VContainer.Unity;
using Object = UnityEngine.Object;

namespace Inventory.ObjectInventory
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class InventoryDisposable : IStartable, IDisposable
    {
        private readonly IInventory inventory;

        public InventoryDisposable(IInventory inventory)
        {
            this.inventory = inventory;
        }
        
        public void Dispose()
        {
            while (inventory.CanGet())
            {
                var item = inventory.Get();
                if (item)
                    Object.Destroy(item.gameObject);
            }
        }

        public void Start() { }
    }
}