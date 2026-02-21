using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BuildingsAndGrid.Buildings;
using GameModes;
using MessagePipe;
using Messages;
using UnityEngine;
using UnityEngine.AddressableAssets;
using VContainer.Unity;
using YG;

namespace Storage
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class BuildingsStorage : IStartable
    {
        private readonly TaskCompletionSource<bool> readyTcs = new();

        public Task Ready => readyTcs.Task;

        public List<BuildingInStorage> Buildings { get; } = new();

        private readonly Dictionary<string, BuildingInStorage> buildingsById = new();

        public BuildingsStorage(ISubscriber<AddBuildingToStorageRequest> addBuildingToStorageRequest)
        {
            addBuildingToStorageRequest.Subscribe(Add);
        }

        public BuildingConfig GetBuildingConfigById(string id)
        {
            return buildingsById[id].BuildingConfig;
        }

        public bool CheckId(string id)
        {
            return buildingsById.ContainsKey(id);
        }
        
        public BuildingInStorage Get(BuildingConfig buildingConfig)
        {
            if (buildingConfig == null)
            {
                Debug.LogError("Storage.Get: BuildingConfig is null");
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

            if (msg.NeedSave)
            {
                var buildingInStorageSave = YG2.saves.BuildingsInStorageSave
                                               .First(s => s.Id.Equals(buildingInStorage.BuildingConfig.Id));
                buildingInStorageSave.Count = buildingInStorage.Count;
                YG2.SaveProgress();
            }
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

                              YG2.saves.BuildingsInStorageSave ??= new List<BuildingInStorageSave>();

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

                                  var buildingInStorageSave = YG2.saves.BuildingsInStorageSave
                                                                 .FirstOrDefault(s => s.Id.Equals(entry.BuildingConfig
                                                                                   .Id));
                                  if (buildingInStorageSave != null)
                                  {
                                      entry.Count = buildingInStorageSave.Count;
                                  }
                                  else
                                  {
                                      YG2.saves.BuildingsInStorageSave
                                         .Add(new BuildingInStorageSave(entry.BuildingConfig.Id, entry.Count));
                                  }
                              }

                              Buildings.Sort((a, b) =>
                                                 a.BuildingConfig.Price.CompareTo(b.BuildingConfig.Price));

                              // StorageAwaiter.SignalReady();
                              readyTcs.SetResult(true);
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