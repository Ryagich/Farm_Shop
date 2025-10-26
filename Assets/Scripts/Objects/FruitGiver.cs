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
        private readonly IPublisher<ItemGivenFromInventory> itemGivenFromInventoryMessage;
        private readonly FruitPlantInventory inventory;
        private readonly FruitPlantConfig fruitPlantConfig;

        public FruitGiver
            (
                Interactable.Interactable interactable,
                IPublisher<ItemGivenFromInventory> itemGivenFromInventoryMessage,
                FruitPlantInventory inventory,
                FruitPlantConfig fruitPlantConfig
            )
        {
            this.itemGivenFromInventoryMessage = itemGivenFromInventoryMessage;
            this.inventory = inventory;
            this.fruitPlantConfig = fruitPlantConfig;

            interactable.Interacted += Interact;
        }
        
        private void Interact(LifetimeScope scope)
        {
            var otherInventory = scope.Container.Resolve<IInventory>();
            if (!inventory.CanGet())
                return;
            if (otherInventory.CanAdd(fruitPlantConfig.HandFruit))
            {
                var item = inventory.Get();
                otherInventory.Add(fruitPlantConfig.HandFruit, item.FruitObj.transform.localToWorldMatrix);
                Object.Destroy(item.FruitObj);
                itemGivenFromInventoryMessage.Publish(new ItemGivenFromInventory());
            }
        }
        
        public void Start() { }
    }
}