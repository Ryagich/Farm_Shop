using System;
using BuildingsAndGrid.Buildings;
using Inventory;
using Inventory.Item;
using MessagePipe;
using Messages;
using Sounds;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Object = UnityEngine.Object;

namespace Checkout
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class CheckoutController : IStartable, IDisposable
    {
        private readonly SoundsConfig soundsConfig;
        private readonly Transform placeForMoneyTransform;
        private readonly IInventory rawInventory;
        private readonly IInventory completeInventory;
        private readonly CheckoutsController checkoutsController;
        private readonly BuildingInteractableFlag buildingInteractableFlag;
        private readonly IPublisher<PlaySoundMessage> playSoundMessagePublisher;
        public ByersQueue ByersQueue { get; }
        public MoneyTaker MoneyTaker { get; }
        public bool CanPay;
        public bool IsInteractable => buildingInteractableFlag.IsInteractable;
        public CheckoutController
            (
                SoundsConfig soundsConfig,
                [Key("PlaceForMoney")] Transform placeForMoneyTransform,
                [Key("RawInventory")] IInventory rawInventory,
                [Key("CompleteInventory")] IInventory completeInventory,
                Interactable.Interactable interactable,
                ByersQueue byersQueue,
                CheckoutsController checkoutsController,
                MoneyTaker moneyTaker,
                BuildingInteractableFlag buildingInteractableFlag,
                IPublisher<PlaySoundMessage> playSoundMessagePublisher
            )
        {
            this.soundsConfig = soundsConfig;
            this.placeForMoneyTransform = placeForMoneyTransform;
            this.rawInventory = rawInventory;
            this.completeInventory = completeInventory;
            this.checkoutsController = checkoutsController;
            this.buildingInteractableFlag = buildingInteractableFlag;
            this.playSoundMessagePublisher = playSoundMessagePublisher;
            ByersQueue = byersQueue;
            MoneyTaker = moneyTaker;

            interactable.Interacted += MoveItems;
            interactable.EndInteracted += OnStopInteract;
        }

        public void Start()
        {
            checkoutsController.RegisterCheckout(new NewCheckoutCreatedMessage(this));
        }
        
        public void Dispose()
        {
            checkoutsController.UnregisterCheckout(new CheckoutDeletedMessage(this));
        }

        public bool CanGet()
        {
           return completeInventory.HaveItem;
        }
        
        public void Add(ItemConfig itemConfig, Matrix4x4 matrix)
        {
            rawInventory.Add(itemConfig, matrix);
        }
        
        public ItemHolder Get()
        {
            return completeInventory.Get();
        }
        
        private void MoveItems(LifetimeScope scope)
        {
            CanPay = true;
            if (rawInventory.HaveItem)
            {
                var itemHolder = rawInventory.Get();
                completeInventory.Add(itemHolder.Config, itemHolder.transform.localToWorldMatrix);
                Object.Destroy(itemHolder.gameObject);
                playSoundMessagePublisher.Publish(new PlaySoundMessage(soundsConfig.ScannerSoundSettings.SoundSettings, placeForMoneyTransform.position, null));
            }
        }

        private void OnStopInteract(LifetimeScope scope)
        {
            CanPay = false;
        }
    }
}