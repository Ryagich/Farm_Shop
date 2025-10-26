using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using DG.Tweening;
using Landings.Plants.PlantConfigs;
using MessagePipe;
using Messages;

namespace Landings.Plants
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class FruitGrower : ITickable
    {
        private readonly FruitPlantConfig plantConfig;
        private readonly Transform parent;
        private readonly List<Fruit> fruits = new();
        private readonly IObjectResolver resolver;
        private readonly IPublisher<FruitHasGrown> fruitHasGrownPublisher;

        public FruitGrower
            (
                FruitPlantConfig plantConfig,
                Transform parent,
                IObjectResolver resolver,
                IPublisher<FruitHasGrown> fruitHasGrownPublisher
            )
        {
            this.plantConfig = plantConfig;
            this.parent = parent;
            this.resolver = resolver;
            this.fruitHasGrownPublisher = fruitHasGrownPublisher;
        }

        public void SetPoints(List<Transform> points)
        {
            fruits.Clear();
            foreach (var point in points)
            {
                fruits.Add(new Fruit(point.transform));
            }
        }
        
        public int StartGrow()
        {
            var count = 0;
            foreach (var fruit in fruits)
            {
                if (plantConfig.FruitGrowChance >= Random.Range(.0f, 1.0f))
                {
                    SpawnFruit(fruit);
                    fruit.IsPlanted = false;
                    count++;
                }
            }
            return count;
        }
        
        public void Tick()
        {
            foreach (var fruit in fruits)
            {
                if (fruit.IsPlanted)
                {
                    continue;
                }
                if (fruit.Time >= fruit.StageTime)
                {
                    fruit.Time = 0;
                    NextStage(fruit);
                } 
                fruit.Time += Time.deltaTime;
            }
        }
        
        private void NextStage(Fruit fruit)
        {
            SpawnFruit(fruit);
            if (fruit.CurrentStage >= plantConfig.FruitStages.Count)
            {
                fruit.IsPlanted = true;
                fruit.CurrentStage = 0;    
                fruitHasGrownPublisher.Publish(new FruitHasGrown(fruit));
            }
        }
 
        private void SpawnFruit(Fruit fruit)
        {
            if (fruit.FruitObj)
                Object.Destroy(fruit.FruitObj);
            fruit.StageTime = Random.Range(plantConfig.FruitGrowTime.x, plantConfig.FruitGrowTime.y);
            fruit.FruitObj = resolver.Instantiate(plantConfig.FruitStages[fruit.CurrentStage], fruit.Parent);
            var t = fruit.FruitObj.transform;
            var targetScale = t.localScale;
            // t.position = fruit.Parent.position;
            t.localScale = targetScale * .5f;
            t.DOScale(targetScale, .5f).SetEase(Ease.OutElastic, .2f);
            fruit.CurrentStage++;
        }
    }
    
    public class Fruit
    {
        public Transform Parent;
        public GameObject FruitObj;
        public float Time;
        public float StageTime;
        public int CurrentStage;
        public bool IsPlanted = true;

        public Fruit(Transform parent)
        {
            Parent = parent;
        }
    }
}