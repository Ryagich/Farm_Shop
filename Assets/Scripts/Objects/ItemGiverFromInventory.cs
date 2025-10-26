using Inventory;
using MessagePipe;
using Messages;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Objects
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ItemGiverFromInventory : IStartable
    {
        private readonly IPublisher<ItemGivenFromInventory> itemGivenFromInventoryMessage;
        private readonly IInventory inventory;

        public ItemGiverFromInventory
            (
                Interactable.Interactable interactable,
                IPublisher<ItemGivenFromInventory> itemGivenFromInventoryMessage,
                IInventory inventory
            )
        {
            this.itemGivenFromInventoryMessage = itemGivenFromInventoryMessage;
            this.inventory = inventory;

            interactable.Interacted += Interact;
        }
        
        private void Interact(LifetimeScope scope)
        {
            var otherInventory = scope.Container.Resolve<IInventory>();
            if (!inventory.CanGet())
                return;
            var itemConfig = inventory.GetConfig();
            if (otherInventory.CanAdd(itemConfig))
            {
                var item = inventory.Get();
                otherInventory.Add(itemConfig, item.transform.localToWorldMatrix);
                Object.Destroy(item.gameObject);
                itemGivenFromInventoryMessage.Publish(new ItemGivenFromInventory());
            }
        }

        public void Start() { }
    }
}