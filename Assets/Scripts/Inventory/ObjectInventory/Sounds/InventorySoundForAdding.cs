using System;
using MessagePipe;
using Messages;
using Sounds;
using UniRx;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Inventory.ObjectInventory.Sounds
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class InventorySoundForAdding : IDisposable, IStartable
    {
        private readonly SoundsConfig soundsConfig;
        private readonly Transform transform;
        private readonly IPublisher<PlaySoundMessage> playSoundMessagePublisher;

        private readonly CompositeDisposable disposables = new();

        public InventorySoundForAdding
            (
                SoundsConfig soundsConfig,
                [Key("TransformForSound")] Transform transform,
                IInventory inventory,
                IPublisher<PlaySoundMessage> playSoundMessagePublisher
            )
        {
            this.soundsConfig = soundsConfig;
            this.transform = transform;
            this.playSoundMessagePublisher = playSoundMessagePublisher;

            inventory.Items
                     .ObserveAdd()
                     .Subscribe(_ => PlaySound())
                     .AddTo(disposables);
        }

        private void PlaySound()
        {
            playSoundMessagePublisher.Publish(
                                              new PlaySoundMessage(
                                                                   soundsConfig.SwipeSoundSettings_1.SoundSettings,
                                                                   transform.position,
                                                                   null));
        }

        public void Dispose()
        {
            disposables.Dispose();
        }

        public void Start() { }
    }
}