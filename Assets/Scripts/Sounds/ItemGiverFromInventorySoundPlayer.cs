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
        private readonly SoundConfig itemGivenSound;
        private readonly GameObject gameObject;

        private readonly IPublisher<PlaySoundMessage> globalPlaySoundPublisher;

        public ItemGiverFromInventorySoundPlayer
            (
                [Key("ItemGivenSound")] SoundConfig itemGivenSound,
                GameObject gameObject,
                ISubscriber<ItemGivenFromInventory> itemGivenFromInventorySubscriber
            )
        {
            this.itemGivenSound = itemGivenSound;
            this.gameObject = gameObject;
            
            globalPlaySoundPublisher = GlobalMessagePipe.GetPublisher<PlaySoundMessage>();

            itemGivenFromInventorySubscriber.Subscribe(PlaySound);
        }

        private void PlaySound(ItemGivenFromInventory msg)
        {
            var newSettings = itemGivenSound.SoundSettings;
            newSettings.position = gameObject.transform.position;
            globalPlaySoundPublisher.Publish(new PlaySoundMessage(newSettings));
        }

        public void Start() { }
    }
}