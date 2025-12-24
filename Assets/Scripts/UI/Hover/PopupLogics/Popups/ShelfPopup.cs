using System;
using System.Linq;
using BuildingsAndGrid.Buildings;
using GameModes;
using Inventory;
using Localization;
using MessagePipe;
using Messages;
using Shelf;
using TMPro;
using UI.Hover.PopupLogics.Holders;
using UnityEngine;
using VContainer;
using Object = UnityEngine.Object;
using UniRx;

namespace UI.Hover.PopupLogics.Popups
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ShelfPopup : IObjectPopup, IDisposable
    {
        public event Action CloseButton;
        
        private readonly LocalizationConfig localizationConfig;
        private readonly IInventory inventory;
        private readonly PopupHolders popupHolders;
        private readonly ShelfInfoRecorder shelfInfoRecorder;
        private readonly Building building;
        private readonly BuildingInteractableFlag buildingInteractableFlag;
        private readonly Canvas canvas;
        private readonly int placesCount;

        private readonly IPublisher<ChoseBuildingMessage> choseBuildingMessagePublisher;
        private readonly IPublisher<DeleteBuildingOnGridRequest> deleteBuildingOnGridPublisher;
        private readonly IPublisher<AddBuildingToStorageRequest> addBuildingToStoragePublisher;
        private readonly IPublisher<ChangeGameModeRequest> changeGameModeRequestPublisher;
        
        private CompositeDisposable disposables = new();
        private RectTransform popupRect;
        
        public ShelfPopup
            (
                LocalizationConfig localizationConfig,
                PopupHolders popupHolders,
                IInventory inventory,
                ShelfInfoRecorder shelfInfoRecorder,
                Building building,
                BuildingInteractableFlag buildingInteractableFlag,
                Canvas canvas,
                [Key("placesCount")] int placesCount
            )
        {
            this.localizationConfig = localizationConfig;
            this.inventory = inventory;
            this.popupHolders = popupHolders;
            this.shelfInfoRecorder = shelfInfoRecorder;
            this.building = building;
            this.buildingInteractableFlag = buildingInteractableFlag;
            this.canvas = canvas;
            this.placesCount = placesCount;
            
            choseBuildingMessagePublisher = GlobalMessagePipe.GetPublisher<ChoseBuildingMessage>();
            deleteBuildingOnGridPublisher = GlobalMessagePipe.GetPublisher<DeleteBuildingOnGridRequest>();
            addBuildingToStoragePublisher = GlobalMessagePipe.GetPublisher<AddBuildingToStorageRequest>();
            changeGameModeRequestPublisher = GlobalMessagePipe.GetPublisher<ChangeGameModeRequest>();
        }
        
        public RectTransform DrawPopup()
        {
            var popup = Object.Instantiate(popupHolders.ShelfPopupHolder, canvas.transform);
            popupRect = popup.GetComponent<RectTransform>();
            disposables = new CompositeDisposable();

            popup.ButtonMove.onClick.AddListener(Move);
            popup.ButtonMoveToInventory.onClick.AddListener(MoveInInventory);
            popup.ButtonDisable.onClick.AddListener(ChangeInteractableState); 
            // popup.ButtonDisable.onClick.AddListener(Dispose);
            
            Redraw();
            Subscribe();

            return popupRect;
        }
        
        public void Redraw()
        {
            if (!popupRect)
                return;
            var popup = popupRect.GetComponent<ShelfPopupHolder>();

            var total = shelfInfoRecorder.info.Count;
            var busy = total - shelfInfoRecorder.info.Count(i => i.IsFree.Value);
            
            popup.BuyersCount.text = $"{busy} / {total}";
            popup.BuyersFillImage.fillAmount = total == 0 ? 0f : (float)busy / total;

            popup.ProductsCount.text = $"{inventory.Items.Count} / {placesCount}";
            popup.ProductsFillImage.fillAmount = (float)inventory.Items.Count / placesCount;

            var allFree = shelfInfoRecorder.info.All(i => i.IsFree.Value);
            popup.ButtonMove.interactable = allFree;
            popup.ButtonMoveToInventory.interactable = allFree;
            
            popup.ButtonDisable.GetComponentInChildren<TMP_Text>().text = buildingInteractableFlag.IsInteractable 
                                                                        ? $"{localizationConfig.DisableWord.GetLocalizedStringCached()}" 
                                                                        : $"{localizationConfig.ActivateWord.GetLocalizedStringCached()}";
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

            inventory.Items
                     .ObserveAdd()
                     .Subscribe(_ => Redraw())
                     .AddTo(disposables);
            inventory.Items
                     .ObserveRemove()
                     .Subscribe(_ => Redraw())
                     .AddTo(disposables);
        }

        private void MoveInInventory()
        {
            deleteBuildingOnGridPublisher.Publish(new DeleteBuildingOnGridRequest(building));
            addBuildingToStoragePublisher.Publish(new AddBuildingToStorageRequest(building.BuildingConfig));
            CloseButton?.Invoke();
            Dispose();
        }
        
        private void Move()
        {
            deleteBuildingOnGridPublisher.Publish(new DeleteBuildingOnGridRequest(building));
            addBuildingToStoragePublisher.Publish(new AddBuildingToStorageRequest(building.BuildingConfig));
            choseBuildingMessagePublisher.Publish(new ChoseBuildingMessage(
                                                                           building.BuildingConfig,
                                                                           building.transform.position,
                                                                           building.Content.localPosition,
                                                                           building.Content.rotation,
                                                                           building.Tiles,
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
            if (popupRect)
                Object.Destroy(popupRect.gameObject);
        }
    }
}