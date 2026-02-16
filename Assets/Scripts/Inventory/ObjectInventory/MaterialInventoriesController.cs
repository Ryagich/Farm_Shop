using System.Collections.Generic;
using System.Linq;
using Inventory.Item;
using Inventory.Movers;
using MessagePipe;
using Messages;
using Products;
using UniRx;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Inventory.ObjectInventory
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class MaterialInventoriesController : IInventory, IFixedTickable
    {
        private readonly IPublisher<ItemHasBeenAddedToInventory> itemHasBeenAddedToInventoryPublisher;
        public List<MaterialHolderInventory> inventories { get; private set; } = new();
        private readonly List<PositionsItemMover> movers = new();

        public MaterialInventoriesController
            (
                IObjectResolver resolver,
                ProductConfig productConfig,
                [Key("MaterialHolderPrefab")] GameObject materialHolderPrefab,
                Transform transform,
                ItemsConfig itemsConfig,
                IPublisher<ItemHasBeenAddedToInventory> itemHasBeenAddedToInventoryPublisher
            )
        {
            this.itemHasBeenAddedToInventoryPublisher = itemHasBeenAddedToInventoryPublisher;
            for (var i = 0; i < productConfig.Materials.Count; i++)
            {
                var material = productConfig.Materials[i];
                var children = new List<GameObject>();
                var places = new List<GameObject>();
                var holder = resolver.Instantiate(materialHolderPrefab);
                var holderT = holder.transform;
                holderT.SetPositionAndRotation(transform.position, transform.rotation);
                holderT.SetParent(transform);
                holderT.position += holderT.right * (i + 1) * productConfig.SpaceBetweenItemHolders;
                    
                foreach (Transform child in holder.transform)
                {
                    children.Add(child.gameObject);
                }
                var placesParent = children.First(c => c.name.ToUpper().Equals("Places".ToUpper()));
                foreach (Transform child in placesParent.transform)
                {
                    places.Add(child.gameObject);
                }
                var inventory = new MaterialHolderInventory(resolver,
                                                            material.ItemConfig,
                                                            material.MaxCount);
                inventories.Add(inventory);
                movers.Add(new PositionsItemMover(places, 
                                                  material.MaxCount, 
                                                  inventory, itemsConfig, 
                                                  material.SpaceBetweenItemsY));
            }
        }

        public bool CanAdd(ItemConfig itemConfig) => inventories.Any(i => i.CanAdd(itemConfig));

        public void Add(ItemConfig config, Matrix4x4 position)
        {
            if (CanAdd(config))
            {
                var inventory = inventories.First(i => i.GetConfig() == config);
                inventory.Add(config, position);
                itemHasBeenAddedToInventoryPublisher.Publish(new ItemHasBeenAddedToInventory());
            }
        }

        public bool CanGet(ItemConfig itemConfig)
        {
            var inventory = inventories.FirstOrDefault(i => i.GetConfig().Id.Equals(itemConfig.Id));
            return inventory is not null && inventory.HaveItem;
        }

        public ItemHolder Get(ItemConfig itemConfig)
        {
            var inventory = inventories.First(i => i.GetConfig() == itemConfig);
            return inventory.Get();
        }
        
        public void FixedTick()
        {
            foreach (var mover in movers)
            {
                mover.Tick();
            }
        }

        public ReactiveCollection<ItemHolder> Items { get; } = new();

        public ItemConfig GetConfig()
        {
            throw new System.NotImplementedException();
        }

        public ItemHolder Get()
        {
            throw new System.NotImplementedException();
        }

        public void Remove(ItemHolder itemHolder)
        {
            throw new System.NotImplementedException();
        }
    }
}