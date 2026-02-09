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
        public readonly Vector2Int Cell;
        public readonly Vector2Int LastCell;

        public CreatedNewBuildingOnGridRequest
            (
                BuildingConfig buildingConfig,
                Vector3 position,
                Vector3 localPosition,
                Quaternion rotation,
                List<Tile> tiles,
                Vector2Int cell,
                Vector2Int lastCell
            )
        {
            BuildingConfig = buildingConfig;
            Position = position;
            LocalPosition = localPosition;
            Rotation = rotation;
            Tiles = tiles;
            Cell = cell;
            LastCell = lastCell;
        }
    }

    public readonly struct CreatedNewObjectOnGridMessage
    {
        public readonly Building Building;
        public readonly Transform Transform;
        public readonly Vector3 Position;
        public readonly Quaternion Rotation;
        public readonly Vector2Int Cell;
        public readonly Vector2Int LastCell;

        public CreatedNewObjectOnGridMessage
            (
                Building building,
                Transform transform,
                Vector3 position,
                Quaternion rotation,
                Vector2Int cell,
                Vector2Int lastCell
            )
        {
            Building = building;
            Transform = transform;
            Position = position;
            Rotation = rotation;
            Cell = cell;
            LastCell = lastCell;
        }
    }
    
    public readonly struct DeleteBuildingOnGridRequest
    {
        public readonly Building Building;
        public readonly bool NeedRemoveFromSave;
        public readonly Vector2Int OldCell;

        public DeleteBuildingOnGridRequest(Building building, bool needRemoveFromSave, Vector2Int oldCell)
        {
            Building = building;
            NeedRemoveFromSave = needRemoveFromSave;
            OldCell = oldCell;
        }
    }
    
    public readonly struct DeleteBuildingOnGridMessage
    {
        public readonly string ID;
        public readonly Vector2Int Cell;
        
        public DeleteBuildingOnGridMessage(string id, Vector2Int cell)
        {
            ID = id;
            Cell = cell;
        }
    }
    
    public readonly struct AddBuildingToStorageRequest
    {
        public readonly BuildingConfig BuildingConfig;
        public readonly bool NeedRemoveFromSave;

        public AddBuildingToStorageRequest(BuildingConfig buildingConfig, bool needRemoveFromSave)
        {
            BuildingConfig = buildingConfig;
            NeedRemoveFromSave = needRemoveFromSave;
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

    public readonly struct ObjectInHisPlaceMessage
    {
        public readonly Transform Transform;

        public ObjectInHisPlaceMessage(Transform transform)
        {
            Transform = transform;
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
        public readonly Vector3 Position;
        public readonly Transform Parent;
        
        public PlaySoundMessage(SoundSettings soundSettings, Vector3 position, Transform parent)
        {
            SoundSettings = soundSettings;
            Position = position;
            Parent = parent;
        }
    }
}