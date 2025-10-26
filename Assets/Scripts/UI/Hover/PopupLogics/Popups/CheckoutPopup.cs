using Checkout;
using UI.Hover.PopupLogics.Holders;
using UnityEngine;

namespace UI.Hover.PopupLogics.Popups
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class CheckoutPopup : IObjectPopup
    {
        private readonly PopupHolders popupHolders;
        private readonly Canvas canvas;
        private readonly ByersQueue byersQueue;

        public CheckoutPopup
            (                
                PopupHolders popupHolders,
                Canvas canvas,
                ByersQueue byersQueue
            )
        {
            this.popupHolders = popupHolders;
            this.canvas = canvas;
            this.byersQueue = byersQueue;
        }

        public RectTransform DrawPopup()
        {
            var popup = Object.Instantiate(popupHolders.CheckoutPopupHolder, canvas.transform);
            popup.BuyersCount.text = $"{byersQueue.Buyers.Count}";
            return popup.GetComponent<RectTransform>();
        }
    }
}