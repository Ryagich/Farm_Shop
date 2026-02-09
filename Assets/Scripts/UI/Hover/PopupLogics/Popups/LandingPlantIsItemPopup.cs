using System;
using System.Linq;
using BuildingsAndGrid.Buildings;
using GameModes;
using Inventory.Item;
using Landings.Plants;
using Landings.Plants.PlantConfigs;
using Localization;
using MessagePipe;
using Messages;
using UI.Hover.PopupLogics.Holders;
using UniRx;
using UnityEngine;
using VContainer;
using Object = UnityEngine.Object;

namespace UI.Hover.PopupLogics.Popups
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class LandingPlantIsItemPopup : IObjectPopup, IDisposable
    {
        public event Action CloseButton;

        private readonly LocalizationConfig localizationConfig;
        private readonly PopupHolders popupHolders;
        private readonly PlantConfig plantConfig;
        private readonly Building building;
        private readonly PlantGrowerByUpper plantGrowerByUpper;
        private readonly PlantGrowerByStages plantGrowerByStages;
        
        private readonly IPublisher<ChoseBuildingMessage> choseBuildingMessagePublisher;
        private readonly IPublisher<DeleteBuildingOnGridRequest> deleteBuildingOnGridPublisher;
        private readonly IPublisher<AddBuildingToStorageRequest> addBuildingToStoragePublisher;
        private readonly IPublisher<ChangeGameModeRequest> changeGameModeRequestPublisher;
                
        private CompositeDisposable disposables = new();
        private RectTransform popupRect;
        
        public LandingPlantIsItemPopup
            (
                LocalizationConfig localizationConfig,
                PopupHolders popupHolders,
                PlantConfig plantConfig,
                Building building,
                [Key(nameof(PlantGrowerByUpper))] PlantGrowerByUpper plantGrowerByUpper,
                [Key(nameof(PlantGrowerByStages))] PlantGrowerByStages plantGrowerByStages
            )
        {
            this.localizationConfig = localizationConfig;
            this.popupHolders = popupHolders;
            this.plantConfig = plantConfig;
            this.building = building;
            this.plantGrowerByUpper = plantGrowerByUpper;
            this.plantGrowerByStages = plantGrowerByStages;
            
            choseBuildingMessagePublisher = GlobalMessagePipe.GetPublisher<ChoseBuildingMessage>();
            deleteBuildingOnGridPublisher = GlobalMessagePipe.GetPublisher<DeleteBuildingOnGridRequest>();
            addBuildingToStoragePublisher = GlobalMessagePipe.GetPublisher<AddBuildingToStorageRequest>();
            changeGameModeRequestPublisher = GlobalMessagePipe.GetPublisher<ChangeGameModeRequest>();
        }

        public RectTransform DrawPopup(Canvas canvas)
        {
            var popup = Object.Instantiate(popupHolders.LandingPlantIsItemHolder, canvas.transform);
            popupRect = popup.GetComponent<RectTransform>();
            disposables = new CompositeDisposable();

            popup.ButtonMove.onClick.AddListener(Move);
            popup.ButtonMoveToInventory.onClick.AddListener(MoveInInventory);
            
            Redraw();
            Subscribe();
            
            return popupRect;
        }

        public void Redraw()        
        {
            if (!popupRect || plantConfig == null)
                return;
            if (plantConfig.Stages == null || plantConfig.Stages.Count == 0)
                return;
            var popup = popupRect.GetComponent<LandingPlantIsItemHolder>();
            var lastStage = plantConfig.Stages.LastOrDefault();
            if (lastStage == null)
            {
                popup.PlantName.text = string.Empty;
                return;
            }
            var itemHolder = lastStage.GetComponent<ItemHolder>();
            if (itemHolder == null)
            {
                popup.PlantName.text = string.Empty;
                return;
            }
            popup.PlantName.text = itemHolder.Config.Name.GetLocalizedStringCached();
            // popup.PlantName.text = $"{plantConfig.Stages.Last().GetComponent<ItemHolder>().Config.Name.GetLocalizedStringCached()}";
            
            if (plantGrowerByUpper.IsPlanting)
            {
                popup.GrowStage.text = $"{localizationConfig.GrowStage.GetLocalizedStringCached()}: 1";
                popup.GrowFill.fillAmount = plantGrowerByUpper.LostDistance.Value / plantGrowerByUpper.Distance;
            }
            else if (plantGrowerByStages.IsPlanted.Value)
            {
                popup.GrowStage.text = $"{localizationConfig.GrowStage.GetLocalizedStringCached()}: {localizationConfig.GrownWord.GetLocalizedStringCached()}";
                popup.GrowFill.fillAmount = 1;
            }
            else
            {
                popup.GrowStage.text = $"{localizationConfig.GrowStage.GetLocalizedStringCached()}: {plantGrowerByStages.currentStage + 1}";
                popup.GrowFill.fillAmount = plantGrowerByStages.timer.Value / plantGrowerByStages.stageTime;
            }
        }

        public void Subscribe()
        {
            plantGrowerByUpper.LostDistance.Subscribe(_ => Redraw()).AddTo(disposables);
            plantGrowerByStages.timer.Subscribe(_ => Redraw()).AddTo(disposables);
            plantGrowerByStages.IsPlanted.Subscribe(_ => Redraw()).AddTo(disposables);
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
            CloseButton?.Invoke();
            Dispose();

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
        }
        
        public void Dispose()
        {
            disposables.Dispose();
            if (popupRect)
                Object.Destroy(popupRect.gameObject);
        }
    }
}