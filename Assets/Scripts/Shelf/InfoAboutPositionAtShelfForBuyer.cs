using BuildingsAndGrid.Buildings;
using Buyer;
using Inventory;
using UniRx;
using UnityEngine;

namespace Shelf
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class InfoAboutPositionAtShelfForBuyer
    {
        public TargetPoint TargetPoint;
        public readonly IInventory ShelfInventory;
        public readonly BuildingInteractableFlag BuildingInteractableFlag;
        public ReactiveProperty<bool> IsFree = new(true);

        public InfoAboutPositionAtShelfForBuyer
            (
                Transform transform,
                IInventory shelfInventory, 
                BuildingInteractableFlag buildingInteractableFlag
            )
        {
            TargetPoint = new TargetPoint(transform);
            ShelfInventory = shelfInventory;
            BuildingInteractableFlag = buildingInteractableFlag;
        }
    }
}