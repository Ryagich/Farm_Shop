using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inventory.Item;
using UnityEngine;
using UnityEngine.AddressableAssets;
using VContainer.Unity;

namespace Storage
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ItemsStorage : IStartable
    {
        private readonly TaskCompletionSource<bool> readyTcs = new();

        public Task Ready => readyTcs.Task;
        public List<ItemConfig> Items { get; } = new();
        
        public ItemConfig GetItemConfigById(string id)
        {
            return Items.First(item => item.Id.Equals(id));
        }
        
        public void Start()
        {
            LoadItems(configs =>
                      {
                          Items.Clear();

                          foreach (var config in configs)
                          {
                              if (string.IsNullOrEmpty(config.Id))
                              {
                                  Debug.LogError($"ItemConfig '{config.name}' has empty Id");
                                  continue;
                              }

                              if (Items.Contains(config) || Items.Any(c => c.Id.Equals(config.Id)))
                              {
                                  Debug.LogError($"Duplicate ItemConfig Id: {config.Id}");
                                  continue;
                              }

                              Items.Add(config);

                          }
                
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
}