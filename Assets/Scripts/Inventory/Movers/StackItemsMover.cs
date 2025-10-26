using Inventory.Item;
using UnityEngine;
using Utils;
using VContainer.Unity;

namespace Inventory.Movers
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class StackItemsMover : ITickable
    {
        private readonly ItemsConfig itemsConfig;
        private readonly IInventory inventory;
        private readonly Transform place;

        public StackItemsMover
            (
                ItemsConfig itemsConfig,
                IInventory inventory,
                Transform place
            )
        {
            this.itemsConfig = itemsConfig;
            this.inventory = inventory;
            this.place = place;
        }

        public void Tick()
        {
            if (inventory.Items.Count == 0)
                return;
            var height = .0f;
            for (var i = 0; i < inventory.Items.Count; i++)
            {
                var itemTrans = inventory.Items[i].transform;
                var position = place.position;
                position = position.WithY(position.y + height);
                height += inventory.Items[i].Config.height;
                if (Vector3.Distance(itemTrans.position, position) < 0.01f)
                {
                    continue;
                }
                itemTrans.position = Vector3.Lerp(itemTrans.position, position, itemsConfig.MoveSpeed * (1 - Mathf.Pow(1 - 1.0f / (i + 11), itemsConfig.Gamma * Time.deltaTime)));
                itemTrans.rotation = Quaternion.RotateTowards(itemTrans.rotation, place.rotation, itemsConfig.MoveSpeed * Time.deltaTime);
            }
            
        }
    }
}