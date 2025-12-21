using System;
using Checkout;
using Localization;
using UI.Hover.PopupLogics.Holders;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UI.Hover.PopupLogics.Popups
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class CheckoutPopup : IObjectPopup
    {
        public event Action CloseButton;

        private readonly LocalizationConfig localizationConfig;
        private readonly PopupHolders popupHolders;
        private readonly Canvas canvas;
        private readonly ByersQueue byersQueue;

        public CheckoutPopup
            (                
                LocalizationConfig localizationConfig,
                PopupHolders popupHolders,
                Canvas canvas,
                ByersQueue byersQueue
            )
        {
            this.localizationConfig = localizationConfig;
            this.popupHolders = popupHolders;
            this.canvas = canvas;
            this.byersQueue = byersQueue;
        }

        public RectTransform DrawPopup()
        {
            var popup = Object.Instantiate(popupHolders.CheckoutPopupHolder, canvas.transform);
            popup.BuyersCount.text = $"{localizationConfig.BuyersWord.GetLocalizedStringCached()}: {byersQueue.Buyers.Count}";
            return popup.GetComponent<RectTransform>();
        }
    }
}