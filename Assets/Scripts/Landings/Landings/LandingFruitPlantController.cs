using System.Collections.Generic;
using System.Linq;
using BuildingsAndGrid.Buildings;
using Inventory.ObjectInventory;
using Landings.Plants;
using Landings.Plants.PlantConfigs;
using MessagePipe;
using Messages;
using Objects;
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
    public class LandingFruitPlantController : IStartable
    {
        public FruitPlantConfig FruitPlantConfig { get; private set; }

        private readonly FruitPlantInventory inventory;
        private readonly PlantsStorage plantsStorage;
        private readonly Building building;
        private readonly IGrower growerByUpper;
        private readonly IGrower growerByStages;
        private readonly ItemGiverFromInventorySoundPlayer itemGiverFromInventorySoundPlayer;
        private readonly FruitGrower fruitGrower;
        private readonly FruitGiver fruitGiver;
        private readonly CompositeDisposable disposables = new();
        
        private GameObject plant;
        public ReactiveProperty<int> fruitCount { get; private set; } = new();
        private int fruitGivensCount;

        public LandingFruitPlantController
            (
                PlantsStorage plantsStorage,
                Building building,
                [Key(nameof(PlantGrowerByUpper))] IGrower growerByUpper,
                [Key(nameof(PlantGrowerByStages))] IGrower growerByStages,
                ItemGiverFromInventorySoundPlayer itemGiverFromInventorySoundPlayer,
                FruitGrower fruitGrower,
                FruitGiver fruitGiver,
                FruitPlantInventory inventory,
                ISubscriber<PlantHasGrownMessage> plantHasGrownSubscriber,
                ISubscriber<PlantHasFinishedGrownMessage> plantHasFinishedGrownSubscriber,
                ISubscriber<ItemGivenFromInventory> ItemGivenFromInventorySubscriber,
                ISubscriber<FruitHasGrown> FruitHasGrownSubscriber
            )
        {
            this.plantsStorage = plantsStorage;
            this.building = building;
            this.growerByUpper = growerByUpper;
            this.growerByStages = growerByStages;
            this.itemGiverFromInventorySoundPlayer = itemGiverFromInventorySoundPlayer;
            this.fruitGrower = fruitGrower;
            this.fruitGiver = fruitGiver;
            this.inventory = inventory;

            plantHasGrownSubscriber.Subscribe(StartGrowByStages).AddTo(disposables);
            plantHasFinishedGrownSubscriber.Subscribe(OnPlantFinishedGrow).AddTo(disposables);
            FruitHasGrownSubscriber.Subscribe(OnFruitGrown).AddTo(disposables);
            ItemGivenFromInventorySubscriber.Subscribe(OnItemGiven).AddTo(disposables);
        }

        public void Start()
        {
            var lastSave = YG2.saves.PlantsSave.FirstOrDefault(save => save.Cell.Equals(building.Cell));
            if (lastSave != null)
            {
                ChangeConfig(plantsStorage.GetPlantConfigById(lastSave.Id) as FruitPlantConfig);
            }
            if (building.HaveLastPosition)
            {
                lastSave = YG2.saves.PlantsSave.FirstOrDefault(save => save.Cell.Equals(building.LastCell));
                if (lastSave != null)
                {
                    YG2.saves.PlantsSave.Remove(lastSave);
                    var fruitPlantConfig = plantsStorage.GetPlantConfigById(lastSave.Id) as FruitPlantConfig;
                    ChangeConfig(fruitPlantConfig);
                    plantsStorage.Get(fruitPlantConfig);
                } 
            }
        }

        private void OnFruitGrown(FruitHasGrown msg)
        {
            inventory.Add(msg.Fruit);
        }
        
        private void StartGrowByStages(PlantHasGrownMessage msg)
        {
            growerByUpper.DeletePlant();
            growerByStages.StartGrow(FruitPlantConfig);
        }
        
        public void ChangeConfig(FruitPlantConfig fruitPlantConfig)
        {
            var lastSave = YG2.saves.PlantsSave.FirstOrDefault(save => save.Cell.Equals(building.Cell) 
                                                                    && save.Id.Equals(fruitPlantConfig.Id));
            if (lastSave != null)
            {
                YG2.saves.PlantsSave.Remove(lastSave);
            }
            YG2.saves.PlantsSave.Add(new PlantSave(fruitPlantConfig.Id, building.Cell));
            YG2.SaveProgress();
            if (FruitPlantConfig)
            {
                growerByUpper.DeletePlant();
                growerByStages.DeletePlant();
            }
            while (inventory.Fruits.Count > 0)
            {
                Object.Destroy(inventory.Get().FruitObj.gameObject);
            }
            FruitPlantConfig = fruitPlantConfig;
            if (FruitPlantConfig)
            {
                fruitGrower.FruitPlantConfig = fruitPlantConfig;
                fruitGiver.FruitPlantConfig = fruitPlantConfig;
                itemGiverFromInventorySoundPlayer.itemGivenSound = fruitPlantConfig.ItemGivenSound;
                growerByUpper.StartGrow(fruitPlantConfig);
            }
        }
        
        private void OnPlantFinishedGrow(PlantHasFinishedGrownMessage msg)
        {
            plant = growerByStages.GivePlant();
            var children = new List<Transform>();
            var places = new List<Transform>();
            foreach (Transform child in plant.transform)
            {
                children.Add(child);
            }
            var placesParent = children.First(c => c.name.ToUpper().Equals("FruitPlaces".ToUpper()));
            foreach (Transform child in placesParent.transform)
            {
                places.Add(child);
            }
            fruitGrower.SetPoints(places);
            fruitCount.Value = fruitGrower.StartGrow();
            if (fruitCount.Value <= 0)
            {
                StartGrowByUp();
            }
        }

        private void OnItemGiven(ItemGivenFromInventory msg)
        {
            fruitGivensCount++;
            if (fruitGivensCount >= fruitCount.Value)
            {
                StartGrowByUp();
            }
        }

        private void StartGrowByUp()
        {
            Object.Destroy(plant);
            fruitGivensCount = 0;
            growerByUpper.StartGrow(FruitPlantConfig);
        }
    }
}