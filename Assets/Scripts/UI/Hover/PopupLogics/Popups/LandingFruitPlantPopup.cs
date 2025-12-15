using System;
using Inventory.ObjectInventory;
using Landings;
using Landings.Landings;
using Landings.Plants;
using Landings.Plants.PlantConfigs;
using Localization;
using UI.Hover.PopupLogics.Holders;
using UnityEngine;
using Utils;
using VContainer;
using Object = UnityEngine.Object;

namespace UI.Hover.PopupLogics.Popups
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class LandingFruitPlantPopup : IObjectPopup
    {
        public event Action CloseButton;

        private readonly LocalizationConfig localizationConfig;
        private readonly PopupHolders popupHolders;
        private readonly FruitPlantConfig fruitPlantConfig;
        private readonly Canvas canvas;
        private readonly PlantGrowerByUpper plantGrowerByUpper;
        private readonly PlantGrowerByStages plantGrowerByStages;
        private readonly LandingFruitPlantController landingFruitPlantController;
        private readonly FruitPlantInventory inventory;

        public LandingFruitPlantPopup
            (
                LocalizationConfig localizationConfig,
                PopupHolders popupHolders,
                FruitPlantConfig fruitPlantConfig,
                LandingFruitPlantController landingFruitPlantController,
                FruitPlantInventory inventory,
                Canvas canvas,
                [Key(nameof(PlantGrowerByUpper))] PlantGrowerByUpper plantGrowerByUpper,
                [Key(nameof(PlantGrowerByStages))] PlantGrowerByStages plantGrowerByStages
            )
        {
            this.localizationConfig = localizationConfig;
            this.popupHolders = popupHolders;
            this.fruitPlantConfig = fruitPlantConfig;
            this.canvas = canvas;
            this.plantGrowerByUpper = plantGrowerByUpper;
            this.plantGrowerByStages = plantGrowerByStages;
            this.landingFruitPlantController = landingFruitPlantController;
            this.inventory = inventory;
        }

        public RectTransform DrawPopup()
        {
            var popup = Object.Instantiate(popupHolders.LandingFruitPlantHolder, canvas.transform);
            popup.PlantName.text = $"{fruitPlantConfig.HandFruit.Name.GetLocalizedString()}";
            
            if (plantGrowerByUpper.IsPlanting)
            {
                popup.GrowStage.text = $"{localizationConfig.GrowStage.GetLocalizedString()}: 1";
                popup.GrowFill.fillAmount = plantGrowerByUpper.LostDistance / plantGrowerByUpper.Distance;
            }
            else if (plantGrowerByStages.IsPlanted)
            {
                popup.GrowStage.text = $"{localizationConfig.GrowStage.GetLocalizedString()}: {localizationConfig.GrownWord.GetLocalizedString()}";
                popup.GrowFill.fillAmount = 1;

                var popupRect = popup.GetComponent<RectTransform>();
                var aboutFruits = Object.Instantiate(popupHolders.LandingFruitPlantInfoAboutFruits, popup.transform);
                var holderRect = aboutFruits.GetComponent<RectTransform>();
                var lastRect = popup.GrowStage.GetComponent<RectTransform>();

                aboutFruits.FruitsCount.text = $"{localizationConfig.FruitsWord.GetLocalizedString()}: {landingFruitPlantController.fruitCount}";
                aboutFruits.FruitsReady.text = $"{localizationConfig.ReadyWord.GetLocalizedString()} {inventory.GetCount()}";
                holderRect.anchoredPosition = new Vector2(.0f, lastRect.anchoredPosition.y - lastRect.sizeDelta.y);
                popupRect.sizeDelta = popupRect.sizeDelta.WithY(popupRect.sizeDelta.y + holderRect.sizeDelta.y);
            }
            else
            {
                popup.GrowStage.text = $"{localizationConfig.GrowStage.GetLocalizedString()}: {plantGrowerByStages.currentStage + 1}";
                popup.GrowFill.fillAmount = plantGrowerByStages.timer / plantGrowerByStages.stageTime;
            }
            return popup.GetComponent<RectTransform>();
        }
    }
}