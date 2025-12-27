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
    public class StepSoundPlayer : IFixedTickable
    {
        private readonly GridSettings gridSettings;
        private readonly PlayerMovementConfig playerMovementConfig;
        private readonly SoundsConfig soundsConfig;
        private readonly TilesController tilesController;
        private readonly Transform transform;
        private readonly IPublisher<PlaySoundMessage> soundMessagePublisher;

        private Vector3 lastPosition;
        private float distanceAccumulator;

        // расстояние между шагами

        public StepSoundPlayer(
                GridSettings gridSettings,
                PlayerMovementConfig playerMovementConfig,
                SoundsConfig soundsConfig,
                TilesController tilesController,
                Transform transform,
                IPublisher<PlaySoundMessage> soundMessagePublisher
            )
        {
            this.gridSettings = gridSettings;
            this.playerMovementConfig = playerMovementConfig;
            this.soundsConfig = soundsConfig;
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

            if (distanceAccumulator < playerMovementConfig.StepDistance)
                return;
            distanceAccumulator -= playerMovementConfig.StepDistance;
            tilesController.Tiles.TryGetTile((int)(currentPosition.x / gridSettings.TileSize.x),
                                             (int)(currentPosition.z / gridSettings.TileSize.z),
                                             out var tile);
            if (tile != null)
            {
                if (tile.Type is Area.Wall or Area.Production)
                {
                    soundMessagePublisher.Publish(new PlaySoundMessage(soundsConfig.StepOnStoneSoundSettings.SoundSettings,
                                                                       transform.position, null));
                }
                else if (tile.Type is Area.Shop)
                    soundMessagePublisher.Publish(new PlaySoundMessage(soundsConfig.StepOnWoodSoundSettings.SoundSettings,
                                                                       transform.position, null));
                else if (tile.Type is Area.Garden)
                    soundMessagePublisher.Publish(new PlaySoundMessage(soundsConfig.StepOnGroundSoundSettings.SoundSettings,
                                                                       transform.position, null));
            }
        }
    }
}