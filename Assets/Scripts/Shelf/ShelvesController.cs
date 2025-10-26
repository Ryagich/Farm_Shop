using System.Collections.Generic;
using Inventory;
using Inventory.Item;
using Messages;

namespace Shelf
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ShelvesController
    {
        public Dictionary<ItemConfig, Dictionary<IInventory, List<InfoAboutPositionAtShelfForBuyer>>> PositionsAtShelvesByTypes = new();

        public void OnNewShelfCreated(NewShelfCreatedMessage msg)
        {
            if (!PositionsAtShelvesByTypes.ContainsKey(msg.ItemConfig))
            {
                PositionsAtShelvesByTypes.Add(msg.ItemConfig, new());
            }

            PositionsAtShelvesByTypes[msg.ItemConfig].Add(msg.Inventory, new());
            foreach (var pos in msg.InfoAboutShelfForBuyerGenerator.info)
            {
                PositionsAtShelvesByTypes[msg.ItemConfig][msg.Inventory].Add(pos);
            }
        }
    }
}