using Inventory;
using Inventory.Item;
using Messages;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Checkout
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class CheckoutController : IStartable
    {
        private readonly IInventory rawInventory;
        private readonly IInventory completeInventory;
        public ByersQueue ByersQueue { get; }
        public MoneyTaker MoneyTaker { get; }
        public bool CanPay;
        
        public CheckoutController
            (
                [Key("RawInventory")] IInventory rawInventory,
                [Key("CompleteInventory")] IInventory completeInventory,
                Interactable.Interactable interactable,
                ByersQueue byersQueue,
                CheckoutsController checkoutsController,
                MoneyTaker moneyTaker
            )
        {
            this.rawInventory = rawInventory;
            this.completeInventory = completeInventory;
            ByersQueue = byersQueue;
            MoneyTaker = moneyTaker;

            checkoutsController.OnNewShelfCreated(new NewCheckoutCreatedMessage(this));
            interactable.Interacted += MoveItems;
            interactable.EndInteracted += OnStopInteract;

        }

        public bool CanGet()
        {
           return completeInventory.CanGet();
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
            if (rawInventory.CanGet())
            {
                var itemHolder = rawInventory.Get();
                completeInventory.Add(itemHolder.Config, itemHolder.transform.localToWorldMatrix);
                Object.Destroy(itemHolder.gameObject);
            }
        }

        private void OnStopInteract(LifetimeScope scope)
        {
            CanPay = false;
        }
        
        public void Start() { }
    }
}