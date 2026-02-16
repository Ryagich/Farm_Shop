using System;
using System.Collections.Generic;
using System.Linq;
using Inventory.ObjectInventory;
using Localization;
using Products;
using UI.Hover.PopupLogics.Holders;
using UnityEngine;
using Utils;
using Object = UnityEngine.Object;

namespace UI.Hover.PopupLogics.Popups
{
    // ReSharper disable once ClassNeverInstantiated.Global
    //TODO: в игре пока нет ProductionZone - поэтому класс этот скипнул. Когда зоны будешь в игру доделывать - нужно и этот классик пилануть
    //Смотри другие реализации IObjectPopup
    public class ProductionZonePopup : IObjectPopup
    {
        public event Action CloseButton;
        public RectTransform Root { get; private set; }
        public List<RectTransform> Children { get; private set; } = new();

        private readonly ProductConfig productConfig;
        private readonly MaterialInventoriesController materialInventoriesController;
        private readonly ProductionZoneController productionZoneController;
        private readonly PopupHolders popupHolders;

        public ProductionZonePopup
            (
                ProductConfig productConfig,
                MaterialInventoriesController materialInventoriesController,
                ProductionZoneController productionZoneController,
                PopupHolders popupHolders
            )
        {
            this.productConfig = productConfig;
            this.materialInventoriesController = materialInventoriesController;
            this.productionZoneController = productionZoneController;
            this.popupHolders = popupHolders;
        }

        public IObjectPopup DrawPopup(Canvas canvas)
        {
            var popup = Object.Instantiate(popupHolders.ProductionZoneHolder, canvas.transform);
            var popupRect = popup.GetComponent<RectTransform>();

            popup.ProductionProductName.text = $"Product: {productConfig.ItemConfig.Name.GetLocalizedStringCached()}";
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
                materialTextHolder.text = $"{material.ItemConfig.Name.GetLocalizedStringCached()} {materialInventoriesController.inventories.First(i => i.GetConfig() == material.ItemConfig).Items.Count} / {material.CountForProduct}";
            }
            popupRect.sizeDelta = popupRect.sizeDelta.WithY(popupRect.sizeDelta.y + addHeight);         
            
            return this;
        }

        public void Redraw() { }
        public void Subscribe() { }
    }
}