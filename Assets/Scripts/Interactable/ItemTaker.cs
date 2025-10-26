using Inventory;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Interactable
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ItemTaker : IStartable
    {
        private readonly IInventory inventory;

        public ItemTaker
            (
                Interactable interactable,
                IInventory inventory
            )
        {
            this.inventory = inventory;
            interactable.Interacted += Take;
        }

        public void Start() { }

        private void Take(LifetimeScope scope)
        {
            var otherInventory = scope.Container.Resolve<IInventory>();
            if (otherInventory.CanGet())
            {
                if(!inventory.CanAdd(otherInventory.GetConfig()))
                    return;
                var dropItem = otherInventory.Get();
                var dropItemTransform = dropItem.transform;
                inventory.Add(dropItem.Config, dropItemTransform.localToWorldMatrix);
                Object.Destroy(dropItemTransform.gameObject);
            }
        }
    }
}