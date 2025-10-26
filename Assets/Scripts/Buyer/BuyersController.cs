using System.Collections.Generic;
using MessagePipe;
using Messages;
using NaughtyAttributes;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Random = UnityEngine.Random;

namespace Buyer
{
    public class BuyersController : LifetimeScope
    {
        [field: SerializeField] public BuyerLifetimeScope BuyerPrefab { get; private set; } = null!;
        [field: SerializeField] public Transform SpawnPointsParent { get; private set; } = null!;

        private readonly List<Transform> spawnPoints = new();

        protected override void Configure(IContainerBuilder builder)
        {
            foreach (Transform child in SpawnPointsParent)
                spawnPoints.Add(child);
            builder.RegisterInstance(spawnPoints).Keyed("SpawnPointsForBuyers");

            // === Local MessagePipe ===
            var options = builder.RegisterMessagePipe();
            builder.RegisterMessageBroker<BuyerIsOverMessage>(options);
            
            builder.Register<ShoppingListGenerator>(Lifetime.Singleton).AsSelf();

            builder.UseEntryPoints(ep =>
                                   {
                                       ep.Add<BuyersReleaser>();
                                   }); 
        }
    
        // ReSharper disable once UnusedMember.Local
        [Button("Создать покупателя")]
        private void InstantiateBuyer()
        {
            if (spawnPoints.Count == 0)
            {
                Debug.LogWarning("Нет точек для спавна покупателей!");
                return;
            }
            var spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
            var buyerScope = CreateChildFromPrefab(BuyerPrefab, _ => { });

            buyerScope.transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
        }
    }
}