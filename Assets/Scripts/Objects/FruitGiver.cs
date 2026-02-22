using Inventory;
using Inventory.ObjectInventory;
using Landings.Plants.PlantConfigs;
using MessagePipe;
using Messages;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Objects
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class FruitGiver : IStartable
    {
        public FruitPlantConfig FruitPlantConfig;
        
        private readonly IPublisher<ItemGivenFromInventory> itemGivenFromInventoryMessage;
        private readonly FruitPlantInventory inventory;

        public FruitGiver
            (
                Interactable.Interactable interactable,
                IPublisher<ItemGivenFromInventory> itemGivenFromInventoryMessage,
                FruitPlantInventory inventory
            )
        {
            this.itemGivenFromInventoryMessage = itemGivenFromInventoryMessage;
            this.inventory = inventory;

            interactable.Interacted += Interact;
        }
        
        private void Interact(LifetimeScope scope)
        {
            if (!FruitPlantConfig || !inventory.CanGet())
                return;
            var otherInventory = scope.Container.Resolve<IInventory>();
            if (otherInventory.CanAdd(FruitPlantConfig.HandFruit))
            {
                var item = inventory.Get();
                otherInventory.Add(FruitPlantConfig.HandFruit, item.FruitObj.transform.localToWorldMatrix);
                Object.Destroy(item.FruitObj);
                itemGivenFromInventoryMessage.Publish(new ItemGivenFromInventory());
            }
        }
        
        public void Start() { }
    }
}