using System;
using System.Collections.Generic;
using System.Linq;
using BuildingsAndGrid.Buildings;
using GameModes;
using MessagePipe;
using Messages;
using UnityEngine.AddressableAssets;
using VContainer.Unity;

namespace Storage
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class Storage : IStartable
    {
        public List<BuildingInStorage> Buildings { get; } = new();

        public Storage(ISubscriber<AddBuildingToStorageRequest> addBuildingToStorageRequest)
        {
            addBuildingToStorageRequest.Subscribe(Add);
        }
        
        public BuildingInStorage Get(BuildingConfig buildingConfig)
        {
            var buildingInStorage = Buildings.First(b => b.BuildingConfig == buildingConfig);
            return buildingInStorage;
        }
        
        private void Add(AddBuildingToStorageRequest msg)
        {
            var buildingInStorage = Buildings.First(b => b.BuildingConfig == msg.BuildingConfig);
            buildingInStorage.Count++;
        }
        
        public IEnumerable<BuildingInStorage> GetBuildings(Area area)
        {
            foreach (var b in Buildings)
                if (b.BuildingConfig.Type == area)
                    yield return b;
        }
        
        public void Start()
        {
            LoadBuildings(configs =>
                          {
                              foreach (var config in configs)
                              {
                                  Buildings.Add(new BuildingInStorage(config));
                              }
                          });
        }
        
        private async void LoadBuildings(Action<List<BuildingConfig>> onComplete)
        {
            var handle = Addressables.LoadAssetsAsync<BuildingConfig>("Buildings", null);
            var results = await handle.Task;
            onComplete?.Invoke(results as List<BuildingConfig>);
        }
    }

    [Serializable]
    public class BuildingInStorage
    {
        public BuildingConfig BuildingConfig { get; private set; }
        public int Count;

        public BuildingInStorage(BuildingConfig buildingConfig)
        {
            BuildingConfig = buildingConfig;
            Count = 0;
        }
    }
}