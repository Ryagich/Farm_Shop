using Buyer;
using Checkout;
using Inventory;
using Inventory.Item;
using Landings;
using Landings.Plants;
using Shelf;
using Sounds;
using UnityEngine;
using VContainer.Unity;

namespace Messages
{
    public readonly struct PlayerMoveMessage
    {
        public readonly Vector2 Direction;

        public PlayerMoveMessage(Vector2 direction)
        {
            Direction = direction;
        }
    }

    public readonly struct InteractableMessage
    {
        public readonly Interactable.Interactable Interactable;

        public InteractableMessage(Interactable.Interactable interactable)
        {
            Interactable = interactable;
        }
    }

    public readonly struct InteractableEndMessage
    {
        public readonly Interactable.Interactable Interactable;

        public InteractableEndMessage(Interactable.Interactable interactable)
        {
            Interactable = interactable;
        }
    }

    public readonly struct PlayerMadePurchaseMessage
    {
        public readonly LifetimeScope Scope;
        public readonly Vector3 Position;
        public readonly Vector3 Rotation;

        public PlayerMadePurchaseMessage(LifetimeScope scope, Vector3 position, Vector3 rotation)
        {
            Scope = scope;
            Position = position;
            Rotation = rotation;
        }
    }

    //here
    public readonly struct CreatedNewObjectMessage
    {
        public readonly Transform Transform;
        public readonly Vector3 Position;
        public readonly Vector3 Rotation;

        public CreatedNewObjectMessage
            (
                Transform transform,
                Vector3 position,
                Vector3 rotation
            )
        {
            Transform = transform;
            Position = position;
            Rotation = rotation;
        }
    }

    public readonly struct PlantHasGrownMessage
    {
        public readonly IGrower Grower;

        public PlantHasGrownMessage(IGrower grower)
        {
            Grower = grower;
        }
    }

    public readonly struct PlantHasFinishedGrownMessage
    {
        public readonly IGrower Grower;

        public PlantHasFinishedGrownMessage(IGrower grower)
        {
            Grower = grower;
        }
    }

    public readonly struct ItemGivenFromInventory
    {
    }

    public readonly struct FruitHasGrown
    {
        public readonly Fruit Fruit;

        public FruitHasGrown(Fruit fruit)
        {
            Fruit = fruit;
        }
    }

    public readonly struct ItemHasBeenAddedToInventory
    {
    }

    public readonly struct MaterialsHasBeenMovedToProduction
    {
    }

    public readonly struct ProductCreated
    {
        public readonly ItemConfig ItemConfig;
        public readonly Matrix4x4 Position;

        public ProductCreated(ItemConfig itemConfig, Matrix4x4 position)
        {
            ItemConfig = itemConfig;
            Position = position;
        }
    }

    public readonly struct NewShelfCreatedMessage
    {
        public readonly InfoAboutShelfForBuyerGenerator InfoAboutShelfForBuyerGenerator;
        public readonly ItemConfig ItemConfig;
        public readonly IInventory Inventory;

        public NewShelfCreatedMessage
            (
                InfoAboutShelfForBuyerGenerator infoAboutShelfForBuyerGenerator,
                ItemConfig itemConfig,
                IInventory inventory
            )
        {
            InfoAboutShelfForBuyerGenerator = infoAboutShelfForBuyerGenerator;
            ItemConfig = itemConfig;
            Inventory = inventory;
        }
    }

    public readonly struct BuyerIsOverMessage
    {
        public readonly BuyerLifetimeScope BuyerLifetimeScope;

        public BuyerIsOverMessage(BuyerLifetimeScope buyerLifetimeScope)
        {
            BuyerLifetimeScope = buyerLifetimeScope;
        }
    }
    
    public readonly struct NewCheckoutCreatedMessage
    {
        public readonly CheckoutController CheckoutController;
        
        public NewCheckoutCreatedMessage(CheckoutController checkoutController)
        {
            CheckoutController = checkoutController;
        }
    }
    
    public readonly struct PlaySoundMessage
    {
        public readonly SoundsSettings SoundsSettings;

        public PlaySoundMessage(SoundsSettings soundsSettings)
        {
            SoundsSettings = soundsSettings;
        }
    }
}