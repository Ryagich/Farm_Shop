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
                    Inventories[i].ChangeItemConfig(save.inventoriesInfo[i].Item1 is null
                                                        ? null
                                                        : itemsStorage
                                                           .GetItemConfigById(save.inventoriesInfo[i].Item1));
                }
            }
            else
            {
                Debug.Log($"YG2.saves.ShelvesSave {YG2.saves.ShelvesSave == null}");
                Debug.Log($"building.BuildingConfig.Id {building.BuildingConfig.Id}");

                var shelfSave = new ShelfSave(building.BuildingConfig.Id, building.Cell);
                foreach (var placesInventory in Inventories)
                {
                    shelfSave.inventoriesInfo.Add((placesInventory.GetConfig() is null 
                                                       ? null 
                                                       : placesInventory.GetConfig().Id, 0));
                }
                YG2.saves.ShelvesSave.Add(shelfSave);
            }
        }

        public bool CanAdd(ItemConfig config)
        {
            return Inventories.Any(i => i.CanAdd(config)); 
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
            return itemHolder;
        }

        public ItemHolder Get(ItemConfig config)
        {
            var itemHolder = Items.Last(i => i.Config.Id.Equals(config.Id));
            var inventory = Inventories.First(i => i.Items.Contains(itemHolder));
            Items.Remove(itemHolder);
            inventory.Remove(itemHolder);
            return itemHolder;
        }

        public void ChangeItemConfig(PlacesInventory placesInventory, ItemConfig itemConfig)
        {
            placesInventory.ChangeItemConfig(itemConfig);
            var index = Inventories.IndexOf(placesInventory);
            var save = YG2.saves.ShelvesSave.FirstOrDefault(s => s.Cell.Equals(building.Cell) 
                                                              && s.Id.Equals(building.BuildingConfig.Id));
            if (save != null)
            {
                save.inventoriesInfo[index] = (itemConfig.Id, 0);
            }
            else
            {
                Debug.LogError($"Полка работает, но не записана в сейвы | форшмак");
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