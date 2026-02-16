using BuildingsAndGrid.Buildings;
using Buyer;
using Inventory.ObjectInventory;
using UniRx;
using UnityEngine;

namespace Shelf
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class InfoAboutPositionAtShelfForBuyer
    {
        public readonly TargetPoint TargetPoint;
        public readonly ShelfInventory ShelfInventory;
        public readonly BuildingInteractableFlag BuildingInteractableFlag;
        public readonly ReactiveProperty<bool> IsFree = new(true);

        public InfoAboutPositionAtShelfForBuyer
            (
                Transform transform,
                ShelfInventory shelfInventory, 
                BuildingInteractableFlag buildingInteractableFlag
            )
        {
            TargetPoint = new TargetPoint(transform);
            ShelfInventory = shelfInventory;
            BuildingInteractableFlag = buildingInteractableFlag;
        }
    }
}