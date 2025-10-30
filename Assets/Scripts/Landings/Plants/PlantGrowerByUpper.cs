using System.Linq;
using Landings.Plants.PlantConfigs;
using MessagePipe;
using Messages;
using VContainer.Unity;
using UnityEngine;
using VContainer;
using Object = UnityEngine.Object;

namespace Landings
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class PlantGrowerByUpper : ITickable, IGrower
    {
        private readonly Transform parent;
        private readonly PlantConfig plantConfig;
        private readonly IObjectResolver resolver;
        private readonly IPublisher<PlantHasGrownMessage> plantHasGrownPublisher;

        private readonly IPublisher<PlaySoundMessage> globalPlaySoundPublisher;

        private GameObject plant;
        public bool IsPlanting { get; private set; }
        public float GrowTime { get; private set; }
        public float Distance { get; private set; }
        public float LostDistance { get; private set; }

        private float speed;
        
        public PlantGrowerByUpper
            (
                PlantConfig plantConfig,
                Transform parent,
                IObjectResolver resolver,
                IPublisher<PlantHasGrownMessage> plantHasGrownPublisher
            )
        {
            this.plantConfig = plantConfig;
            this.parent = parent;
            this.resolver = resolver;
            this.plantHasGrownPublisher = plantHasGrownPublisher;
            
            globalPlaySoundPublisher = GlobalMessagePipe.GetPublisher<PlaySoundMessage>();
        }

        public void StartGrow()
        {
            UpdateLocalValues();
            plant = resolver.Instantiate(plantConfig.Stages.First());
            var t = plant.transform;
            t.SetParent(parent);
            t.localPosition = plantConfig.StartPosition;
            IsPlanting = true;
        }

        public GameObject GivePlant()
        {
            var toGive = plant;
            plant = null;
            return toGive;
        }
        
        public void DeletePlant()
        {
            if (plant)
                Object.Destroy(plant);
            plant = null;
        }
        
        public void Tick()
        {
            if (!plant || !IsPlanting)
                return;
            if (plant.transform.localPosition.Equals(plantConfig.TargetPosition))
            {
                IsPlanting = false;
                plantHasGrownPublisher.Publish(new PlantHasGrownMessage(this));
                var newSettings = plantConfig.PlantSoundsSettings.GrownStageSoundsSettings;
                newSettings.position = plantConfig.TargetPosition;
                globalPlaySoundPublisher.Publish(new PlaySoundMessage(newSettings));
                return;
            }
            var localPos = plant.transform.localPosition;
            plant.transform.localPosition = Vector3.MoveTowards(localPos, plantConfig.TargetPosition,
                                                                speed * Time.deltaTime);
            LostDistance = Vector3.Distance(localPos, plantConfig.StartPosition);
        }

        private void UpdateLocalValues()
        {
            Distance = Vector3.Distance(plantConfig.StartPosition, plantConfig.TargetPosition);
            GrowTime = Random.Range(plantConfig.GrowTime.x, plantConfig.GrowTime.y);
            speed = Distance / GrowTime;
        }
    }
}