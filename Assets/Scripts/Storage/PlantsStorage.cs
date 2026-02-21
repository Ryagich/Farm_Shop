using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Landings.Plants;
using Landings.Plants.PlantConfigs;
using MessagePipe;
using Messages;
using UnityEngine;
using UnityEngine.AddressableAssets;
using VContainer.Unity;
using YG;

namespace Storage
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class PlantsStorage : IStartable
    {
        private readonly TaskCompletionSource<bool> readyTcs = new();

        public Task Ready => readyTcs.Task;
        
        public List<PlantInStorage> Plants { get; } = new();

        // быстрый доступ по Id
        private readonly Dictionary<string, PlantInStorage> plantsById = new();
        
        public PlantsStorage(ISubscriber<AddPlantToStorageRequest> addPlantToStorageRequest)
        {
            addPlantToStorageRequest.Subscribe(Add);
        }

        public PlantConfig GetPlantConfigById(string id)
        {
            return plantsById[id].PlantConfig;
        }
        
        public PlantInStorage Get(PlantConfig plantConfig)
        {
            if (plantConfig == null)
            {
                Debug.LogError("Storage.Get: PlantConfig is null");
                return null;
            }

            if (plantsById.TryGetValue(plantConfig.Id, out var plantInStorage))
            {
                plantInStorage.Count--;
                var plantInStorageSave = YG2.saves.PlantsInStorageSave
                                            .First(s => s.Id.Equals(plantInStorage.PlantConfig.Id));
                plantInStorageSave.Count = plantInStorage.Count;
                YG2.SaveProgress();
                return plantInStorage;
            }

            Debug.LogError($"Storage.Get: Plant with Id '{plantConfig.Id}' not found");
            return null;
        }
        
        public void Add(AddPlantToStorageRequest msg)
        {
            Debug.Log($"Add");
            if (msg.PlantConfig == null)
            {
                Debug.LogError("Storage.Add: PlantConfig is null");
                return;
            }

            if (!plantsById.TryGetValue(msg.PlantConfig.Id, out var plantInStorage))
            {
                Debug.LogError($"Storage.Add: Plant with Id '{msg.PlantConfig.Id}' not found in storage");
                return;
            }

            plantInStorage.Count++;
            Debug.Log($"Add add");

            if (msg.NeedSave)
            {
                Debug.Log($"Add NeedSave");
                var plantInStorageSave = YG2.saves.PlantsInStorageSave
                                            .First(s => s.Id.Equals(plantInStorage.PlantConfig.Id));
                plantInStorageSave.Count = plantInStorage.Count;
                YG2.SaveProgress();
            }
        }
        
        public IEnumerable<PlantInStorage> GetPlants(PlantType type)
        {
            foreach (var p in Plants)
                if (p.PlantConfig.Type == type)
                    yield return p;
        }

        public void Start()
        {
            LoadPlants(configs =>
                          {
                              Plants.Clear();
                              plantsById.Clear();

                              YG2.saves.PlantsInStorageSave ??= new List<PlantInStorageSave>();

                              foreach (var config in configs)
                              {
                                  if (string.IsNullOrEmpty(config.Id))
                                  {
                                      Debug.LogError($"PlantConfig '{config.name}' has empty Id");
                                      continue;
                                  }

                                  if (plantsById.ContainsKey(config.Id))
                                  {
                                      Debug.LogError($"Duplicate PlantConfig Id: {config.Id}");
                                      continue;
                                  }

                                  var entry = new PlantInStorage(config);
                                  Plants.Add(entry);
                                  plantsById.Add(config.Id, entry);

                                  var plantInStorageSave = YG2.saves.PlantsInStorageSave
                                                                 .FirstOrDefault(s => s.Id.Equals(entry.PlantConfig.Id));
                                  if (plantInStorageSave != null)
                                  {
                                      entry.Count = plantInStorageSave.Count;
                                  }
                                  else
                                  {
                                      YG2.saves.PlantsInStorageSave
                                         .Add(new PlantInStorageSave(entry.PlantConfig.Id, entry.Count));
                                  }
                              }

                              Plants.Sort((a, b) => a.PlantConfig.Price.CompareTo(b.PlantConfig.Price));

                              // StorageAwaiter.SignalReady();
                              readyTcs.SetResult(true);
                          });
        }
        
        private async void LoadPlants(Action<List<PlantConfig>> onComplete)
        {
            var handle = Addressables.LoadAssetsAsync<PlantConfig>("Plants", null);
            var results = await handle.Task;
            onComplete?.Invoke(results as List<PlantConfig>);
        }
    }
    
    [Serializable]
    public class PlantInStorage
    {
        public PlantConfig PlantConfig { get; }
        public int Count;

        public PlantInStorage(PlantConfig plantConfig)
        {
            PlantConfig = plantConfig;
            Count = 0;
        }
    }
}