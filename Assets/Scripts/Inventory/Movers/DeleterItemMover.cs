using System.Diagnostics.CodeAnalysis;
using Inventory.Item;
using UnityEngine;
using VContainer.Unity;

namespace Inventory.Movers
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class DeleterItemMover : IFixedTickable
    {
        private readonly IInventory inventory;
        private readonly Transform place;
        private readonly ItemsConfig config;

        [SuppressMessage("ReSharper", "ParameterHidesMember")]
        public DeleterItemMover
            (
                Transform place,
                ItemsConfig config,
                IInventory inventory 
            )
        {
            this.inventory = inventory;
            this.place = place;
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
                var position = place.position;

                itemTrans.position = Vector3.MoveTowards(itemTrans.position, position, config.MoveSpeed * Time.deltaTime);
                itemTrans.rotation = Quaternion.RotateTowards(itemTrans.rotation, place.rotation, config.MoveSpeed * Time.deltaTime);
           
                if (Vector3.Distance(itemTrans.position, position) < 0.01f)
                {
                    inventory.Remove(item);
                    Object.Destroy(item.gameObject);
                }
            }
        }
    }
}