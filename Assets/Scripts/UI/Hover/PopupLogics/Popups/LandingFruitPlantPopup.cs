using System;
using System.Collections.Generic;
using BuildingsAndGrid.Buildings;
using GameModes;
using Inventory.ObjectInventory;
using Landings.Landings;
using Landings.Plants;
using Landings.Plants.PlantConfigs;
using Localization;
using MessagePipe;
using Messages;
using UI.Hover.PopupLogics.Holders;
using UniRx;
using UnityEngine;
using Utils;
using VContainer;
using Object = UnityEngine.Object;

namespace UI.Hover.PopupLogics.Popups
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class LandingFruitPlantPopup : IObjectPopup, IDisposable
    {
        public event Action CloseButton;
        public RectTransform Root { get; private set; }
        public List<RectTransform> Children { get; private set; } = new();

        private readonly LocalizationConfig localizationConfig;
        private readonly PopupHolders popupHolders;
        private readonly FruitPlantConfig fruitPlantConfig;
        private readonly PlantGrowerByUpper plantGrowerByUpper;
        private readonly PlantGrowerByStages plantGrowerByStages;
        private readonly LandingFruitPlantController landingFruitPlantController;
        private readonly FruitPlantInventory inventory;
        private readonly Building building;

        private readonly IPublisher<ChoseBuildingMessage> choseBuildingMessagePublisher;
        private readonly IPublisher<DeleteBuildingOnGridRequest> deleteBuildingOnGridPublisher;
        private readonly IPublisher<AddBuildingToStorageRequest> addBuildingToStoragePublisher;
        private readonly IPublisher<ChangeGameModeRequest> changeGameModeRequestPublisher;
        
        private CompositeDisposable disposables = new();
     
        private LandingFruitPlantInfoAboutFruits fruitsInfo;
        private float baseHeight;
        
        public LandingFruitPlantPopup
            (
                LocalizationConfig localizationConfig,
                PopupHolders popupHolders,
                FruitPlantConfig fruitPlantConfig,
                LandingFruitPlantController landingFruitPlantController,
                FruitPlantInventory inventory,
                Building building,
                [Key(nameof(PlantGrowerByUpper))] PlantGrowerByUpper plantGrowerByUpper,
                [Key(nameof(PlantGrowerByStages))] PlantGrowerByStages plantGrowerByStages
            )
        {
            this.localizationConfig = localizationConfig;
            this.popupHolders = popupHolders;
            this.fruitPlantConfig = fruitPlantConfig;
            this.plantGrowerByUpper = plantGrowerByUpper;
            this.plantGrowerByStages = plantGrowerByStages;
            this.landingFruitPlantController = landingFruitPlantController;
            this.inventory = inventory;
            this.building = building;

            choseBuildingMessagePublisher = GlobalMessagePipe.GetPublisher<ChoseBuildingMessage>();
            deleteBuildingOnGridPublisher = GlobalMessagePipe.GetPublisher<DeleteBuildingOnGridRequest>();
            addBuildingToStoragePublisher = GlobalMessagePipe.GetPublisher<AddBuildingToStorageRequest>();
            changeGameModeRequestPublisher = GlobalMessagePipe.GetPublisher<ChangeGameModeRequest>();
        }

        public IObjectPopup DrawPopup(Canvas canvas)
        {
            var popup = Object.Instantiate(popupHolders.LandingFruitPlantHolder, canvas.transform);
            Root = popup.GetComponent<RectTransform>();
            disposables = new CompositeDisposable();
            baseHeight = Root.sizeDelta.y;
            
            popup.ButtonMove.onClick.AddListener(Move);
            popup.ButtonMoveToInventory.onClick.AddListener(MoveInInventory);
            
            Redraw();
            Subscribe();
            
            return this;
        }

        public void Redraw()
        {
            if (!Root)
                return;
            var popup = Root.GetComponent<LandingFruitPlantHolder>();
            
            popup.PlantName.text = $"{fruitPlantConfig.HandFruit.Name.GetLocalizedStringCached()}";
            if (fruitsInfo)
            {
                Object.Destroy(fruitsInfo.gameObject);
            }
            
            if (plantGrowerByUpper.IsPlanting)
            {
                popup.GrowStage.text = $"{localizationConfig.GrowStage.GetLocalizedStringCached()}: 1";
                popup.GrowFill.fillAmount = plantGrowerByUpper.LostDistance.Value / plantGrowerByUpper.Distance;
            }
            else if (plantGrowerByStages.IsPlanted.Value)
            {
                popup.GrowStage.text = $"{localizationConfig.GrowStage.GetLocalizedStringCached()}: {localizationConfig.GrownWord.GetLocalizedStringCached()}";
                popup.GrowFill.fillAmount = 1;

                fruitsInfo = Object.Instantiate(popupHolders.LandingFruitPlantInfoAboutFruits, popup.transform);
                var holderRect = fruitsInfo.GetComponent<RectTransform>();
                var lastRect = popup.GrowStage.GetComponent<RectTransform>();
                var moveButtonRect = popup.ButtonMove.GetComponent<RectTransform>();
                var inventoryButtonRect = popup.ButtonMoveToInventory.GetComponent<RectTransform>();

                fruitsInfo.FruitsCount.text = $"{localizationConfig.FruitsWord.GetLocalizedStringCached()}: {landingFruitPlantController.fruitCount}";
                fruitsInfo.FruitsReady.text = $"{localizationConfig.ReadyWord.GetLocalizedStringCached()} {inventory.GetCount()}";
                holderRect.anchoredPosition = new Vector2(.0f, lastRect.anchoredPosition.y - lastRect.sizeDelta.y);
                // popupRect.sizeDelta = popupRect.sizeDelta.WithY(popupRect.sizeDelta.y + holderRect.sizeDelta.y);
                Root.sizeDelta = Root.sizeDelta.WithY(baseHeight + holderRect.sizeDelta.y);
                moveButtonRect.anchoredPosition = moveButtonRect.anchoredPosition.WithY(holderRect.anchoredPosition.y - holderRect.sizeDelta.y);
                inventoryButtonRect.anchoredPosition = inventoryButtonRect.anchoredPosition.WithY(moveButtonRect.anchoredPosition.y - moveButtonRect.sizeDelta.y);
            }
            else
            {
                popup.GrowStage.text = $"{localizationConfig.GrowStage.GetLocalizedStringCached()}: {plantGrowerByStages.currentStage + 1}";
                popup.GrowFill.fillAmount = plantGrowerByStages.timer.Value / plantGrowerByStages.stageTime;
            }
        }
        
        public void Subscribe()
        {
            inventory.Fruits
                     .ObserveAdd()
                     .Subscribe(_ => Redraw())
                     .AddTo(disposables);
            inventory.Fruits
                     .ObserveRemove()
                     .Subscribe(_ => Redraw())
                     .AddTo(disposables);
            plantGrowerByUpper.LostDistance.Subscribe(_ => Redraw()).AddTo(disposables);
            plantGrowerByStages.timer.Subscribe(_ => Redraw()).AddTo(disposables);
            plantGrowerByStages.IsPlanted.Subscribe(_ => Redraw()).AddTo(disposables);
            landingFruitPlantController.fruitCount.Subscribe(_ => Redraw()).AddTo(disposables);
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