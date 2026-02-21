using System;
using System.Collections.Generic;
using System.Linq;
using BuildingsAndGrid.Buildings;
using GameModes;
using Inventory.Item;
using Landings.Landings;
using Landings.Plants;
using Landings.Plants.PlantConfigs;
using Localization;
using MessagePipe;
using Messages;
using Storage;
using TMPro;
using UI.Cards;
using UI.Configs;
using UI.Hover.PopupLogics.Holders;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using VContainer;
using Object = UnityEngine.Object;

namespace UI.Hover.PopupLogics.Popups
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class LandingPlantIsItemPopup : IObjectPopup, IDisposable
    {
        public event Action CloseButton;
        public RectTransform Root { get; private set; }
        public List<RectTransform> Children { get; private set; } = new();

        private readonly UIConfig uiConfig;
        private readonly LocalizationConfig localizationConfig;
        private readonly PopupHolders popupHolders;
        private readonly PlantsStorage plantsStorage;
        private readonly Building building;
        private readonly LandingPlantIsItemController landingPlantIsItemController;
        private readonly PlantGrowerByUpper plantGrowerByUpper;
        private readonly PlantGrowerByStages plantGrowerByStages;
        private readonly IPublisher<AddPlantToStorageRequest> addPlantToStorageRequestPublisher;

        private readonly IPublisher<ChoseBuildingMessage> choseBuildingMessagePublisher;
        private readonly IPublisher<DeleteBuildingOnGridRequest> deleteBuildingOnGridPublisher;
        private readonly IPublisher<AddBuildingToStorageRequest> addBuildingToStoragePublisher;
        private readonly IPublisher<ChangeGameModeRequest> changeGameModeRequestPublisher;
                
        private CompositeDisposable disposables = new();
        private WindowToChangeProduct windowToChangeProduct;
        
        public LandingPlantIsItemPopup
            (
                UIConfig uiConfig,
                LocalizationConfig localizationConfig,
                PopupHolders popupHolders,
                PlantsStorage plantsStorage,
                Building building,
                LandingPlantIsItemController landingPlantIsItemController,
                [Key(nameof(PlantGrowerByUpper))] PlantGrowerByUpper plantGrowerByUpper,
                [Key(nameof(PlantGrowerByStages))] PlantGrowerByStages plantGrowerByStages,
                IPublisher<AddPlantToStorageRequest> addPlantToStorageRequestPublisher
            )
        {
            this.uiConfig = uiConfig;
            this.localizationConfig = localizationConfig;
            this.popupHolders = popupHolders;
            this.plantsStorage = plantsStorage;
            this.building = building;
            this.landingPlantIsItemController = landingPlantIsItemController;
            this.plantGrowerByUpper = plantGrowerByUpper;
            this.plantGrowerByStages = plantGrowerByStages;
            this.addPlantToStorageRequestPublisher = addPlantToStorageRequestPublisher;

            choseBuildingMessagePublisher = GlobalMessagePipe.GetPublisher<ChoseBuildingMessage>();
            deleteBuildingOnGridPublisher = GlobalMessagePipe.GetPublisher<DeleteBuildingOnGridRequest>();
            addBuildingToStoragePublisher = GlobalMessagePipe.GetPublisher<AddBuildingToStorageRequest>();
            changeGameModeRequestPublisher = GlobalMessagePipe.GetPublisher<ChangeGameModeRequest>();
        }

        public IObjectPopup DrawPopup(Canvas canvas)
        {
            var popup = Object.Instantiate(popupHolders.LandingPlantIsItemHolder, canvas.transform);
            Root = popup.GetComponent<RectTransform>();
            disposables = new CompositeDisposable();
            
            popup.ButtonMove.onClick.AddListener(Move);
            popup.ButtonMoveToInventory.onClick.AddListener(MoveInInventory);
            popup.ChangePlantButton.onClick.AddListener(() => OpenProductChangedWindow(popup.ChangePlantButton.GetComponent<RectTransform>()));

            Redraw();
            Subscribe();
            
            return this;
        }
        
        public void Redraw()        
        {
            if (!Root)
                return;
            var popup = Root.GetComponent<LandingPlantIsItemHolder>();
           
            popup.PlantName.text = string.Empty;
            
            if (landingPlantIsItemController.PlantConfig)
            {
                popup.PlantName.text = landingPlantIsItemController.PlantConfig.Name.GetLocalizedStringCached();
                popup.Icon.sprite = landingPlantIsItemController.PlantConfig.Icon;
            }
            if (!landingPlantIsItemController.PlantConfig)
            {
                popup.GrowStage.text = string.Empty;
                popup.GrowFill.fillAmount = 0;
            }
            else if (plantGrowerByUpper.IsPlanting)
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

        private void OpenProductChangedWindow(RectTransform rectTransform)
        {
            if (Root && windowToChangeProduct)
            {
                var wr = windowToChangeProduct.GetComponent<RectTransform>();
                if (Children.Contains(wr))
                {
                    Children.Remove(wr);
                }   
                Object.Destroy(windowToChangeProduct.gameObject);
            }
            windowToChangeProduct = Object.Instantiate(uiConfig.WindowToChangeProduct, rectTransform);
            windowToChangeProduct.ButtonToClose.onClick.AddListener(CloseProductChangedWindow);

            var windowRect = windowToChangeProduct.GetComponent<RectTransform>();
            windowRect.anchoredPosition = windowRect.anchoredPosition.WithY(windowRect.anchoredPosition.y - rectTransform.rect.size.y/2);
            windowRect.SetParent(Root);
            Children.Add(windowRect);

            foreach (var plantInStorage in plantsStorage.GetPlants(PlantType.Vegetable))
            {
                var productCard = Object.Instantiate(uiConfig.ProductCard, windowToChangeProduct.Content);
                var buttonText = productCard.Button.GetComponentInChildren<TMP_Text>();
                var lastStage = plantInStorage.PlantConfig.Stages.Last();
                var itemHolder = lastStage.GetComponent<ItemHolder>();
                var itemConfig = itemHolder.Config;
                
                productCard.Icon.sprite = itemConfig.Icon;
                productCard.Name.text = itemConfig.Name.GetLocalizedStringCached();
                
                if (landingPlantIsItemController.PlantConfig != null 
                 && landingPlantIsItemController.PlantConfig.Id.Equals(plantInStorage.PlantConfig.Id))
                {
                    productCard.InInventory.text = $"{localizationConfig.InInventory.GetLocalizedStringCached()}: {plantInStorage.Count}";
                    buttonText.text = localizationConfig.Selected.GetLocalizedString(); //Выбрано
                    productCard.Button.interactable = false;
                }
                else
                {
                    productCard.InInventory.text = $"{localizationConfig.InInventory.GetLocalizedStringCached()}: {plantInStorage.Count}";
                    buttonText.text = localizationConfig.Select.GetLocalizedString(); //Выбрать
                    productCard.Button.interactable = plantInStorage.Count > 0;
                    productCard.Button.onClick.AddListener(() => ReRegister(plantInStorage));
                }
            }
            var cardWidth = uiConfig.ProductCard.GetComponent<RectTransform>().rect.width;
            var lg = windowToChangeProduct.Content.GetComponent<HorizontalLayoutGroup>();
            var cardsCount = plantsStorage.Plants.Count; 
            var contentSize = lg.padding.left + lg.padding.right + (lg.spacing * (cardsCount - 1)) + (cardWidth * cardsCount);
            windowToChangeProduct.Content.anchorMin = Vector2.up;
            windowToChangeProduct.Content.anchorMax = Vector2.up;
            windowToChangeProduct.Content.pivot = Vector2.up;

            windowToChangeProduct.Content.sizeDelta = windowToChangeProduct.Content.sizeDelta.WithX(contentSize);
        }
        
        private void ReRegister(PlantInStorage plantInStorage)
        {
            var oldPlantConfig = landingPlantIsItemController.PlantConfig;
            plantsStorage.Get(plantInStorage.PlantConfig);
            if (oldPlantConfig)
            {
                plantsStorage.Add(new AddPlantToStorageRequest(oldPlantConfig, true));
            }
            landingPlantIsItemController.ChangeConfig(plantInStorage.PlantConfig);
            Redraw();
            if (Root && windowToChangeProduct)
            {
                var wr = windowToChangeProduct.GetComponent<RectTransform>();
                if (Children.Contains(wr))
                {
                    Children.Remove(wr);
                }
                Object.Destroy(windowToChangeProduct.gameObject);
            }
            var popup = Root.GetComponent<LandingPlantIsItemHolder>();
            OpenProductChangedWindow(popup.ChangePlantButton.GetComponent<RectTransform>());
        }
        
        private void CloseProductChangedWindow()
        {
            Children.Clear();
            if (Root && windowToChangeProduct)
            {
                Object.Destroy(windowToChangeProduct.gameObject);
            }
            windowToChangeProduct = null;
            Redraw();
        }
        
        public void Subscribe()
        {
            plantGrowerByUpper.LostDistance.Subscribe(_ => Redraw()).AddTo(disposables);
            plantGrowerByStages.timer.Subscribe(_ => Redraw()).AddTo(disposables);
            plantGrowerByStages.IsPlanted.Subscribe(_ => Redraw()).AddTo(disposables);
        }

        private void MoveInInventory()
        {
            var oldPlantConfig = landingPlantIsItemController.PlantConfig;
            if (oldPlantConfig)
            {
                plantsStorage.Add(new AddPlantToStorageRequest(oldPlantConfig, true));
            }
            
            deleteBuildingOnGridPublisher.Publish(new DeleteBuildingOnGridRequest(building, true, building.Cell));
            addBuildingToStoragePublisher.Publish(new AddBuildingToStorageRequest(building.BuildingConfig, true));
            CloseButton?.Invoke();
            Dispose();
        }
        
        private void Move()
        {
            var oldPlantConfig = landingPlantIsItemController.PlantConfig;
            if (oldPlantConfig)
            {
                plantsStorage.Add(new AddPlantToStorageRequest(oldPlantConfig, false));
            }
            
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
            if (Root)
                Object.Destroy(Root.gameObject);
        }
    }
}