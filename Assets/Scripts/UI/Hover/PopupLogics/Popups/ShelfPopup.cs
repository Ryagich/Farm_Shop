using System;
using System.Linq;
using BuildingsAndGrid.Buildings;
using GameModes;
using Inventory;
using Inventory.Item;
using MessagePipe;
using Messages;
using Shelf;
using TMPro;
using UI.Hover.PopupLogics.Holders;
using UnityEngine;
using VContainer;
using Object = UnityEngine.Object;

namespace UI.Hover.PopupLogics.Popups
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ShelfPopup : IObjectPopup
    {
        public event Action CloseButton;

        private readonly ItemConfig itemConfig;
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

        public ShelfPopup
            (
                ItemConfig itemConfig,
                PopupHolders popupHolders,
                IInventory inventory,
                ShelfInfoRecorder shelfInfoRecorder,
                Building building,
                BuildingInteractableFlag buildingInteractableFlag,
                Canvas canvas,
                [Key("placesCount")] int placesCount
            )
        {
            this.itemConfig = itemConfig;
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
            popup.ProductDescription.text = $"Product: {itemConfig.ItemName}";
            
            popup.ProductsCount.text = $"{inventory.Items.Count} / {placesCount}";
            popup.ProductsFillImage.fillAmount = (float)inventory.Items.Count / placesCount;
            
            popup.BuyersCount.text = $"{shelfInfoRecorder.info.Where(i => !i.IsFree).ToArray().Length} / {shelfInfoRecorder.info.Count}";
            popup.BuyersFillImage.fillAmount = (float)shelfInfoRecorder.info.Where(i => !i.IsFree).ToArray().Length / shelfInfoRecorder.info.Count;

            if (shelfInfoRecorder.info.All(i => i.IsFree))
            {
                popup.ButtonMove.onClick.AddListener(Move);
                popup.ButtonMoveToInventory.onClick.AddListener(MoveInInventory);
            }
            else
            {
                popup.ButtonMove.interactable = false;
                popup.ButtonMoveToInventory.interactable = false;
            }

            if (buildingInteractableFlag.IsInteractable)
            {
                popup.ButtonDisable.GetComponentInChildren<TMP_Text>().text = "Disable";
            }
            else
            {
                popup.ButtonDisable.GetComponentInChildren<TMP_Text>().text = "Activate";
            }
            popup.ButtonDisable.onClick.AddListener(() => buildingInteractableFlag.IsInteractable = !buildingInteractableFlag.IsInteractable);
            
            return popup.GetComponent<RectTransform>();
        }
        
        private void MoveInInventory()
        {
            deleteBuildingOnGridPublisher.Publish(new DeleteBuildingOnGridRequest(building));
            addBuildingToStoragePublisher.Publish(new AddBuildingToStorageRequest(building.BuildingConfig));
            CloseButton?.Invoke();
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
        }
    }
}