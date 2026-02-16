using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BuildingsAndGrid.Buildings;
using Inventory.Item;
using MessagePipe;
using Messages;
using UnityEngine;
using UnityEngine.AddressableAssets;
using VContainer.Unity;
using YG;

namespace Storage
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ItemsStorage : IStartable
    {
        private readonly TaskCompletionSource<bool> readyTcs = new();

        public Task Ready => readyTcs.Task;
        public List<ItemInStorage> Items { get; } = new();
    
        // быстрый доступ по Id
        private readonly Dictionary<string, ItemInStorage> itemsById = new();
        
        public ItemsStorage(ISubscriber<AddItemToStorageRequest> addItemToStorageRequest)
        {
            addItemToStorageRequest.Subscribe(Add);
        }
        
        public ItemConfig GetItemConfigById(string id)
        {
            return itemsById[id].ItemConfig;
        }
        
        public ItemInStorage Get(ItemConfig itemConfig)
        {
            if (itemConfig == null)
            {
                Debug.LogError("Storage.Get: buildingConfig is null");
                return null;
            }

            if (itemsById.TryGetValue(itemConfig.Id, out var result))
                return result;

            Debug.LogError($"Storage.Get: Building with Id '{itemConfig.Id}' not found");
            return null;
        }
        
        private void Add(AddItemToStorageRequest msg)
        {
            if (msg.ItemConfig == null)
            {
                Debug.LogError("Storage.Add: BuildingConfig is null");
                return;
            }

            if (!itemsById.TryGetValue(msg.ItemConfig.Id, out var buildingInStorage))
            {
                Debug.LogError($"Storage.Add: Item with Id '{msg.ItemConfig.Id}' not found in storage");
                return;
            }

            buildingInStorage.Count++;
            if (msg.NeedSave)
            {
                var itemInStorageSave = YG2.saves.BuildingInStorageSave
                                           .First(s => s.Id.Equals(buildingInStorage.ItemConfig.Id));
                itemInStorageSave.Count = buildingInStorage.Count;
                YG2.SaveProgress();
            }
        }
        
        public void Start()
        {
            LoadItems(configs =>
                          {
                              Items.Clear();
                              itemsById.Clear();

                              YG2.saves.ItemInStorageSave ??= new List<ItemInStorageSave>();
                
                              foreach (var config in configs)
                              {
                                  if (string.IsNullOrEmpty(config.Id))
                                  {
                                      Debug.LogError($"BuildingConfig '{config.name}' has empty Id");
                                      continue;
                                  }

                                  if (itemsById.ContainsKey(config.Id))
                                  {
                                      Debug.LogError($"Duplicate ItemConfig Id: {config.Id}");
                                      continue;
                                  }

                                  var entry = new ItemInStorage(config);
                                  Items.Add(entry);
                                  itemsById.Add(config.Id, entry);

                                  var buildingInStorageSave = YG2.saves.BuildingInStorageSave
                                                                 .FirstOrDefault(s => s.Id.Equals(entry.ItemConfig.Id));
                                  if (buildingInStorageSave != null)
                                  {
                                      entry.Count = buildingInStorageSave.Count;
                                  }
                                  else
                                  {
                                      YG2.saves.ItemInStorageSave.Add(new ItemInStorageSave(entry.ItemConfig.Id, entry.Count));
                                  }
                              }
                
                              Items.Sort((a, b) =>
                                             a.ItemConfig.Price.CompareTo(b.ItemConfig.Price));
                              
                              readyTcs.SetResult(true);
                          });
        }
        
        private async void LoadItems(Action<List<ItemConfig>> onComplete)
        {
            var handle = Addressables.LoadAssetsAsync<ItemConfig>("Items", null);
            var results = await handle.Task;
            onComplete?.Invoke(results as List<ItemConfig>);
        }
    }
    
    [Serializable]
    public class ItemInStorage
    {
        public ItemConfig ItemConfig { get; }
        public int Count;

        public ItemInStorage(ItemConfig itemConfig)
        {
            ItemConfig = itemConfig;
            Count = 0;
        }
    }
}