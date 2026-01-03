using System;
using System.Collections.Generic;
using System.Linq;
using BuildingsAndGrid.Buildings;
using GameModes;
using MessagePipe;
using Messages;
using UnityEngine;
using UnityEngine.AddressableAssets;
using VContainer.Unity;

namespace Storage
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class Storage : IStartable
    {
        // основной контейнер
        public List<BuildingInStorage> Buildings { get; } = new();

        // быстрый доступ по Id
        private readonly Dictionary<string, BuildingInStorage> buildingsById = new();

        public Storage(ISubscriber<AddBuildingToStorageRequest> addBuildingToStorageRequest)
        {
            addBuildingToStorageRequest.Subscribe(Add);
        }

        public BuildingInStorage Get(BuildingConfig buildingConfig)
        {
            if (buildingConfig == null)
            {
                Debug.LogError("Storage.Get: buildingConfig is null");
                return null;
            }

            if (buildingsById.TryGetValue(buildingConfig.Id, out var result))
                return result;

            Debug.LogError($"Storage.Get: Building with Id '{buildingConfig.Id}' not found");
            return null;
        }

        private void Add(AddBuildingToStorageRequest msg)
        {
            if (msg.BuildingConfig == null)
            {
                Debug.LogError("Storage.Add: BuildingConfig is null");
                return;
            }

            if (!buildingsById.TryGetValue(msg.BuildingConfig.Id, out var buildingInStorage))
            {
                Debug.LogError($"Storage.Add: Building with Id '{msg.BuildingConfig.Id}' not found in storage");
                return;
            }

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
                Buildings.Clear();
                buildingsById.Clear();

                foreach (var config in configs)
                {
                    if (string.IsNullOrEmpty(config.Id))
                    {
                        Debug.LogError($"BuildingConfig '{config.name}' has empty Id");
                        continue;
                    }

                    if (buildingsById.ContainsKey(config.Id))
                    {
                        Debug.LogError($"Duplicate BuildingConfig Id: {config.Id}");
                        continue;
                    }

                    var entry = new BuildingInStorage(config);
                    Buildings.Add(entry);
                    buildingsById.Add(config.Id, entry);
                }

                Buildings.Sort((a, b) =>
                    a.BuildingConfig.Price.CompareTo(b.BuildingConfig.Price));
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
        public BuildingConfig BuildingConfig { get; }
        public int Count;

        public BuildingInStorage(BuildingConfig buildingConfig)
        {
            BuildingConfig = buildingConfig;
            Count = 0;
        }
    }
}
