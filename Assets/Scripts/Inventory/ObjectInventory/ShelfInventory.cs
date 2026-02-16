using System.Collections.Generic;
using System.Linq;
using BuildingsAndGrid.Buildings;
using Inventory.Item;
using Inventory.Movers;
using Shelf;
using Storage;
using UniRx;
using UnityEngine;
using VContainer.Unity;
using YG;

namespace Inventory.ObjectInventory
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ShelfInventory : IInventory, IStartable, IFixedTickable
    {
        private readonly ItemsStorage itemsStorage;
        private readonly Building building;
        public ReactiveCollection<ItemHolder> Items { get; private set; } = new();

        public readonly List<PlacesInventory> Inventories = new();
        private readonly List<PlacesItemMover> movers = new();

        public ShelfInventory
            (
                ItemsConfig itemsConfig,
                ItemsStorage itemsStorage,
                List<GoodPlacesOnShelf> goodPlacesOnShelf,
                Building building
            )
        {
            this.itemsStorage = itemsStorage;
            this.building = building;
            
            foreach (var goodPlaces in goodPlacesOnShelf)
            {
                var inventory = new PlacesInventory(null, goodPlaces.Places.Count);
                var mover = new PlacesItemMover(goodPlaces.Places, inventory, itemsConfig);
                Inventories.Add(inventory);
                movers.Add(mover);
            }
        }

        public void Start()
        {
            var save = YG2.saves.ShelvesSave.FirstOrDefault(s => s.Cell.Equals(building.Cell) 
                                                              && s.Id.Equals(building.BuildingConfig.Id));
            if (save != null)
            {
                for (var i = 0; i < save.inventoriesInfo.Count; i++)
                {
                    var id = save.inventoriesInfo[i].Item1;
                    if (id is null)
                    {
                        Inventories[i].ChangeItemConfig(null);
                    }
                    else
                    {
                        var config = itemsStorage.GetItemConfigById(save.inventoriesInfo[i].Item1);
                        Inventories[i].ChangeItemConfig(config);
                        for (var j = 0; j < save.inventoriesInfo[i].Item2; j++)
                        {
                            AddFromSave(config, building.Content.localToWorldMatrix);
                        }
                    }
                }
            }
            else
            {
                var shelfSave = new ShelfSave(building.BuildingConfig.Id, building.Cell);
                foreach (var placesInventory in Inventories)
                {
                    shelfSave.inventoriesInfo.Add((placesInventory.GetConfig() is null 
                                                       ? null 
                                                       : placesInventory.GetConfig().Id, 
                                                   placesInventory.Items.Count));
                }
                YG2.saves.ShelvesSave.Add(shelfSave);
            }
        }

        public bool CanAdd(ItemConfig config)
        {
            return Inventories.Any(i => i.CanAdd(config)); 
        }
        
        private void AddFromSave(ItemConfig config, Matrix4x4 position)
        {
            var inventory = Inventories.FirstOrDefault(i => i.CanAdd(config));
            if (inventory != null)
            {
                var handItem = Object.Instantiate(config.HandPrefab);
                handItem.transform.SetPositionAndRotation(position.GetPosition(), position.rotation);
                Items.Add(handItem);
                inventory.Add(handItem);
            }
        }
        
        public void Add(ItemConfig config, Matrix4x4 position)
        {
            var inventory = Inventories.FirstOrDefault(i => i.CanAdd(config));
            if (inventory != null)
            {
                var handItem = Object.Instantiate(config.HandPrefab);
                handItem.transform.SetPositionAndRotation(position.GetPosition(), position.rotation);
                Items.Add(handItem);
                inventory.Add(handItem);
                
                SaveItemsCount(inventory, config);
            }
        }

        public bool CanGet(ItemConfig config)
        {
            return Inventories.Any(i => i.CanGet(config));
        }

        public ItemConfig GetConfig()
        {
            throw new System.NotImplementedException();
        }

        public ItemHolder Get()
        {
            var itemHolder = Items.Last();
            var inventory = Inventories.First(i => i.Items.Contains(itemHolder));
            Items.Remove(itemHolder);
            inventory.Remove(itemHolder);
           
            SaveItemsCount(inventory, itemHolder.Config);
            
            return itemHolder;
        }

        public ItemHolder Get(ItemConfig config)
        {
            var itemHolder = Items.Last(i => i.Config.Id.Equals(config.Id));
            var inventory = Inventories.First(i => i.Items.Contains(itemHolder));
            Items.Remove(itemHolder);
            inventory.Remove(itemHolder);

            SaveItemsCount(inventory, config);
            
            return itemHolder;
        }

        public void ChangeItemConfig(PlacesInventory placesInventory, ItemConfig itemConfig)
        {
            placesInventory.ChangeItemConfig(itemConfig);
            var index = Inventories.IndexOf(placesInventory);
            var shelfSave = YG2.saves.ShelvesSave.FirstOrDefault(s => s.Cell.Equals(building.Cell) 
                                                              && s.Id.Equals(building.BuildingConfig.Id));
            if (shelfSave != null)
            {
                shelfSave.inventoriesInfo[index] = (itemConfig.Id, 0);
            }
            else
            {
                Debug.LogError($"Полка работает, но не записана в сейвы | форшмак");
            }
        }

        private void SaveItemsCount(PlacesInventory inventory, ItemConfig config)
        {
            var shelfSave = YG2.saves.ShelvesSave.FirstOrDefault(s => s.Cell.Equals(building.Cell) 
                                                          && s.Id.Equals(building.BuildingConfig.Id));
            if (shelfSave != null)
            {
                var index = Inventories.IndexOf(inventory);
                shelfSave.inventoriesInfo[index] = (config.Id, inventory.Items.Count);
            }
        }
        
        public void Remove(ItemHolder itemHolder)
        {
            Items.Remove(itemHolder);
        }

        public void FixedTick()
        {
            foreach (var mover in movers)
            {
                mover.Tick(Time.fixedDeltaTime);
            }
        }
    }
}