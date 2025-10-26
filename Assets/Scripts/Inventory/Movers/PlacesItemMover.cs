using System.Collections.Generic;
using Inventory.Item;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Inventory.Movers
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class PlacesItemMover : IFixedTickable
    {
        private readonly Transform transform;
        private readonly Vector3 startPos;
        private readonly IInventory inventory;
        private readonly List<(Vector3, Quaternion)> places;
        private readonly ItemsConfig config;

        public PlacesItemMover
            (
                Transform transform,
                [Key("StartPosition")] Vector3 startPos,
                [Key("places")] List<(Vector3, Quaternion)> places,
                IInventory inventory,
                ItemsConfig config
            )
        {
            this.transform = transform;
            this.startPos = startPos;
            this.places = places;
            this.inventory = inventory;
            this.config = config;
        }
        
        public void FixedTick()
        {
            if (inventory.Items.Count == 0)
                return;
            for (var i = 0; i < inventory.Items.Count; i++)
            {
                var item = inventory.Items[i];
                var itemTrans = item.transform;
                var offset = places[i].Item1 - startPos;
                var target = transform.position + offset;
                
                if (Vector3.Distance(itemTrans.position, target) < 0.01f)
                {
                    continue;
                }

                itemTrans.position =
                    Vector3.MoveTowards(itemTrans.position, target, config.MoveSpeed * Time.deltaTime);
                itemTrans.rotation =
                    Quaternion.RotateTowards(itemTrans.rotation, places[i].Item2, config.MoveSpeed * Time.deltaTime);
            }
        }
    }
}