using System;
using System.Linq;
using Inventory;
using Inventory.Item;
using Shelf;
using UI.Hover.PopupLogics.Holders;
using UnityEngine;
using VContainer;
using Object = UnityEngine.Object;

namespace UI.Hover.PopupLogics.Popups
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ShelfPopup : IObjectPopup
    {
        public event Action CloseButton;

        private readonly ItemConfig itemConfig;
        private readonly IInventory inventory;
        private readonly PopupHolders popupHolders;
        private readonly ShelfInfoRecorder shelfInfoRecorder;
        private readonly Canvas canvas;
        private readonly int placesCount;

        public ShelfPopup
            (
                ItemConfig itemConfig,
                PopupHolders popupHolders,
                IInventory inventory,
                ShelfInfoRecorder shelfInfoRecorder,
                Canvas canvas,
                [Key("placesCount")] int placesCount
            )
        {
            this.itemConfig = itemConfig;
            this.inventory = inventory;
            this.popupHolders = popupHolders;
            this.shelfInfoRecorder = shelfInfoRecorder;
            this.canvas = canvas;
            this.placesCount = placesCount;
        }

        public RectTransform DrawPopup()
        {
            var popup = Object.Instantiate(popupHolders.ShelfPopupHolder, canvas.transform);
            popup.ProductDescription.text = $"Product: {itemConfig.ItemName}";
            
            popup.ProductsCount.text = $"{inventory.Items.Count} / {placesCount}";
            popup.ProductsFillImage.fillAmount = (float)inventory.Items.Count / placesCount;
            
            popup.BuyersCount.text = $"{shelfInfoRecorder.info.Where(i => !i.IsFree).ToArray().Length} / {shelfInfoRecorder.info.Count}";
            popup.BuyersFillImage.fillAmount = (float)shelfInfoRecorder.info.Where(i => !i.IsFree).ToArray().Length / shelfInfoRecorder.info.Count;

            return popup.GetComponent<RectTransform>();
        }
    }
}