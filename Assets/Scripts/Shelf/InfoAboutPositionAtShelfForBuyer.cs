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
        public bool IsFree;

        public InfoAboutPositionAtShelfForBuyer
            (
                Transform transform,
                IInventory shelfInventory, 
                bool isFree = true
            )
        {
            TargetPoint = new TargetPoint(transform);
            ShelfInventory = shelfInventory;
            IsFree = isFree;
        }
    }
}