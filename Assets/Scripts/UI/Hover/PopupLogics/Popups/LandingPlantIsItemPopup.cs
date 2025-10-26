using System.Linq;
using Inventory.Item;
using Landings;
using Landings.Plants;
using Landings.Plants.PlantConfigs;
using UI.Hover.PopupLogics.Holders;
using UnityEngine;
using VContainer;

namespace UI.Hover.PopupLogics.Popups
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class LandingPlantIsItemPopup : IObjectPopup
    {
        private readonly PopupHolders popupHolders;
        private readonly PlantConfig plantConfig;
        private readonly Canvas canvas;
        private readonly PlantGrowerByUpper plantGrowerByUpper;
        private readonly PlantGrowerByStages plantGrowerByStages;
        
        public LandingPlantIsItemPopup
            (
                PopupHolders popupHolders,
                PlantConfig plantConfig,
                Canvas canvas,
                [Key(nameof(PlantGrowerByUpper))] PlantGrowerByUpper plantGrowerByUpper,
                [Key(nameof(PlantGrowerByStages))] PlantGrowerByStages plantGrowerByStages
            )
        {
            this.popupHolders = popupHolders;
            this.plantConfig = plantConfig;
            this.canvas = canvas;
            this.plantGrowerByUpper = plantGrowerByUpper;
            this.plantGrowerByStages = plantGrowerByStages;
        }

        public RectTransform DrawPopup()
        {
            var popup = Object.Instantiate(popupHolders.LandingPlantIsItemHolder, canvas.transform);
            popup.PlantName.text = $"{plantConfig.Stages.Last().GetComponent<ItemHolder>().Config.ItemName}";
            
            if (plantGrowerByUpper.IsPlanting)
            {
                popup.GrowStage.text = $"Grow Stage: 1";
                popup.GrowFill.fillAmount = plantGrowerByUpper.LostDistance / plantGrowerByUpper.Distance;
            }
            else if (plantGrowerByStages.IsPlanted)
            {
                popup.GrowStage.text = $"Grow Stage: Grown";
                popup.GrowFill.fillAmount = 1;
            }
            else
            {
                popup.GrowStage.text = $"Grow Stage: {plantGrowerByStages.currentStage + 1}";
                popup.GrowFill.fillAmount = plantGrowerByStages.timer / plantGrowerByStages.stageTime;
            }
            return popup.GetComponent<RectTransform>();
        }
    }
}