using Inventory;
using UnityEngine;

namespace Shelf
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class InfoAboutPositionAtShelfForBuyer
    {
        public readonly Transform Transform;
        public readonly IInventory ShelfInventory;
        public bool IsFree;

        public InfoAboutPositionAtShelfForBuyer
            (
                Transform transform,
                IInventory shelfInventory, 
                bool isFree = true
            )
        {
            Transform = transform;
            ShelfInventory = shelfInventory;
            IsFree = isFree;
        }
    }
}