using System.Collections.Generic;
using Inventory.Item;
using MessagePipe;
using Messages;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Products
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class MaterialsMoverToProduction : IFixedTickable
    {
        public bool IsMoving { get; private set; }

        private readonly ItemsConfig itemsConfig;
        private readonly Transform target;
        private readonly IPublisher<MaterialsHasBeenMovedToProduction> publisher;

        private List<ItemHolder> toMove = new();
        
        public MaterialsMoverToProduction
            (
                ItemsConfig itemsConfig,
                [Key("CreatorTransform")] Transform target,
                IPublisher<MaterialsHasBeenMovedToProduction> publisher
            )
        {
            this.itemsConfig = itemsConfig;
            this.target = target;
            this.publisher = publisher;
        }

        public void SetMoveObjects(IEnumerable<ItemHolder> newToMove)
        {
            toMove.AddRange(newToMove);
            IsMoving = true;
        }
        
        public void FixedTick()
        {
            if (!IsMoving)
                return;
            var toDestroy = new List<ItemHolder>();
            foreach (var holder in toMove)
            {
                var t = holder.transform;
                t.position = Vector3.MoveTowards(t.position, target.position, itemsConfig.MoveSpeed * Time.deltaTime);
                if (Vector3.Distance(holder.transform.position, target.position) < 0.01f)
                {
                    toDestroy.Add(holder);
                }
            }
            foreach (var toDestroyObj in toDestroy)
            {
                toMove.Remove(toDestroyObj);
                Object.Destroy(toDestroyObj.gameObject);
            }
            if (toMove.Count <= 0)
            {
                IsMoving = false;
                publisher.Publish(new MaterialsHasBeenMovedToProduction());
            }
        }
    }
}