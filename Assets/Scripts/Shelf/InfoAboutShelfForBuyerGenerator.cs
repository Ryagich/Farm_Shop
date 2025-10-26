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
    // ReSharper disable once ClassNeverInstantiated.Global
    //PositionsAtShelfForBuyer использует ссылку на ShelvesController
    //Чтобы самого себя зарегистрировать. 
    //Хотел использовать сообщения, но полки находящиеся на сцене
    //отправляют собщения до появления ShelvesController
    public class InfoAboutShelfForBuyerGenerator : IStartable
    {
        public List<InfoAboutPositionAtShelfForBuyer> info;
        
        public InfoAboutShelfForBuyerGenerator
            (
                ItemConfig itemConfig,
                [Key("placesForBuyer")] List<Transform> places,
                ShelvesController shelvesController,
                IInventory inventory
            )
        {
            info = places.Select(place => new InfoAboutPositionAtShelfForBuyer(place, inventory)).ToList();

            shelvesController.OnNewShelfCreated(new NewShelfCreatedMessage(this, itemConfig, inventory));
        }
        
        public void Start() { }
    }
}