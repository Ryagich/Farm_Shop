using System;
using System.Linq;
using BuildingsAndGrid.Buildings;
using GameModes;
using Inventory;
using Inventory.Item;
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
        private readonly ItemConfig itemConfig;
        private readonly IInventory inventory;
        private readonly PopupHolders popupHolders;
        private readonly ShelfInfoRecorder shelfInfoRecorder;
        private readonly Building building;
        private readonly BuildingInteractableFlag buildingInteractableFlag;
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
                ItemConfig itemConfig,
                PopupHolders popupHolders,
                IInventory inventory,
                ShelfInfoRecorder shelfInfoRecorder,
                Building building,
                BuildingInteractableFlag buildingInteractableFlag,
                [Key("placesCount")] int placesCount
            )
        {
            this.localizationConfig = localizationConfig;
            this.itemConfig = itemConfig;
            this.inventory = inventory;
            this.popupHolders = popupHolders;
            this.shelfInfoRecorder = shelfInfoRecorder;
            this.building = building;
            this.buildingInteractableFlag = buildingInteractableFlag;
            this.placesCount = placesCount;
            
            choseBuildingMessagePublisher = GlobalMessagePipe.GetPublisher<ChoseBuildingMessage>();
            deleteBuildingOnGridPublisher = GlobalMessagePipe.GetPublisher<DeleteBuildingOnGridRequest>();
            addBuildingToStoragePublisher = GlobalMessagePipe.GetPublisher<AddBuildingToStorageRequest>();
            changeGameModeRequestPublisher = GlobalMessagePipe.GetPublisher<ChangeGameModeRequest>();
        }
        
        public RectTransform DrawPopup(Canvas canvas)
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

            popup.ProductDescription.text = $"{localizationConfig.ProductWord.GetLocalizedStringCached()}: {itemConfig.Name.GetLocalizedStringCached()}";
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
            deleteBuildingOnGridPublisher.Publish(new DeleteBuildingOnGridRequest(building, true));
            addBuildingToStoragePublisher.Publish(new AddBuildingToStorageRequest(building.BuildingConfig, true));
            CloseButton?.Invoke();
            Dispose();
        }
        
        private void Move()
        {
            deleteBuildingOnGridPublisher.Publish(new DeleteBuildingOnGridRequest(building,false));
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
            if (popupRect)
                Object.Destroy(popupRect.gameObject);
        }
    }
}