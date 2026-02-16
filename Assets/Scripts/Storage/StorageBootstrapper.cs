using System.Threading.Tasks;
using UnityEngine;
using VContainer.Unity;

namespace Storage
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class StorageBootstrapper : IStartable
    {
        private readonly BuildingsStorage buildingsStorage;
        private readonly ItemsStorage itemsStorage;

        private readonly TaskCompletionSource<bool> readyTcs = new();

        public Task Ready => readyTcs.Task;
        
        public StorageBootstrapper
            (
                BuildingsStorage buildingsStorage,
                ItemsStorage itemsStorage
            )
        {
            this.buildingsStorage = buildingsStorage;
            this.itemsStorage = itemsStorage;
        }

        public async void Start()
        {
            await buildingsStorage.Ready;
            await itemsStorage.Ready;

            readyTcs.SetResult(true);
            
            Debug.Log("All storages ready");
        }
    }
}