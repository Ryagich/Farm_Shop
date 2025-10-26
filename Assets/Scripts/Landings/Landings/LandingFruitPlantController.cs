using System.Collections.Generic;
using System.Linq;
using Inventory.ObjectInventory;
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
    public class LandingFruitPlantController : IStartable
    {
        private readonly IGrower growerByUpper;
        private readonly IGrower growerByStages;
        private readonly FruitGrower fruitGrower;
        private readonly FruitPlantInventory inventory;
        private readonly CompositeDisposable disposables = new();
        
        private GameObject plant;
        public int fruitCount { get; private set; }
        private int fruitGivensCount;

        public LandingFruitPlantController
            (
                [Key(nameof(PlantGrowerByUpper))] IGrower growerByUpper,
                [Key(nameof(PlantGrowerByStages))] IGrower growerByStages,
                FruitGrower fruitGrower,
                FruitPlantInventory inventory,
                ISubscriber<PlantHasGrownMessage> plantHasGrownSubscriber,
                ISubscriber<PlantHasFinishedGrownMessage> plantHasFinishedGrownSubscriber,
                ISubscriber<ItemGivenFromInventory> ItemGivenFromInventorySubscriber,
                ISubscriber<FruitHasGrown> FruitHasGrownSubscriber
            )
        {
            this.growerByUpper = growerByUpper;
            this.growerByStages = growerByStages;
            this.fruitGrower = fruitGrower;
            this.inventory = inventory;

            plantHasGrownSubscriber.Subscribe(StartGrowByStages).AddTo(disposables);
            plantHasFinishedGrownSubscriber.Subscribe(OnPlantFinishedGrow).AddTo(disposables);
            FruitHasGrownSubscriber.Subscribe(OnFruitGrown).AddTo(disposables);
            ItemGivenFromInventorySubscriber.Subscribe(OnItemGiven).AddTo(disposables);
            growerByUpper.StartGrow();
        }

        private void OnFruitGrown(FruitHasGrown msg)
        {
            inventory.Add(msg.Fruit);
        }
        
        private void StartGrowByStages(PlantHasGrownMessage msg)
        {
            growerByUpper.DeletePlant();
            growerByStages.StartGrow();
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
            fruitCount = fruitGrower.StartGrow();
            if (fruitCount <= 0)
            {
                Restart();
            }
        }

        private void OnItemGiven(ItemGivenFromInventory msg)
        {
            fruitGivensCount++;
            if (fruitGivensCount >= fruitCount)
            {
                Restart();
            }
        }

        private void Restart()
        {
            Object.Destroy(plant);
            fruitGivensCount = 0;
            growerByUpper.StartGrow();
        }
        
        public void Start() { }
    }
}