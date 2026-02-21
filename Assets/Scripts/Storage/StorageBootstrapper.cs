using System.Threading.Tasks;
using UnityEngine;
using VContainer.Unity;
using YG;

namespace Storage
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class StorageBootstrapper : IStartable
    {
        private readonly BuildingsStorage buildingsStorage;
        private readonly ItemsStorage itemsStorage;
        private readonly PlantsStorage plantsStorage;

        private readonly TaskCompletionSource<bool> readyTcs = new();

        public Task Ready => readyTcs.Task;
        
        public StorageBootstrapper
            (
                BuildingsStorage buildingsStorage,
                ItemsStorage itemsStorage,
                PlantsStorage plantsStorage
            )
        {
            this.buildingsStorage = buildingsStorage;
            this.itemsStorage = itemsStorage;
            this.plantsStorage = plantsStorage;
        }

        public async void Start()
        {
            await buildingsStorage.Ready;
            await itemsStorage.Ready;
            await plantsStorage.Ready;

            readyTcs.SetResult(true);
            
            Debug.Log("All storages ready");
            if (!YG2.saves.StorageReadyMetricSend)
            {
                YG2.MetricaSend("StoragesReady");
                YG2.saves.StorageReadyMetricSend = true;
            }
        }
    }
}