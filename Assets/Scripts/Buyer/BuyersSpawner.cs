using MessagePipe;
using Messages;
using Storage;
using UniRx;
using UnityEngine;
using UnityEngine.AI;
using VContainer.Unity;

namespace Buyer
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class BuyersSpawner : IStartable, IFixedTickable
    {
        private readonly BuyerSettings buyerSettings;
        private readonly BuyerSpawnPoints buyerSpawnPoints;
        private readonly LifetimeScope parentScope;

        private float t;
        public ReactiveCollection<BuyerLifetimeScope> buyers = new();
        private bool isWorldReady;
        public BuyersSpawner
            (
                BuyerSettings buyerSettings,
                BuyerSpawnPoints buyerSpawnPoints,
                LifetimeScope parentScope,
                ISubscriber<BuyerIsOverMessage> BuyerIsOverSubscriber
            )
        {
            this.buyerSettings = buyerSettings;
            this.buyerSpawnPoints = buyerSpawnPoints;
            this.parentScope = parentScope;
            
            BuyerIsOverSubscriber.Subscribe(OnBuyerIsOver);
        }

        public async void Start()
        {
            await StorageAwaiter.WaitReadyAsync();
            isWorldReady = true;
        }

        public void FixedTick()
        {
            if (!isWorldReady)
                return;
            
            var targetTime = buyerSettings.TimeBetweenSpawnBuyers + buyerSettings.AddTimeForBuyer * buyers.Count;
            if (buyers.Count < buyerSettings.MaxBuyers 
              && t > targetTime
                //&& checkoutsController.GetMaxPositionCount() < buyers.Count
                )
            {
                t = .0f;
                InstantiateBuyer();
            }
            t += Time.fixedDeltaTime;
        }
        
        private void OnBuyerIsOver(BuyerIsOverMessage msg)
        {
            buyers.Remove(msg.BuyerLifetimeScope);
            Object.Destroy(msg.BuyerLifetimeScope.gameObject);
        }
        
        private void InstantiateBuyer()
        {
            if (buyerSpawnPoints.SpawnPoints.Count == 0)
            {
                Debug.LogWarning("Нет точек для спавна покупателей!");
                return;
            }
            var spawnPoint = buyerSpawnPoints.SpawnPoints[Random.Range(0, buyerSpawnPoints.SpawnPoints.Count)].Target.transform;
            var buyerScope = parentScope.CreateChildFromPrefab(buyerSettings.BuyerPrefabs[Random.Range(0, buyerSettings.BuyerPrefabs.Count)]);
            var agent = buyerScope.GetComponent<NavMeshAgent>();
            agent.enabled = false;
            buyerScope.transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
            agent.enabled = true;
            
            buyers.Add(buyerScope);
        }
    }
}