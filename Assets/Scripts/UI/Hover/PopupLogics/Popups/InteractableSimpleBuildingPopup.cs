using System;
using System.Collections.Generic;
using BuildingsAndGrid.Buildings;
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
    public class InteractableSimpleBuildingPopup : IObjectPopup, IDisposable
    {
        public event Action CloseButton;
        public RectTransform Root { get; private set; }
        public List<RectTransform> Children { get; private set; } = new();

        private readonly LocalizationConfig localizationConfig;
        private readonly PopupHolders popupHolders;
        private readonly Building building;
        private readonly BuildingInteractableFlag buildingInteractableFlag;

        private readonly IPublisher<ChoseBuildingMessage> choseBuildingMessagePublisher;
        private readonly IPublisher<DeleteBuildingOnGridRequest> deleteBuildingOnGridPublisher;
        private readonly IPublisher<AddBuildingToStorageRequest> addBuildingToStoragePublisher;
        private readonly IPublisher<ChangeGameModeRequest> changeGameModeRequestPublisher;

        private CompositeDisposable disposables = new();
       
        public InteractableSimpleBuildingPopup
            (
                LocalizationConfig localizationConfig,
                PopupHolders popupHolders,
                Building building,
                BuildingInteractableFlag buildingInteractableFlag
            )
        {
            this.localizationConfig = localizationConfig;
            this.popupHolders = popupHolders;
            this.building = building;
            this.buildingInteractableFlag = buildingInteractableFlag;
        
            choseBuildingMessagePublisher = GlobalMessagePipe.GetPublisher<ChoseBuildingMessage>();
            deleteBuildingOnGridPublisher = GlobalMessagePipe.GetPublisher<DeleteBuildingOnGridRequest>();
            addBuildingToStoragePublisher = GlobalMessagePipe.GetPublisher<AddBuildingToStorageRequest>();
            changeGameModeRequestPublisher = GlobalMessagePipe.GetPublisher<ChangeGameModeRequest>();
        }

        public IObjectPopup DrawPopup(Canvas canvas)
        {
            var popup = Object.Instantiate(popupHolders.InteractableSimpleBuildingHolder, canvas.transform);
            Root = popup.GetComponent<RectTransform>();
            disposables = new CompositeDisposable();
     
            popup.ButtonMove.onClick.AddListener(Move);
            popup.ButtonDisable.onClick.AddListener(ChangeInteractableState); 

            Redraw();
            Subscribe();

            return this;
        }

        public void Redraw()
        {
            if (!Root)
                return;
            var popup = Root.GetComponent<InteractableSimpleBuildingHolder>();

            popup.ButtonDisable.GetComponentInChildren<TMP_Text>().text = buildingInteractableFlag.IsInteractable 
                                                                              ? $"{localizationConfig.DisableWord.GetLocalizedStringCached()}" 
                                                                              : $"{localizationConfig.ActivateWord.GetLocalizedStringCached()}";
        }

        public void Subscribe() { }
        public void ClickOnObject() { }

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

        private void ChangeInteractableState()
        {
            buildingInteractableFlag.IsInteractable = !buildingInteractableFlag.IsInteractable;
            Redraw();
        }

        public void Dispose()
        {
            disposables.Dispose();
            if (Root)
                Object.Destroy(Root.gameObject);
        }
    }
}