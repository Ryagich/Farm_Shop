using System;
using System.Collections.Generic;
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
        public RectTransform Root { get; private set; }
        public List<RectTransform> Children { get; private set; } = new();

        private readonly PopupHolders popupHolders;
        private readonly Building building;

        private readonly IPublisher<ChoseBuildingMessage> choseBuildingMessagePublisher;
        private readonly IPublisher<DeleteBuildingOnGridRequest> deleteBuildingOnGridPublisher;
        private readonly IPublisher<AddBuildingToStorageRequest> addBuildingToStoragePublisher;
        private readonly IPublisher<ChangeGameModeRequest> changeGameModeRequestPublisher;
       
        private CompositeDisposable disposables = new();
        
        public EnvironmentPopup
            (
                PopupHolders popupHolders,
                Building building
            )
        {
            this.popupHolders = popupHolders;
            this.building = building;
            
            choseBuildingMessagePublisher = GlobalMessagePipe.GetPublisher<ChoseBuildingMessage>();
            deleteBuildingOnGridPublisher = GlobalMessagePipe.GetPublisher<DeleteBuildingOnGridRequest>();
            addBuildingToStoragePublisher = GlobalMessagePipe.GetPublisher<AddBuildingToStorageRequest>();
            changeGameModeRequestPublisher = GlobalMessagePipe.GetPublisher<ChangeGameModeRequest>();
        }
        
        public IObjectPopup DrawPopup(Canvas canvas)
        {
            var popup = Object.Instantiate(popupHolders.EnvironmentHolder, canvas.transform);
            Root = popup.GetComponent<RectTransform>();
            disposables = new CompositeDisposable();

            popup.ButtonMove.onClick.AddListener(Move);
            
            return this;
        }

        public void Redraw() { }
        public void Subscribe() { }

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
        
        public void Dispose()
        {
            disposables.Dispose();
            if (Root)
                Object.Destroy(Root.gameObject);
        }
    }
}