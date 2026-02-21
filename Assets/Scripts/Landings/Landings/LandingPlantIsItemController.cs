using System.Linq;
using BuildingsAndGrid.Buildings;
using Inventory;
using Inventory.Item;
using Landings.Plants;
using Landings.Plants.PlantConfigs;
using MessagePipe;
using Messages;
using Sounds;
using Storage;
using UniRx;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using YG;

namespace Landings.Landings
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class LandingPlantIsItemController : IStartable
    {
        public PlantConfig PlantConfig { get; private set; }

        private readonly PlantsStorage plantsStorage;
        private readonly Building building;
        private readonly IInventory inventory;
        private readonly IGrower growerByUpper;
        private readonly IGrower growerByStages;
        private readonly ItemGiverFromInventorySoundPlayer itemGiverFromInventorySoundPlayer;

        private readonly CompositeDisposable disposables = new();

        public LandingPlantIsItemController
            (
                PlantsStorage plantsStorage,
                Building building,
                IInventory inventory,
                [Key(nameof(PlantGrowerByUpper))] IGrower growerByUpper,
                [Key(nameof(PlantGrowerByStages))] IGrower growerByStages,
                ItemGiverFromInventorySoundPlayer itemGiverFromInventorySoundPlayer,
                ISubscriber<PlantHasGrownMessage> plantHasGrownSubscriber,
                ISubscriber<PlantHasFinishedGrownMessage> plantHasFinishedGrownSubscriber,
                ISubscriber<ItemGivenFromInventory> itemGivenFromInventoryMessage
            )
        {
            this.plantsStorage = plantsStorage;
            this.building = building;
            this.inventory = inventory;
            this.growerByUpper = growerByUpper;
            this.growerByStages = growerByStages;
            this.itemGiverFromInventorySoundPlayer = itemGiverFromInventorySoundPlayer;

            plantHasGrownSubscriber.Subscribe(StartGrowByStages).AddTo(disposables);
            plantHasFinishedGrownSubscriber.Subscribe(OnGrown).AddTo(disposables);  
            itemGivenFromInventoryMessage.Subscribe(StartGrowByUp).AddTo(disposables);
        }

        public void Start()
        {
            var lastSave = YG2.saves.PlantsSave.FirstOrDefault(save => save.Cell.Equals(building.Cell));
            if (lastSave != null)
            {
                ChangeConfig(plantsStorage.GetPlantConfigById(lastSave.Id));
            }
            if (building.HaveLastPosition)
            {
                lastSave = YG2.saves.PlantsSave.FirstOrDefault(save => save.Cell.Equals(building.LastCell));
                if (lastSave != null)
                {
                    YG2.saves.PlantsSave.Remove(lastSave);
                    var plantConfig = plantsStorage.GetPlantConfigById(lastSave.Id);
                    ChangeConfig(plantConfig);
                    plantsStorage.Get(plantConfig);
                } 
            }
        }

        private void StartGrowByUp(ItemGivenFromInventory msg)
        {
            growerByUpper.StartGrow(PlantConfig);
        }

        private void StartGrowByStages(PlantHasGrownMessage msg)
        {
            growerByUpper.DeletePlant();
            growerByStages.StartGrow(PlantConfig);
        }
        
        private void OnGrown(PlantHasFinishedGrownMessage msg)
        {
            var plant = msg.Grower.GivePlant();
            var ItemHolder = plant.GetComponent<ItemHolder>();
            inventory.Add(ItemHolder.Config, plant.transform.localToWorldMatrix);
            Object.Destroy(plant);
        }

        public void ChangeConfig(PlantConfig plantConfig)
        {
            var lastSave = YG2.saves.PlantsSave.FirstOrDefault(save => save.Cell.Equals(building.Cell) 
                                                                    && save.Id.Equals(plantConfig.Id));
            if (lastSave != null)
            {
                YG2.saves.PlantsSave.Remove(lastSave);
            }
            YG2.saves.PlantsSave.Add(new PlantSave(plantConfig.Id, building.Cell));
            YG2.SaveProgress();
            if (PlantConfig)
            {
                growerByUpper.DeletePlant();
                growerByStages.DeletePlant();
            }
            while (inventory.HaveItem)
            {
                Object.Destroy(inventory.Get().gameObject);
            }
            itemGiverFromInventorySoundPlayer.itemGivenSound = plantConfig.ItemGivenSound;
            PlantConfig = plantConfig;
            growerByUpper.StartGrow(plantConfig);
        }
    }
}