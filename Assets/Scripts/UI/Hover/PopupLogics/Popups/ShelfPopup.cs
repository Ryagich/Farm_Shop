using System;
using System.Collections.Generic;
using System.Linq;
using BuildingsAndGrid.Buildings;
using GameModes;
using Inventory.Item;
using Inventory.ObjectInventory;
using Localization;
using MessagePipe;
using Messages;
using Shelf;
using Storage;
using TMPro;
using UI.Cards;
using UI.Configs;
using UI.Hover.PopupLogics.Holders;
using UnityEngine;
using Object = UnityEngine.Object;
using UniRx;
using UnityEngine.UI;
using Utils;

namespace UI.Hover.PopupLogics.Popups
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ShelfPopup : IObjectPopup, IDisposable
    {
        public event Action CloseButton;
        public RectTransform Root { get; private set; }
        public List<RectTransform> Children { get; private set; } = new();

        private readonly UIConfig uiConfig;
        private readonly LocalizationConfig localizationConfig;
        private readonly ShelfInventory shelfInventory;
        private readonly PopupHolders popupHolders;
        private readonly ItemsStorage itemsStorage;
        private readonly ShelfInfoRecorder shelfInfoRecorder;
        private readonly Building building;
        private readonly BuildingInteractableFlag buildingInteractableFlag;

        private readonly IPublisher<ChoseBuildingMessage> choseBuildingMessagePublisher;
        private readonly IPublisher<DeleteBuildingOnGridRequest> deleteBuildingOnGridPublisher;
        private readonly IPublisher<AddBuildingToStorageRequest> addBuildingToStoragePublisher;
        private readonly IPublisher<ChangeGameModeRequest> changeGameModeRequestPublisher;
        
        private CompositeDisposable disposables = new();
        
        private WindowToChangeProduct windowToChangeProduct;
        private readonly List<ProductCard> productCards = new();
        
        public ShelfPopup
            (
                UIConfig uiConfig,
                LocalizationConfig localizationConfig,
                PopupHolders popupHolders,
                ItemsStorage itemsStorage,
                ShelfInventory shelfInventory,
                ShelfInfoRecorder shelfInfoRecorder,
                Building building,
                BuildingInteractableFlag buildingInteractableFlag
            )
        {
            this.uiConfig = uiConfig;
            this.localizationConfig = localizationConfig;
            this.shelfInventory = shelfInventory;
            this.popupHolders = popupHolders;
            this.itemsStorage = itemsStorage;
            this.shelfInfoRecorder = shelfInfoRecorder;
            this.building = building;
            this.buildingInteractableFlag = buildingInteractableFlag;
            
            choseBuildingMessagePublisher = GlobalMessagePipe.GetPublisher<ChoseBuildingMessage>();
            deleteBuildingOnGridPublisher = GlobalMessagePipe.GetPublisher<DeleteBuildingOnGridRequest>();
            addBuildingToStoragePublisher = GlobalMessagePipe.GetPublisher<AddBuildingToStorageRequest>();
            changeGameModeRequestPublisher = GlobalMessagePipe.GetPublisher<ChangeGameModeRequest>();
        }
        
        public IObjectPopup DrawPopup(Canvas canvas)
        {
            var popup = Object.Instantiate(popupHolders.ShelfPopupHolder, canvas.transform);
            Root = popup.GetComponent<RectTransform>();
            disposables = new CompositeDisposable();

            popup.ButtonMove.onClick.AddListener(Move);
            popup.ButtonMoveToInventory.onClick.AddListener(MoveInInventory);
            popup.ButtonDisable.onClick.AddListener(ChangeInteractableState); 
            
            Redraw();
            Subscribe();
            
            return this;
        }
        
        public void Redraw()
        {
            if (!Root)
                return;
            var popup = Root.GetComponent<ShelfPopupHolder>();

            var total = shelfInfoRecorder.info.Count;
            var busy = total - shelfInfoRecorder.info.Count(i => i.IsFree.Value);

            foreach (var productCard in productCards)
            {
                if (productCard)
                {
                    Object.Destroy(productCard.gameObject);
                }
            }
            productCards.Clear();
            foreach (var inventory in shelfInventory.Inventories)
            {
                var productCard = Object.Instantiate(uiConfig.ProductCard, popup.CardParent);
                var buttonText = productCard.Button.GetComponentInChildren<TMP_Text>();
                var itemConfig = inventory.GetConfig(); 
                
                if (itemConfig != null)
                {
                    productCard.Icon.sprite = itemConfig.Icon;
                    productCard.Name.text = itemConfig.Name.GetLocalizedStringCached();
                    productCard.InInventory.text = $"{inventory.Items.Count}/{inventory.MaxItems}";
                    productCard.Button.onClick.AddListener(() => OpenProductChangedWindow(
                                                            productCard.Button.GetComponent<RectTransform>(), 
                                                            inventory, itemConfig));
                    if (inventory.Items.Count > 0)
                    {
                        productCard.Button.interactable = false;
                    }
                }
                else
                {

                    productCard.Name.text = localizationConfig.Empty.GetLocalizedString();
                    productCard.InInventory.text = $"";
                    productCard.Button.onClick.AddListener(() => OpenProductChangedWindow(
                                                            productCard.Button.GetComponent<RectTransform>(),
                                                            inventory, null));
                }
                buttonText.text = localizationConfig.Edit.GetLocalizedString();
                
                productCards.Add(productCard);
            }
            var cardWidth = uiConfig.ProductCard.GetComponent<RectTransform>().rect.width;
            var lg = popup.CardParent.GetComponent<HorizontalLayoutGroup>();
            var cardsCount = productCards.Count; 
            var contentSize = lg.padding.left + lg.padding.right + lg.spacing * (cardsCount - 1) + cardWidth * cardsCount;
            popup.CardParent.anchorMin = Vector2.up;
            popup.CardParent.anchorMax = Vector2.up;
            popup.CardParent.pivot = Vector2.up;
            popup.CardParent.sizeDelta = popup.CardParent.sizeDelta.WithX(contentSize);
            
            popup.BuyersCount.text = $"{busy} / {total}";
            popup.BuyersFillImage.fillAmount = total == 0 ? 0f : (float)busy / total;

            var allFree = shelfInfoRecorder.info.All(i => i.IsFree.Value);
            popup.ButtonMove.interactable = allFree;
            popup.ButtonMoveToInventory.interactable = allFree;
            
            popup.ButtonDisable.GetComponentInChildren<TMP_Text>().text = buildingInteractableFlag.IsInteractable 
                                                                        ? $"{localizationConfig.DisableWord.GetLocalizedStringCached()}" 
                                                                        : $"{localizationConfig.ActivateWord.GetLocalizedStringCached()}";
        }
        
        private void OpenProductChangedWindow(RectTransform rectTransform, PlacesInventory placesInventory, ItemConfig itemConfig)
        {
            if (Root && windowToChangeProduct)
            {
                var wr = windowToChangeProduct.GetComponent<RectTransform>();
                if (Children.Contains(wr))
                {
                    Children.Remove(wr);
                }
                Object.Destroy(windowToChangeProduct.gameObject);
            }
            
            windowToChangeProduct = Object.Instantiate(uiConfig.WindowToChangeProduct, rectTransform);
            windowToChangeProduct.ButtonToClose.onClick.AddListener(CloseProductChangedWindow);
            
            var windowRect = windowToChangeProduct.GetComponent<RectTransform>();
            windowRect.anchoredPosition = windowRect.anchoredPosition.WithY(windowRect.anchoredPosition.y - rectTransform.rect.size.y/2);
            windowRect.SetParent(Root);
            Children.Add(windowRect);

            foreach (var itemInStorage in itemsStorage.Items)
            {
                var productCard = Object.Instantiate(uiConfig.ProductCard, windowToChangeProduct.Content);
                var buttonText = productCard.Button.GetComponentInChildren<TMP_Text>();
                productCard.Icon.sprite = itemInStorage.ItemConfig.Icon;
                productCard.Name.text = itemInStorage.ItemConfig.Name.GetLocalizedStringCached();

                if (itemConfig != null && itemConfig.Id.Equals(itemInStorage.ItemConfig.Id))
                {
                    productCard.InInventory.text = $"";
                    buttonText.text = localizationConfig.Selected.GetLocalizedString(); //Выбрано
                    productCard.Button.interactable = false;
                }
                else
                {
                    productCard.InInventory.text = $"";
                    buttonText.text = localizationConfig.Select.GetLocalizedString(); //Выбрать
                    productCard.Button.onClick.AddListener(() => ReRegister(placesInventory, itemInStorage.ItemConfig));
                }
            }
            var cardWidth = uiConfig.ProductCard.GetComponent<RectTransform>().rect.width;
            var lg = windowToChangeProduct.Content.GetComponent<HorizontalLayoutGroup>();
            var cardsCount = itemsStorage.Items.Count; 
            var contentSize = lg.padding.left + lg.padding.right + (lg.spacing * (cardsCount - 1)) + (cardWidth * cardsCount);
            windowToChangeProduct.Content.anchorMin = Vector2.up;
            windowToChangeProduct.Content.anchorMax = Vector2.up;
            windowToChangeProduct.Content.pivot = Vector2.up;

            windowToChangeProduct.Content.sizeDelta = windowToChangeProduct.Content.sizeDelta.WithX(contentSize);
        }

        private void CloseProductChangedWindow()
        {
            foreach (var child in Children)
            {
                Children.Remove(child);
            }
            Children.Clear();
            if (Root && windowToChangeProduct)
            {
                Object.Destroy(windowToChangeProduct.gameObject);
            }
            windowToChangeProduct = null;
            Redraw();
        }
        
        private void ReRegister(PlacesInventory placesInventory, ItemConfig itemConfig)
        {
            shelfInventory.ChangeItemConfig(placesInventory, itemConfig);
            Redraw();
            if (Root && windowToChangeProduct)
            {
                var wr = windowToChangeProduct.GetComponent<RectTransform>();
                if (Children.Contains(wr))
                {
                    Children.Remove(wr);
                }
                Object.Destroy(windowToChangeProduct.gameObject);
            }
        }
        
        public void Subscribe()
        {
            shelfInfoRecorder.info
                             .ObserveAdd()
                             .Subscribe(e =>
                                        {
                                            e.Value.IsFree
                                             .Subscribe(_ => Redraw())
                                             .AddTo(disposables);
                                            Redraw();
                                        })
                             .AddTo(disposables);

            shelfInfoRecorder.info
                             .ObserveRemove()
                             .Subscribe(_ => Redraw())
                             .AddTo(disposables);

            foreach (var item in shelfInfoRecorder.info)
            {
                item.IsFree
                    .Subscribe(_ => Redraw())
                    .AddTo(disposables);
            }

            shelfInventory.Items
                     .ObserveAdd()
                     .Subscribe(_ => Redraw())
                     .AddTo(disposables);
            shelfInventory.Items
                     .ObserveRemove()
                     .Subscribe(_ => Redraw())
                     .AddTo(disposables);
        }

        private void MoveInInventory()
        {
            deleteBuildingOnGridPublisher.Publish(new DeleteBuildingOnGridRequest(building, true, building.Cell));
            addBuildingToStoragePublisher.Publish(new AddBuildingToStorageRequest(building.BuildingConfig, true));
            CloseButton?.Invoke();
            Dispose();
        }
        
        private void Move()
        {
            deleteBuildingOnGridPublisher.Publish(new DeleteBuildingOnGridRequest(building,false, building.Cell));
            addBuildingToStoragePublisher.Publish(new AddBuildingToStorageRequest(building.BuildingConfig,false));
            choseBuildingMessagePublisher.Publish(new ChoseBuildingMessage(
                                                                           building.BuildingConfig,
                                                                           building.transform.position,
                                                                           building.Content.localPosition,
                                                                           building.Content.rotation,
                                                                           building.Tiles,
                                                                           building.Cell,
                                                                           true
                                                                          ));
            changeGameModeRequestPublisher.Publish(new ChangeGameModeRequest(GameMode.Redactor));
            CloseButton?.Invoke();
            Dispose();
        }

        private void ChangeInteractableState()
        {
            buildingInteractableFlag.IsInteractable = !buildingInteractableFlag.IsInteractable;
            Redraw();
        }
        
        public void Dispose()
        {
            disposables.Dispose();
            if (Root)
            {
                Object.Destroy(Root.gameObject);
            }
        }
    }
}