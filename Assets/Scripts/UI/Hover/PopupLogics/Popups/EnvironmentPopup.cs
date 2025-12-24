using System;
using BuildingsAndGrid.Buildings;
using GameModes;
using MessagePipe;
using Messages;
using UI.Hover.PopupLogics.Holders;
using UniRx;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UI.Hover.PopupLogics.Popups
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class EnvironmentPopup : IObjectPopup, IDisposable
    {
        public event Action CloseButton;

        private readonly PopupHolders popupHolders;
        private readonly Building building;
        private readonly Canvas canvas;

        private readonly IPublisher<ChoseBuildingMessage> choseBuildingMessagePublisher;
        private readonly IPublisher<DeleteBuildingOnGridRequest> deleteBuildingOnGridPublisher;
        private readonly IPublisher<AddBuildingToStorageRequest> addBuildingToStoragePublisher;
        private readonly IPublisher<ChangeGameModeRequest> changeGameModeRequestPublisher;
       
        private CompositeDisposable disposables = new();
        private RectTransform popupRect;
        
        public EnvironmentPopup
            (
                PopupHolders popupHolders,
                Building building,
                Canvas canvas
            )
        {
            this.popupHolders = popupHolders;
            this.building = building;
            this.canvas = canvas;
            
            choseBuildingMessagePublisher = GlobalMessagePipe.GetPublisher<ChoseBuildingMessage>();
            deleteBuildingOnGridPublisher = GlobalMessagePipe.GetPublisher<DeleteBuildingOnGridRequest>();
            addBuildingToStoragePublisher = GlobalMessagePipe.GetPublisher<AddBuildingToStorageRequest>();
            changeGameModeRequestPublisher = GlobalMessagePipe.GetPublisher<ChangeGameModeRequest>();
        }
        
        public RectTransform DrawPopup()
        {
            var popup = Object.Instantiate(popupHolders.EnvironmentHolder, canvas.transform);
            popupRect = popup.GetComponent<RectTransform>();
            disposables = new CompositeDisposable();

            popup.ButtonMove.onClick.AddListener(OnMove);
            
            return popupRect;
        }

        public void Redraw() { }
        public void Subscribe() { }

        private void OnMove()
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
        
        public void Dispose()
        {
            disposables.Dispose();
            if (popupRect)
                Object.Destroy(popupRect.gameObject);
        }
    }
}