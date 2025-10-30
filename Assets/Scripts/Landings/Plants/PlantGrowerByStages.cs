using DG.Tweening;
using Landings.Plants.PlantConfigs;
using MessagePipe;
using Messages;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Landings.Plants
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class PlantGrowerByStages : ITickable, IGrower
    {
        private readonly PlantConfig plantConfig;
        private readonly IObjectResolver resolver;
        private readonly IPublisher<PlantHasFinishedGrownMessage> plantHasFinishedGrowPublisher;
        private readonly Transform parent;

        private GameObject plant;
        public bool IsPlanted { get; private set; } = true;
        public int currentStage { get; private set; }
        public float timer { get; private set; }
        public float stageTime { get; private set; }

            private readonly IPublisher<PlaySoundMessage> globalPlaySoundPublisher;

        public PlantGrowerByStages
            (
                PlantConfig plantConfig,
                Transform parent,
                IObjectResolver resolver,
                IPublisher<PlantHasFinishedGrownMessage> plantHasFinishedGrowPublisher
            )
        {
            this.plantConfig = plantConfig;
            this.resolver = resolver;
            this.parent = parent;
            this.resolver = resolver;
            this.plantHasFinishedGrowPublisher = plantHasFinishedGrowPublisher;

            globalPlaySoundPublisher = GlobalMessagePipe.GetPublisher<PlaySoundMessage>();
        }

        public void StartGrow()
        {
            NextStage(false);
            IsPlanted = false;
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
            if (IsPlanted)
                return;

            timer += Time.deltaTime;
            if (timer >= stageTime)
            {
                timer = 0f;
                NextStage();
            }
        }

        private void NextStage(bool doAnimation = true)
        {
            SpawnPlant(doAnimation);
            currentStage++;
            if (currentStage >= plantConfig.Stages.Count)
            {
                IsPlanted = true;
                currentStage = 0;
                plantHasFinishedGrowPublisher.Publish(new PlantHasFinishedGrownMessage(this));
            }
        }

        private void SpawnPlant(bool doAnimation = true)
        {
            if (plant)
                Object.Destroy(plant);
            stageTime = Random.Range(plantConfig.TimeBetweenStages.x, plantConfig.TimeBetweenStages.y);
            plant = resolver.Instantiate(plantConfig.Stages[currentStage]);
            var t = plant.transform;
            t.SetParent(parent);
            t.localPosition = plantConfig.TargetPosition;
            if (doAnimation)
            {
                var targetScale = t.localScale;
                t.localScale = targetScale * .5f;
                t.DOScale(targetScale, .5f).SetEase(Ease.OutElastic, .2f);
            }
            var newSettings = plantConfig.PlantSoundsSettings.GrownStageSoundsSettings;
            newSettings.position = plantConfig.TargetPosition;
            globalPlaySoundPublisher.Publish(new PlaySoundMessage(newSettings));
        }
    }
}