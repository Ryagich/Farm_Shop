using Inventory;
using Inventory.Item;
using Landings.Plants;
using MessagePipe;
using Messages;
using UniRx;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Landings.Landings
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class LandingPlantIsItemController : IStartable
    {
        private readonly IInventory inventory;
        private readonly IGrower growerByUpper;
        private readonly IGrower growerByStages;
        
        private readonly CompositeDisposable disposables = new();

        public LandingPlantIsItemController
            (
                IInventory inventory,
                [Key(nameof(PlantGrowerByUpper))] IGrower growerByUpper,
                [Key(nameof(PlantGrowerByStages))] IGrower growerByStages,
                ISubscriber<PlantHasGrownMessage> plantHasGrownSubscriber,
                ISubscriber<PlantHasFinishedGrownMessage> plantHasFinishedGrownSubscriber,
                ISubscriber<ItemGivenFromInventory> itemGivenFromInventoryMessage
            )
        {
            this.inventory = inventory;
            this.growerByUpper = growerByUpper;
            this.growerByStages = growerByStages;
            
            plantHasGrownSubscriber.Subscribe(StartGrowByStages).AddTo(disposables);
            plantHasFinishedGrownSubscriber.Subscribe(OnGrown).AddTo(disposables);  
            itemGivenFromInventoryMessage.Subscribe(StartGrowByUp).AddTo(disposables);  
            
            growerByUpper.StartGrow();
        }

        private void StartGrowByStages(PlantHasGrownMessage msg)
        {
            growerByUpper.DeletePlant();
            growerByStages.StartGrow();
        }
        
        private void OnGrown(PlantHasFinishedGrownMessage msg)
        {
            var plant = msg.Grower.GivePlant();
            var ItemHolder = plant.GetComponent<ItemHolder>();
            inventory.Add(ItemHolder.Config, plant.transform.localToWorldMatrix);
            Object.Destroy(plant);
        }
     
        private void StartGrowByUp(ItemGivenFromInventory msg)
        {
            growerByUpper.StartGrow();
        }
        
        public void Start() { }
    }
}