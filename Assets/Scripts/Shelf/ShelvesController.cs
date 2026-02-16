using System.Collections.Generic;
using Inventory;
using Messages;

namespace Shelf
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ShelvesController
    {
        public readonly Dictionary<IInventory, List<InfoAboutPositionAtShelfForBuyer>> Shelves = new();

        public void RegisterShelf(NewShelfCreatedMessage msg)
        {
            if (!Shelves.ContainsKey(msg.ShelfInventory))
            {
                Shelves.Add(msg.ShelfInventory, new());
            }
            foreach (var pos in msg.ShelfInfoRecorder.info)
            {
                Shelves[msg.ShelfInventory].Add(pos);
            }
        }
        
        public void UnregisterShelf(ShelfDeletedMessage msg)
        {
            Shelves.Remove(msg.ShelfInventory);
        }
    }
}