using System;
using System.Collections.Generic;
using System.Linq;
using Buyer;
using Localization;
using UI.Hover.PopupLogics.Holders;
using UniRx;
using UnityEngine;
using Utils;
using Object = UnityEngine.Object;

namespace UI.Hover.PopupLogics.Popups
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class BuyerPopup : IObjectPopup, IDisposable
    {
        public event Action CloseButton;
        public RectTransform Root { get; private set; }
        public List<RectTransform> Children { get; private set; } = new();

        private readonly PopupHolders popupHolders;
        private readonly BuyerController buyerController;

        private CompositeDisposable disposables = new();
        private List<RectTransform> buyPositions = new();
        private float baseHeight;
        
        public BuyerPopup
            (
                PopupHolders popupHolders,
                BuyerController buyerController
            )
        {
            this.popupHolders = popupHolders;
            this.buyerController = buyerController;
        }
        
        public IObjectPopup DrawPopup(Canvas canvas)
        {
            var popup = Object.Instantiate(popupHolders.BuyerPopupHolder, canvas.transform);
            Root = popup.GetComponent<RectTransform>();
            disposables = new CompositeDisposable();
            baseHeight = Root.sizeDelta.y;

            Redraw();
            Subscribe();

            return this;
        }

        public void Redraw()
        {
            if (buyerController is null)
            {
                Dispose();
                return;
            }
            if (!Root)
                return;
            var popup = Root.GetComponent<BuyerPopupHolder>();
            var h = popup.ProductsListTitle.rectTransform.anchoredPosition.y - popup.ProductsListTitle.rectTransform.sizeDelta.y;
           
            foreach (var buyPosition in buyPositions)
            {
                if (buyPosition)
                    Object.Destroy(buyPosition.gameObject);
            }
            buyPositions.Clear();
            
            foreach (var buyPosition in buyerController.context.BuyPositions)
            {
                var positionHolder = Object.Instantiate(popupHolders.BuyerProductInfo, popup.transform);
                var holderRect = positionHolder.GetComponent<RectTransform>();
                holderRect.anchoredPosition = new Vector2(.0f, h);
                h -= holderRect.sizeDelta.y;
                positionHolder.ProductName.text = $"{buyPosition.Config.Name.GetLocalizedStringCached()}";
                // positionHolder.ProductName.text = $"{localizationConfig.ProductWord.GetLocalizedStringCached()}: {buyPosition.Config.Name.GetLocalizedStringCached()}";
                positionHolder.ProductCounts.text = $"{buyPosition.Count.Value} / {buyPosition.Need}";
                positionHolder.Fill.fillAmount = (float)buyPosition.Count.Value / buyPosition.Need;

                var positionsAtShelves = buyerController.context.ShelvesController.Shelves
                                                        .Where(shelf => shelf.Key.CanGet(buyPosition.Config)).ToArray();
                if (positionsAtShelves.Length is 0)
                {
                    positionHolder.FillBack.color = buyerController.context.BuyerSettings.NotForSaleColor;
                }
                buyPositions.Add(holderRect);
            }
            popup.BuyerStatus.text = buyerController.CurrentState.Value.Name.GetLocalizedStringCached();
            var itemHeight = popupHolders.BuyerProductInfo
                                           .GetComponent<RectTransform>()
                                           .sizeDelta.y;
            Root.sizeDelta = Root.sizeDelta.WithY(
                                                  baseHeight + buyerController.context.BuyPositions.Count * itemHeight + 5f
                                                 );
        }
        
        public void ClickOnObject() { }

        public void Subscribe()
        {
            foreach (var buyPosition in buyerController.context.BuyPositions)
                buyPosition.Count.Subscribe(_ => Redraw()).AddTo(disposables);
            buyerController.CurrentState.Subscribe(_ => Redraw()).AddTo(disposables);
        }

        public void Dispose()
        {
            disposables.Dispose();
            buyPositions.Clear();
            if (Root)
                Object.Destroy(Root.gameObject);
        }
    }
}