using System.Collections.Generic;
using Inventory.Item;
using UnityEngine;

namespace Inventory.Movers
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class PositionsItemMover
    {
        private readonly IInventory inventory;
        private readonly ItemsConfig config;
        
        private readonly List<Vector3> positions = new();

        public PositionsItemMover
            (
                List<GameObject> places,
                int placesCount,
                IInventory inventory,
                ItemsConfig config,
                float layerOffset = 1f // насколько поднимать каждый слой
            )
        {
            this.inventory = inventory;
            this.config = config;

            var baseCount = places.Count;
            
            for (var i = 0; i < placesCount; i++)
            {
                var layerIndex = i / baseCount;
                var localIndex = i % baseCount;
                var basePos = places[localIndex].transform.position;
                var finalPos = basePos + Vector3.up * layerIndex * layerOffset;
                positions.Add(finalPos);
            }
        }

        public void Tick()
        {
            if (inventory.Items.Count == 0)
                return;
            for (var i = 0; i < inventory.Items.Count; i++)
            {
                var item = inventory.Items[i];
                var itemTrans = item.transform;
                var position = positions[i];

                if (Vector3.Distance(itemTrans.position, position) < 0.01f)
                {
                    continue;
                }

                itemTrans.position =
                    Vector3.MoveTowards(itemTrans.position, position, config.MoveSpeed * Time.deltaTime);
            }
        }
    }
}