using System;
using Checkout;
using Localization;
using UI.Hover.PopupLogics.Holders;
using UniRx;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UI.Hover.PopupLogics.Popups
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class CheckoutPopup : IObjectPopup, IDisposable
    {
        public event Action CloseButton;

        private readonly LocalizationConfig localizationConfig;
        private readonly PopupHolders popupHolders;
        private readonly Canvas canvas;
        private readonly ByersQueue byersQueue;

        private CompositeDisposable disposables = new();
        private RectTransform popupRect;

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
            disposables = new CompositeDisposable();
            popupRect = Object.Instantiate(popupHolders.CheckoutPopupHolder, canvas.transform)
                              .GetComponent<RectTransform>();
            Redraw();
            Subscribe();
            
            return popupRect;
        }

        public void Redraw()
        {
            if (!popupRect)
                return;
            var popup = popupRect.GetComponent<CheckoutPopupHolder>();
            popup.BuyersCount.text = $"{localizationConfig.BuyersWord.GetLocalizedStringCached()}: {byersQueue.Buyers.Count}";
        }

        public void Subscribe()
        {
            byersQueue.Buyers
                      .ObserveAdd()
                      .Subscribe(_ => Redraw())
                      .AddTo(disposables);
        }
        
        public void Dispose()
        {
            disposables.Dispose();
            if (popupRect)
                Object.Destroy(popupRect.gameObject);
        }
    }
}