using MessagePipe;
using Messages;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Sounds
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ItemGiverFromInventorySoundPlayer : IStartable
    {
        public SoundConfig itemGivenSound;
        private readonly GameObject gameObject;

        private readonly IPublisher<PlaySoundMessage> globalPlaySoundPublisher;

        public ItemGiverFromInventorySoundPlayer
            (
                GameObject gameObject,
                ISubscriber<ItemGivenFromInventory> itemGivenFromInventorySubscriber
            )
        {
            this.gameObject = gameObject;
            
            globalPlaySoundPublisher = GlobalMessagePipe.GetPublisher<PlaySoundMessage>();

            itemGivenFromInventorySubscriber.Subscribe(PlaySound);
        }

        private void PlaySound(ItemGivenFromInventory msg)
        {
            if (!itemGivenSound)
                return;
            var newSettings = itemGivenSound.SoundSettings;
            globalPlaySoundPublisher.Publish(new PlaySoundMessage(newSettings, gameObject.transform.position, null));
        }

        public void Start() { }
    }
}