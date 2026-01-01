using BuildingsAndGrid;
using GameModes;
using MessagePipe;
using Messages;
using Sounds;
using UnityEngine;
using VContainer.Unity;

namespace Movement
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class StepSound : IFixedTickable
    {
        private readonly GridSettings gridSettings;
        private readonly StepSoundConfig stepSoundConfig;
        private readonly TilesController tilesController;
        private readonly Transform transform;
        private readonly IPublisher<PlaySoundMessage> soundMessagePublisher;

        private Vector3 lastPosition;
        private float distanceAccumulator;

        public StepSound
            (
                GridSettings gridSettings,
                StepSoundConfig stepSoundConfig,
                TilesController tilesController,
                Transform transform,
                IPublisher<PlaySoundMessage> soundMessagePublisher
            )
        {
            this.gridSettings = gridSettings;
            this.stepSoundConfig = stepSoundConfig;
            this.tilesController = tilesController;
            this.transform = transform;
            this.soundMessagePublisher = soundMessagePublisher;

            lastPosition = transform.position;
        }

        public void FixedTick()
        {
            var currentPosition = transform.position;
            var delta = Vector3.Distance(currentPosition, lastPosition);

            distanceAccumulator += delta;
            lastPosition = currentPosition;

            if (distanceAccumulator < stepSoundConfig.StepDistance)
                return;
            distanceAccumulator -= stepSoundConfig.StepDistance;
            tilesController.Tiles.TryGetTile((int)(currentPosition.x / gridSettings.TileSize.x),
                                             (int)(currentPosition.z / gridSettings.TileSize.z),
                                             out var tile);
            if (tile != null)
            {
                if (tile.Type is Area.Wall or Area.Production)
                {
                    soundMessagePublisher.Publish(new PlaySoundMessage(stepSoundConfig.StepOnStoneSoundSettings.SoundSettings,
                                                                       transform.position, null));
                }
                else if (tile.Type is Area.Shop)
                    soundMessagePublisher.Publish(new PlaySoundMessage(stepSoundConfig.StepOnWoodSoundSettings.SoundSettings,
                                                                       transform.position, null));
                else if (tile.Type is Area.Garden)
                    soundMessagePublisher.Publish(new PlaySoundMessage(stepSoundConfig.StepOnGroundSoundSettings.SoundSettings,
                                                                       transform.position, null));
            }
        }
    }
}