using System.Linq;
using Inventory.ObjectInventory;
using Products;
using UI.Hover.PopupLogics.Holders;
using UnityEngine;
using Utils;

namespace UI.Hover.PopupLogics.Popups
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ProductionZonePopup : IObjectPopup
    {
        private readonly ProductConfig productConfig;
        private readonly MaterialInventoriesController materialInventoriesController;
        private readonly ProductionZoneController productionZoneController;
        private readonly PopupHolders popupHolders;
        private readonly Canvas canvas;

        public ProductionZonePopup
            (
                ProductConfig productConfig,
                MaterialInventoriesController materialInventoriesController,
                ProductionZoneController productionZoneController,
                PopupHolders popupHolders,
                Canvas canvas
            )
        {
            this.productConfig = productConfig;
            this.materialInventoriesController = materialInventoriesController;
            this.productionZoneController = productionZoneController;
            this.popupHolders = popupHolders;
            this.canvas = canvas;
        }

        public RectTransform DrawPopup()
        {
            var popup = Object.Instantiate(popupHolders.ProductionZoneHolder, canvas.transform);
            var popupRect = popup.GetComponent<RectTransform>();

            popup.ProductionProductName.text = $"Product: {productConfig.ItemConfig.ItemName}";
            popup.ProductionTime.text = $"Production time: {productConfig.Time}";
            popup.ReadyToTake.text = $"Ready: {productionZoneController.productionInventory.Items.Count}";
            var materialsHeaderRect = popup.MaterialsHeader.GetComponent<RectTransform>();
            var startHeight = materialsHeaderRect.anchoredPosition.y;
            var addHeight = .0f;
          
            foreach (var material in productConfig.Materials)           
            {
                var materialTextHolder = Object.Instantiate(popupHolders.MaterialProductText, popup.transform);
                var materialTextRect = materialTextHolder.GetComponent<RectTransform>();
                addHeight += materialTextRect.sizeDelta.y;
                materialTextRect.anchoredPosition = materialTextRect.anchoredPosition.WithY(startHeight - addHeight);
                materialTextHolder.text = $"{material.ItemConfig.ItemName} {materialInventoriesController.inventories.First(i => i.GetConfig() == material.ItemConfig).Items.Count} / {material.CountForProduct}";
            }
            popupRect.sizeDelta = popupRect.sizeDelta.WithY(popupRect.sizeDelta.y + addHeight);         
            
            return popup.GetComponent<RectTransform>();
        }
    }
}