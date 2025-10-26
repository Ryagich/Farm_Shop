using System.Linq;
using Inventory;
using Inventory.Item;
using Shelf;
using UI.Hover.PopupLogics.Holders;
using UnityEngine;
using VContainer;

namespace UI.Hover.PopupLogics.Popups
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ShelfPopup : IObjectPopup
    {
        private readonly ItemConfig itemConfig;
        private readonly IInventory inventory;
        private readonly PopupHolders popupHolders;
        private readonly InfoAboutShelfForBuyerGenerator infoAboutShelfForBuyerGenerator;
        private readonly Canvas canvas;
        private readonly int placesCount;

        public ShelfPopup
            (
                ItemConfig itemConfig,
                PopupHolders popupHolders,
                IInventory inventory,
                InfoAboutShelfForBuyerGenerator infoAboutShelfForBuyerGenerator,
                Canvas canvas,
                [Key("placesCount")] int placesCount
            )
        {
            this.itemConfig = itemConfig;
            this.inventory = inventory;
            this.popupHolders = popupHolders;
            this.infoAboutShelfForBuyerGenerator = infoAboutShelfForBuyerGenerator;
            this.canvas = canvas;
            this.placesCount = placesCount;
        }

        public RectTransform DrawPopup()
        {
            var popup = Object.Instantiate(popupHolders.ShelfPopupHolder, canvas.transform);
            popup.ProductDescription.text = $"Product: {itemConfig.ItemName}";
            
            popup.ProductsCount.text = $"{inventory.Items.Count} / {placesCount}";
            popup.ProductsFillImage.fillAmount = (float)inventory.Items.Count / placesCount;
            
            popup.BuyersCount.text = $"{infoAboutShelfForBuyerGenerator.info.Where(i => !i.IsFree).ToArray().Length} / {infoAboutShelfForBuyerGenerator.info.Count}";
            popup.BuyersFillImage.fillAmount = (float)infoAboutShelfForBuyerGenerator.info.Where(i => !i.IsFree).ToArray().Length / infoAboutShelfForBuyerGenerator.info.Count;

            return popup.GetComponent<RectTransform>();
        }
    }
}