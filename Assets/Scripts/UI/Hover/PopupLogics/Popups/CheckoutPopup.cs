using System;
using System.Collections.Generic;
using System.Linq;
using BuildingsAndGrid.Buildings;
using Buyer;
using Checkout;
using GameModes;
using Localization;
using MessagePipe;
using Messages;
using TMPro;
using UI.Hover.PopupLogics.Holders;
using UniRx;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UI.Hover.PopupLogics.Popups
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class CheckoutPopup : IObjectPopup, IDisposable
    {
        public event Action CloseButton;
        public RectTransform Root { get; private set; }
        public List<RectTransform> Children { get; private set; } = new();

        private readonly LocalizationConfig localizationConfig;
        private readonly PopupHolders popupHolders;
        private readonly Building building;
        private readonly BuildingInteractableFlag buildingInteractableFlag;
        private readonly ByersQueue byersQueue;
        private readonly BuyersSpawner buyersSpawner;

        private CompositeDisposable disposables = new();
        private CompositeDisposable buyersDisposables = new();

        private readonly IPublisher<ChoseBuildingMessage> choseBuildingMessagePublisher;
        private readonly IPublisher<DeleteBuildingOnGridRequest> deleteBuildingOnGridPublisher;
        private readonly IPublisher<AddBuildingToStorageRequest> addBuildingToStoragePublisher;
        private readonly IPublisher<ChangeGameModeRequest> changeGameModeRequestPublisher;
        
        public CheckoutPopup
            (                
                LocalizationConfig localizationConfig,
                PopupHolders popupHolders,
                Building building,
                BuildingInteractableFlag buildingInteractableFlag,
                ByersQueue byersQueue,
                BuyersSpawner buyersSpawner
            )
        {
            this.localizationConfig = localizationConfig;
            this.popupHolders = popupHolders;
            this.building = building;
            this.buildingInteractableFlag = buildingInteractableFlag;
            this.byersQueue = byersQueue;
            this.buyersSpawner = buyersSpawner;

            choseBuildingMessagePublisher = GlobalMessagePipe.GetPublisher<ChoseBuildingMessage>();
            deleteBuildingOnGridPublisher = GlobalMessagePipe.GetPublisher<DeleteBuildingOnGridRequest>();
            addBuildingToStoragePublisher = GlobalMessagePipe.GetPublisher<AddBuildingToStorageRequest>();
            changeGameModeRequestPublisher = GlobalMessagePipe.GetPublisher<ChangeGameModeRequest>();
        }

        public IObjectPopup DrawPopup(Canvas canvas)
        {
            var popup = Object.Instantiate(popupHolders.CheckoutPopupHolder, canvas.transform);
            Root = popup.GetComponent<RectTransform>();

            disposables = new CompositeDisposable();
            buyersDisposables = new CompositeDisposable();

            popup.ButtonMove.onClick.AddListener(Move);
            popup.ButtonDisable.onClick.AddListener(ChangeInteractableState);

            Subscribe();
            Redraw();

            return this;
        }

        public void Redraw()
        {
            if (!Root)
                return;
            var popup = Root.GetComponent<CheckoutPopupHolder>();
            var buyersInShop = buyersSpawner.buyers.Where(b => b.IsInsideShop.Value).ToArray();
            popup.ButtonMove.interactable = byersQueue.Buyers.Count <= 0 && buyersInShop.Length is 0;
            popup.BuyersCount.text = $"{localizationConfig.BuyersWord.GetLocalizedStringCached()}: {byersQueue.Buyers.Count}";
            popup.ButtonDisable.GetComponentInChildren<TMP_Text>().text = buildingInteractableFlag.IsInteractable 
                                                                              ? $"{localizationConfig.DisableWord.GetLocalizedStringCached()}" 
                                                                              : $"{localizationConfig.ActivateWord.GetLocalizedStringCached()}";
        }

        public void Subscribe()
        {
            // Изменение состава покупателей
            buyersSpawner.buyers
                         .ObserveAdd()
                         .Subscribe(e =>
                                    {
                                        SubscribeToBuyer(e.Value);
                                        Redraw();
                                    })
                         .AddTo(disposables);

            buyersSpawner.buyers
                         .ObserveRemove()
                         .Subscribe(_ =>
                                    {
                                        Redraw();
                                    })
                         .AddTo(disposables);

            // Подписаться на уже существующих
            foreach (var buyer in buyersSpawner.buyers)
                SubscribeToBuyer(buyer);
        }
        
        private void SubscribeToBuyer(BuyerLifetimeScope buyer)
        {
            buyer.IsInsideShop
                 .Subscribe(_ => Redraw())
                 .AddTo(buyersDisposables);
        }
        
        private void Move()
        {
            deleteBuildingOnGridPublisher.Publish(new DeleteBuildingOnGridRequest(building, false, building.Cell));
            addBuildingToStoragePublisher.Publish(new AddBuildingToStorageRequest(building.BuildingConfig, false));
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

        public void ClickOnObject() { }
        
        private void ChangeInteractableState()
        {
            buildingInteractableFlag.IsInteractable = !buildingInteractableFlag.IsInteractable;
            Redraw();
        }
        
        public void Dispose()
        {
            disposables.Dispose();
            buyersDisposables.Dispose();

            if (Root)
                Object.Destroy(Root.gameObject);
        }
    }
}