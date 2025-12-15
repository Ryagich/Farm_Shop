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
using UnityEngine;
using VContainer;
using Object = UnityEngine.Object;

namespace UI.Hover.PopupLogics.Popups
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class LandingPlantIsItemPopup : IObjectPopup
    {
        public event Action CloseButton;

        private readonly LocalizationConfig localizationConfig;
        private readonly PopupHolders popupHolders;
        private readonly PlantConfig plantConfig;
        private readonly Building building;
        private readonly Canvas canvas;
        private readonly PlantGrowerByUpper plantGrowerByUpper;
        private readonly PlantGrowerByStages plantGrowerByStages;
        
        private readonly IPublisher<ChoseBuildingMessage> choseBuildingMessagePublisher;
        private readonly IPublisher<DeleteBuildingOnGridRequest> deleteBuildingOnGridPublisher;
        private readonly IPublisher<AddBuildingToStorageRequest> addBuildingToStoragePublisher;
        private readonly IPublisher<ChangeGameModeRequest> changeGameModeRequestPublisher;
        
        public LandingPlantIsItemPopup
            (
                LocalizationConfig localizationConfig,
                PopupHolders popupHolders,
                PlantConfig plantConfig,
                Building building,
                Canvas canvas,
                [Key(nameof(PlantGrowerByUpper))] PlantGrowerByUpper plantGrowerByUpper,
                [Key(nameof(PlantGrowerByStages))] PlantGrowerByStages plantGrowerByStages
            )
        {
            this.localizationConfig = localizationConfig;
            this.popupHolders = popupHolders;
            this.plantConfig = plantConfig;
            this.building = building;
            this.canvas = canvas;
            this.plantGrowerByUpper = plantGrowerByUpper;
            this.plantGrowerByStages = plantGrowerByStages;
            
            choseBuildingMessagePublisher = GlobalMessagePipe.GetPublisher<ChoseBuildingMessage>();
            deleteBuildingOnGridPublisher = GlobalMessagePipe.GetPublisher<DeleteBuildingOnGridRequest>();
            addBuildingToStoragePublisher = GlobalMessagePipe.GetPublisher<AddBuildingToStorageRequest>();
            changeGameModeRequestPublisher = GlobalMessagePipe.GetPublisher<ChangeGameModeRequest>();
        }

        public RectTransform DrawPopup()
        {
            var popup = Object.Instantiate(popupHolders.LandingPlantIsItemHolder, canvas.transform);
            popup.PlantName.text = $"{plantConfig.Stages.Last().GetComponent<ItemHolder>().Config.Name.GetLocalizedString()}";
            
            if (plantGrowerByUpper.IsPlanting)
            {
                popup.GrowStage.text = $"{localizationConfig.GrowStage.GetLocalizedString()}: 1";
                popup.GrowFill.fillAmount = plantGrowerByUpper.LostDistance / plantGrowerByUpper.Distance;
            }
            else if (plantGrowerByStages.IsPlanted)
            {
                popup.GrowStage.text = $"{localizationConfig.GrowStage.GetLocalizedString()}: Grown";
                popup.GrowFill.fillAmount = 1;
            }
            else
            {
                popup.GrowStage.text = $"{localizationConfig.GrowStage.GetLocalizedString()}: {plantGrowerByStages.currentStage + 1}";
                popup.GrowFill.fillAmount = plantGrowerByStages.timer / plantGrowerByStages.stageTime;
            }
            popup.ButtonMove.onClick.AddListener(Move);
            popup.ButtonMoveToInventory.onClick.AddListener(MoveInInventory);
                    
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