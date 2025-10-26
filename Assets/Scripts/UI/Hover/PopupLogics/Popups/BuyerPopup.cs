using System.Linq;
using Buyer;
using UI.Hover.PopupLogics.Holders;
using UnityEngine;
using Utils;

namespace UI.Hover.PopupLogics.Popups
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class BuyerPopup : IObjectPopup
    {
        private readonly PopupHolders popupHolders;
        private readonly BuyerController buyerController;
        private readonly Canvas canvas;

        public BuyerPopup
            (
                PopupHolders popupHolders,
                BuyerController buyerController,
                Canvas canvas
            )
        {
            this.popupHolders = popupHolders;
            this.buyerController = buyerController;
            this.canvas = canvas;
        }
        
        public RectTransform DrawPopup()
        {
            var popup = Object.Instantiate(popupHolders.BuyerPopupHolder, canvas.transform);
            var popupRect = popup.GetComponent<RectTransform>();
            var h = popup.ProductsListTitle.rectTransform.anchoredPosition.y - popup.ProductsListTitle.rectTransform.sizeDelta.y;
            
            foreach (var buyPosition in buyerController.context.BuyPositions)
            {
                var positionHolder = Object.Instantiate(popupHolders.BuyerProductInfo, popup.transform);
                var holderRect = positionHolder.GetComponent<RectTransform>();
                holderRect.anchoredPosition = new Vector2(.0f, h);
                h -= holderRect.sizeDelta.y;
                positionHolder.ProductName.text = $"Product: {buyPosition.Config.ItemName}";
                positionHolder.ProductCounts.text = $"{buyPosition.Count} / {buyPosition.Need}";
                positionHolder.Fill.fillAmount = (float)buyPosition.Count / buyPosition.Need;

                var positionsAtShelves = buyerController.context.ShelvesController.PositionsAtShelvesByTypes
                               .Where(p => p.Key == buyPosition.Config).ToArray();
                if (positionsAtShelves.Length is 0)
                {
                    positionHolder.FillBack.color = buyerController.context.BuyerSettings.NotForSaleColor;
                }
                else if (!positionsAtShelves.Any(p
                                             => p.Value
                                                 .Any(si => si.Key.CanGet())))
                {
                    positionHolder.FillBack.color = buyerController.context.BuyerSettings.OutOfStockColor;
                }
            }
            popup.BuyerStatus.text = buyerController.CurrentState.StateName;
            popupRect.sizeDelta = popupRect.sizeDelta.WithY(popupRect.sizeDelta.y
                                + (buyerController.context.BuyPositions.Count 
                                 * popupHolders.BuyerProductInfo.GetComponent<RectTransform>().sizeDelta.y) 
                                + 5.0f);
            return popup.GetComponent<RectTransform>();
        }
    }
}