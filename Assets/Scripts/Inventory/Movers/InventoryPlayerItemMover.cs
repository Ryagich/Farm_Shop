using Inventory.Item;
using MessagePipe;
using Messages;
using UniRx;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Inventory.Movers
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class InventoryPlayerItemMover : ITickable
    {
        private readonly Camera cam;
        private readonly IInventory inventory;
        private readonly ItemsConfig config;
        private readonly Transform place;

        private Vector2 velocity;
        private readonly CompositeDisposable disposables = new();

        public InventoryPlayerItemMover
            (
                Camera cam, 
                [Key("Hand")] Transform place,
                ItemsConfig config,
                ISubscriber<PlayerMoveMessage> subscriber,
                IInventory inventory 
            )
        {
            this.cam = cam;
            this.inventory = inventory;
            this.place = place;

            this.config = config;
            subscriber.Subscribe(OnMove).AddTo(disposables);  
        }
        
        private void OnMove(PlayerMoveMessage msg)
        {
            velocity = msg.Direction;
        }
        
        public void Tick()
        {
            if (inventory.Items.Count == 0)
                return;
            var height = .0f;
            for (var i = 0; i < inventory.Items.Count; i++)
            {
                var moveDirection = Quaternion.Euler(0, cam.transform.rotation.eulerAngles.y, 0) *
                                    new Vector3(velocity.x, 0, velocity.y);
                var itemTrans = inventory.Items[i].transform;
                var position = place.position;
                position = new Vector3(position.x - (0.1f * i * moveDirection.z),
                                       position.y + height,
                                       position.z - (0.1f * i * moveDirection.z)
                                      );
                height += inventory.Items[i].Config.height;
                
                if (Vector3.Distance(itemTrans.position, position) < 0.01f)
                {
                    // itemTrans.SetParent(place);
                    continue;
                }
                //TODO: Нормальные формулы написаит перемещения предметов.
                //TODO: разделить механики полета в руки и к руке
                // itemTrans.position = Vector3.MoveTowards(itemTrans.position, position, 
                //                                          config.MoveSpeed * Time.deltaTime/ Mathf.Pow((i+1),config.Gamma));
                itemTrans.position = Vector3.Lerp(itemTrans.position, position, config.MoveSpeed * (1 - Mathf.Pow(1 - 1.0f / (i + 11), config.Gamma * Time.deltaTime)));
                itemTrans.rotation = Quaternion.RotateTowards(itemTrans.rotation, place.rotation, config.MoveSpeed * Time.deltaTime);
            }
        }
    }
}