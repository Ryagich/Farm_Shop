using BuildingsAndGrid.Buildings;
using Inventory;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Interactable
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ItemTakerWithBuildingInteractableFlag : IStartable
    {
        private readonly IInventory inventory;
        private readonly BuildingInteractableFlag buildingInteractableFlag;

        public ItemTakerWithBuildingInteractableFlag
            (
                Interactable interactable,
                IInventory inventory,
                BuildingInteractableFlag buildingInteractableFlag
            )
        {
            this.inventory = inventory;
            this.buildingInteractableFlag = buildingInteractableFlag;
            interactable.Interacted += Take;
        }

        public void Start() { }

        private void Take(LifetimeScope scope)
        {
            var otherInventory = scope.Container.Resolve<IInventory>();
            if (otherInventory.CanGet() && buildingInteractableFlag.IsInteractable)
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