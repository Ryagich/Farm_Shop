using BuildingsAndGrid.Buildings;
using Buyer;
using Inventory;
using UnityEngine;

namespace Shelf
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class InfoAboutPositionAtShelfForBuyer
    {
        public TargetPoint TargetPoint;
        public readonly IInventory ShelfInventory;
        public readonly BuildingInteractableFlag BuildingInteractableFlag;
        public bool IsFree;

        public InfoAboutPositionAtShelfForBuyer
            (
                Transform transform,
                IInventory shelfInventory, 
                BuildingInteractableFlag buildingInteractableFlag,
                bool isFree = true
            )
        {
            TargetPoint = new TargetPoint(transform);
            ShelfInventory = shelfInventory;
            BuildingInteractableFlag = buildingInteractableFlag;
            IsFree = isFree;
        }
    }
}