using System.Collections.Generic;
using Inventory.Item;
using UnityEngine;

namespace Inventory.Movers
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class PlacesItemMover
    {
        private readonly IInventory inventory;
        // private readonly List<(Vector3, Quaternion)> places;
        private readonly List<Transform> places;
        private readonly ItemsConfig config;

        public PlacesItemMover
            (
                List<Transform> places,
                IInventory inventory,
                ItemsConfig config
            )
        {
            this.places = places;
            this.inventory = inventory;
            this.config = config;
        }
        
        public void Tick(float deltaTime)
        {
            if (inventory.Items.Count == 0)
                return;

            for (int i = 0; i < inventory.Items.Count; i++)
            {
                var item = inventory.Items[i];
                var target = places[i].position;

                item.transform.position =
                    Vector3.MoveTowards(item.transform.position, target, config.MoveSpeed * deltaTime);

                item.transform.rotation =
                    Quaternion.RotateTowards(item.transform.rotation, places[i].rotation, config.MoveSpeed * deltaTime);
            }
        }
    }
}