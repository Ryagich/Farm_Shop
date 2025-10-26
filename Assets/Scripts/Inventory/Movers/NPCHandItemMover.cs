using Inventory.Item;
using UnityEngine;
using UnityEngine.AI;
using VContainer;
using VContainer.Unity;

namespace Inventory.Movers
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class NPCHandItemMover : ITickable
    {
        private readonly Transform place;
        private readonly ItemsConfig itemsConfig;
        private readonly IInventory inventory;
        private readonly NavMeshAgent agent;

        public NPCHandItemMover
            (
                ItemsConfig itemsConfig,
                IInventory inventory,
                NavMeshAgent agent,
                [Key("Hand")] Transform place
            )
        {
            this.itemsConfig = itemsConfig;
            this.inventory = inventory;
            this.agent = agent;
            this.place = place;
        }
        
        public void Tick()
        {
            if (inventory.Items.Count == 0)
                return;
            var height = .0f;
            for (var i = 0; i < inventory.Items.Count; i++)
            {
                var velocity = agent.velocity;
                var moveDirection = new Vector3(velocity.x, 0, velocity.z);
                var itemTrans = inventory.Items[i].transform;
                var position = place.position;
                position = new Vector3(position.x - (0.1f * i * moveDirection.z),
                                       position.y + height,
                                       position.z - (0.1f * i * moveDirection.z)
                                      );
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