using System.Collections.Generic;
using Inventory;
using Inventory.Item;
using Messages;

namespace Shelf
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ShelvesController
    {
        public Dictionary<string, Dictionary<IInventory, List<InfoAboutPositionAtShelfForBuyer>>> PositionsAtShelvesByTypes = new();

        public void RegisterShelf(NewShelfCreatedMessage msg)
        {
            if (!PositionsAtShelvesByTypes.ContainsKey(msg.ItemConfig.ID))
            {
                PositionsAtShelvesByTypes.Add(msg.ItemConfig.ID, new());
            }

            PositionsAtShelvesByTypes[msg.ItemConfig.ID].Add(msg.Inventory, new());
            foreach (var pos in msg.ShelfInfoRecorder.info)
            {
                PositionsAtShelvesByTypes[msg.ItemConfig.ID][msg.Inventory].Add(pos);
            }
        }
        
        public void UnregisterShelf(ShelfDeletedMessage msg)
        {
            PositionsAtShelvesByTypes[msg.ItemConfig.ID].Remove(msg.Inventory);
        }
    }
}