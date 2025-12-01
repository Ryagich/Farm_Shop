using System.Collections.Generic;
using System.Linq;
using Inventory.Item;
using MessagePipe;
using Messages;
using UniRx;
using UnityEngine;
using Utils;
using VContainer.Unity;

namespace Objects
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ObjectMoverInHisPlace : IFixedTickable
    {
        private readonly List<(Transform, Vector3)> scopeTransforms = new();
        private readonly ItemsConfig itemsConfig;
        private readonly CompositeDisposable disposables = new();

        public ObjectMoverInHisPlace
            (
                ItemsConfig itemsConfig,
                ISubscriber<CreatedNewObjectMessage> playerMadePurchaseSubscriber,
                ISubscriber<CreatedNewObjectOnGridMessage> createdNewObjectOnGridSubscriber,
                ISubscriber<DeleteBuildingOnGridRequest> deleteBuildingOnGridRequestSubscriber
            )
        {
            playerMadePurchaseSubscriber.Subscribe(OnObjectCreated).AddTo(disposables);
            createdNewObjectOnGridSubscriber.Subscribe(OnObjectCreatedOnGrid);
            deleteBuildingOnGridRequestSubscriber.Subscribe(DeleteBuilding);
            
            this.itemsConfig = itemsConfig;
        }

        private void DeleteBuilding(DeleteBuildingOnGridRequest msg)
        {
            var el = scopeTransforms.FirstOrDefault(any => any.Item1 == msg.Building.transform);
            if (el != default && el.Item1)
            {
                scopeTransforms.Remove(el);
            }
        }
        
        private void OnObjectCreated(CreatedNewObjectMessage msg)
        {
            var position = msg.Position;
            msg.Transform.SetPositionAndRotation(position.WithY(position.y -10.0f), Quaternion.Euler(msg.Rotation));
            scopeTransforms.Add((msg.Transform, position));
        }

        private void OnObjectCreatedOnGrid(CreatedNewObjectOnGridMessage msg)
        {
            var position = msg.Position;
            msg.Transform.position = position.WithY(position.y -10.0f);
            msg.Building.SetContentRotation(msg.Rotation);
            scopeTransforms.Add((msg.Transform, position));
        }
        
        public void FixedTick()
        {
            if (scopeTransforms.Count is 0)
                return;
            var toRemove = new List<(Transform, Vector3)>();
            foreach (var scopeTransform in scopeTransforms)
            {
                scopeTransform.Item1.position =
                    Vector3.MoveTowards(scopeTransform.Item1.position, scopeTransform.Item2, itemsConfig.MoveSpeed * Time.deltaTime);
                if (Vector3.Distance(scopeTransform.Item1.position, scopeTransform.Item2) < 0.01f)
                {
                    scopeTransform.Item1.position = scopeTransform.Item2;
                    toRemove.Add(scopeTransform);
                }
            }
            foreach (var scopeTransform in toRemove)
            {
                scopeTransforms.Remove(scopeTransform);
            }
        }
    }
}