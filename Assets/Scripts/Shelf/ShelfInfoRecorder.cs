using System;
using System.Collections.Generic;
using System.Linq;
using Inventory;
using Inventory.Item;
using Messages;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Shelf
{
    //ReSharper disable once ClassNeverInstantiated.Global
    //PositionsAtShelfForBuyer использует ссылку на ShelvesController
    //Чтобы самого себя зарегистрировать. 
    //Хотел использовать сообщения, но полки находящиеся на сцене
    //отправляют собщения до появления ShelvesController
    public class ShelfInfoRecorder : IStartable, IDisposable
    {
        private readonly ItemConfig itemConfig;
        private readonly ShelvesController shelvesController;
        private readonly IInventory inventory;

        public readonly List<InfoAboutPositionAtShelfForBuyer> info;
        
        public ShelfInfoRecorder
            (
                ItemConfig itemConfig,
                [Key("placesForBuyer")] List<Transform> places,
                ShelvesController shelvesController,
                IInventory inventory
            )
        {
            this.itemConfig = itemConfig;
            this.shelvesController = shelvesController;
            this.inventory = inventory;
            info = places.Select(place => new InfoAboutPositionAtShelfForBuyer(place, inventory)).ToList();
        }
        
        public void Start()
        {
            shelvesController.RegisterShelf(new NewShelfCreatedMessage(this, itemConfig, inventory));
        }
        
        public void Dispose()
        {
            shelvesController.UnregisterShelf(new ShelfDeletedMessage(itemConfig, inventory));
        }
    }
}