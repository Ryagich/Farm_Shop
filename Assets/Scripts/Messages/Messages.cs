using System.Collections.Generic;
using BuildingsAndGrid;
using BuildingsAndGrid.Buildings;
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

    public readonly struct CreatedNewBuildingOnGridRequest
    {
        public readonly BuildingConfig BuildingConfig;
        public readonly Vector3 Position;
        public readonly Vector3 LocalPosition;
        public readonly Quaternion Rotation;
        public readonly List<Tile> Tiles;
        
        public CreatedNewBuildingOnGridRequest
            (
                BuildingConfig buildingConfig,
                Vector3 position,
                Vector3 localPosition,
                Quaternion rotation,
                List<Tile> tiles
            )
        {
            BuildingConfig = buildingConfig;
            Position = position;
            LocalPosition = localPosition;
            Rotation = rotation;
            Tiles = tiles;
        }
    }

    public readonly struct DeleteBuildingOnGridRequest
    {
        public readonly Building Building;
        
        public DeleteBuildingOnGridRequest(Building building)
        {
            Building = building;
        }
    }
    
    public readonly struct AddBuildingToStorageRequest
    {
        public readonly BuildingConfig BuildingConfig;
        
        public AddBuildingToStorageRequest(BuildingConfig buildingConfig)
        {
            BuildingConfig = buildingConfig;
        }
    }
    
    public readonly struct CreatedNewObjectRequest
    {
        public readonly LifetimeScope Scope;
        public readonly Vector3 Position;
        public readonly Vector3 Rotation;

        public CreatedNewObjectRequest(LifetimeScope scope, Vector3 position, Vector3 rotation)
        {
            Scope = scope;
            Position = position;
            Rotation = rotation;
        }
    }

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
    
    public readonly struct CreatedNewObjectOnGridMessage
    {
        public readonly Building Building;
        public readonly Transform Transform;
        public readonly Vector3 Position;
        public readonly Quaternion Rotation;

        public CreatedNewObjectOnGridMessage
            (
                Building building,
                Transform transform,
                Vector3 position,
                Quaternion rotation
            )
        {
            Building = building;
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

    public readonly struct ItemGivenFromInventory { }

    public readonly struct FruitHasGrown
    {
        public readonly Fruit Fruit;

        public FruitHasGrown(Fruit fruit)
        {
            Fruit = fruit;
        }
    }

    public readonly struct ItemHasBeenAddedToInventory { }

    public readonly struct MaterialsHasBeenMovedToProduction { }

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
        public readonly ShelfInfoRecorder ShelfInfoRecorder;
        public readonly ItemConfig ItemConfig;
        public readonly BuildingInteractableFlag BuildingInteractableFlag;
        public readonly IInventory Inventory;

        public NewShelfCreatedMessage
            (
                ShelfInfoRecorder shelfInfoRecorder,
                ItemConfig itemConfig,
                BuildingInteractableFlag buildingInteractableFlag,
                IInventory inventory
            )
        {
            ShelfInfoRecorder = shelfInfoRecorder;
            ItemConfig = itemConfig;
            BuildingInteractableFlag = buildingInteractableFlag;
            Inventory = inventory;
        }
    }

    public readonly struct ShelfDeletedMessage
    {
        public readonly ItemConfig ItemConfig;
        public readonly IInventory Inventory;

        public ShelfDeletedMessage
            (
                ItemConfig itemConfig,
                IInventory inventory
            )
        {
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
    
    public readonly struct CheckoutDeletedMessage
    {
        public readonly CheckoutController CheckoutController;
        
        public CheckoutDeletedMessage(CheckoutController checkoutController)
        {
            CheckoutController = checkoutController;
        }
    }
    
    public readonly struct PlaySoundMessage
    {
        public readonly SoundSettings SoundSettings;

        public PlaySoundMessage(SoundSettings soundSettings)
        {
            SoundSettings = soundSettings;
        }
    }
}