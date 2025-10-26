using System.Collections.Generic;
using System.Linq;
using Inventory.Item;
using Inventory.Movers;
using Inventory.ObjectInventory;
using MessagePipe;
using Messages;
using Objects;
using UniRx;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Products
{
    //TODO: Пиздец) Надо бить на классы... Я панк, кста
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ProductionZoneController : IFixedTickable
    {
        private readonly Interactable.Interactable interactable;
        private readonly CompositeDisposable disposables = new();
        private readonly ProductConfig productConfig;
        private readonly ItemsConfig itemsConfig;
        private readonly MaterialComposer materialComposer;
        private readonly MaterialsMoverToProduction materialsMoverToProduction;
        private readonly Transform transform;
        private readonly ProductCreator productCreator;
        private readonly GameObject materialHolderPrefab;
        private readonly IObjectResolver resolver;
        private readonly IPublisher<ItemGivenFromInventory> itemGivenFromInventoryMessage;
        public MaterialHolderInventory productionInventory { get; private set; }
        private PositionsItemMover positionsItemMover;
        
        public ProductionZoneController
            (
                ProductConfig productConfig,
                ItemsConfig itemsConfig,
                MaterialComposer materialComposer,
                MaterialsMoverToProduction materialsMoverToProduction,
                Transform transform,
                ProductCreator productCreator,
                Interactable.Interactable interactable,
                [Key("MaterialHolderPrefab")] GameObject materialHolderPrefab,
                IObjectResolver resolver,
                IPublisher<ItemGivenFromInventory> itemGivenFromInventoryMessage,
                ISubscriber<ItemHasBeenAddedToInventory> ItemHasBeenAddedToInventorySubscriber,
                ISubscriber<MaterialsHasBeenMovedToProduction> materialsHasBeenMovedToProductionSubscriber,
                ISubscriber<ProductCreated> ProductCreatedSubscriber
            )
        {
            this.interactable = interactable;
            this.productConfig = productConfig;
            this.itemsConfig = itemsConfig;
            this.materialComposer = materialComposer;
            this.materialsMoverToProduction = materialsMoverToProduction;
            this.transform = transform;
            this.productCreator = productCreator;
            this.materialHolderPrefab = materialHolderPrefab;
            this.resolver = resolver;
            this.itemGivenFromInventoryMessage = itemGivenFromInventoryMessage;

            ItemHasBeenAddedToInventorySubscriber.Subscribe(OnItemHasBeenAdded).AddTo(disposables);  
            materialsHasBeenMovedToProductionSubscriber.Subscribe(CreateProduct).AddTo(disposables);  
            ProductCreatedSubscriber.Subscribe(OnItemHasBeenCreated).AddTo(disposables);
           
            CreateProductionInventory();
        }

        private void CreateProductionInventory()
        {
            var children = new List<GameObject>();
            var places = new List<GameObject>();
            var holder = resolver.Instantiate(materialHolderPrefab);
            var holderT = holder.transform;
            holderT.SetPositionAndRotation(transform.position, transform.rotation);
            holderT.SetParent(transform);
            holderT.position -= holderT.right * productConfig.SpaceBetweenItemHolders;
            foreach (Transform child in holder.transform)
            {
                children.Add(child.gameObject);
            }
            var placesParent = children.First(c => c.name.ToUpper().Equals("Places".ToUpper()));
            foreach (Transform child in placesParent.transform)
            {
                places.Add(child.gameObject);
            }
            productionInventory = new MaterialHolderInventory(resolver,
                                                              productConfig.ItemConfig,
                                                              productConfig.MaxCount);
            positionsItemMover = new PositionsItemMover(places,
                                                        productConfig.MaxCount,
                                                        productionInventory,
                                                        itemsConfig,
                                                        productConfig.SpaceBetweenItemsY);
            // ReSharper disable once ObjectCreationAsStatement
            new ItemGiverFromInventory(interactable, itemGivenFromInventoryMessage, productionInventory);
        }
        
        private void OnItemHasBeenAdded(ItemHasBeenAddedToInventory msg)
        {
            TryCreateProduct();
        }

        private void OnItemHasBeenCreated(ProductCreated msg)
        {
            productionInventory.Add(msg.ItemConfig, msg.Position);
            TryCreateProduct();
        }
        
        private void CreateProduct(MaterialsHasBeenMovedToProduction msg)
        {
            productCreator.StartWork();
        }

        private void TryCreateProduct()
        {
            if (!materialComposer.CheckCanCreateProduct() 
             || productCreator.IsWorking 
             || materialsMoverToProduction.IsMoving 
             || !productionInventory.HaveFreePlace())
                return;
            var materials = materialComposer.GetMaterialsForProduct();
            materialsMoverToProduction.SetMoveObjects(materials);
        }
        
        public void FixedTick()
        {
            positionsItemMover.Tick();
        }
    }
}