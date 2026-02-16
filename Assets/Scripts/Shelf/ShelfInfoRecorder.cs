using System;
using System.Collections.Generic;
using System.Linq;
using BuildingsAndGrid.Buildings;
using Inventory.ObjectInventory;
using Messages;
using UniRx;
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
        private readonly ShelvesController shelvesController;
        private readonly ShelfInventory shelfInventory;
        private readonly BuildingInteractableFlag buildingInteractableFlag;

        public readonly ReactiveCollection<InfoAboutPositionAtShelfForBuyer> info;
        
        public ShelfInfoRecorder
            (
                [Key("placesForBuyer")] List<Transform> places,
                ShelvesController shelvesController,
                ShelfInventory shelfInventory,
                BuildingInteractableFlag buildingInteractableFlag
            )
        {
            this.shelvesController = shelvesController;
            this.buildingInteractableFlag = buildingInteractableFlag;
            this.shelfInventory = shelfInventory;
            
            info = new ReactiveCollection<InfoAboutPositionAtShelfForBuyer>(places.Select(place => 
                                                                            new InfoAboutPositionAtShelfForBuyer(place, shelfInventory, buildingInteractableFlag)));
        }
        
        public void Start()
        {
            Register();
        }
        
        public void Dispose()
        {
            UnRegister();
        }

        public void Register()
        {
            shelvesController.RegisterShelf(new NewShelfCreatedMessage(this, shelfInventory, buildingInteractableFlag));
        }
        
        public void UnRegister()
        {
            shelvesController.UnregisterShelf(new ShelfDeletedMessage(shelfInventory));
        }
    }
}