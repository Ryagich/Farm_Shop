using System;
using System.Collections.Generic;
using Purchase;
using UI.Hover.PopupLogics.Holders;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Object = UnityEngine.Object;

namespace UI.Hover.PopupLogics.Popups
{
    // ReSharper disable once ClassNeverInstantiated.Global
    //TODO: в игре пока нет PurchaseObject - поэтому класс этот скипнул. Когда зоны будешь в игру доделывать - нужно и этот классик пилануть
    //Смотри другие реализации IObjectPopup
    public class PurchaseObjectPopup : IObjectPopup
    {
        public event Action CloseButton;
        public RectTransform Root { get; private set; }
        public List<RectTransform> Children { get; private set; } = new();

        private readonly PopupHolders popupHolders;
        private readonly PurchaseObject purchaseObject;
        private readonly LifetimeScope purchase;
        private readonly int cost;

        public PurchaseObjectPopup
            (
                PopupHolders popupHolders,
                PurchaseObject purchaseObject,
                [Key("Purchase")] LifetimeScope purchase,
                [Key("Cost")] int cost
            )
        {
            this.popupHolders = popupHolders;
            this.purchaseObject = purchaseObject;
            this.purchase = purchase;
            this.cost = cost;
        }
            
        public IObjectPopup DrawPopup(Canvas canvas)
        {
            var popup = Object.Instantiate(popupHolders.PurchaseObjectHolder, canvas.transform);
            popup.PurchaseObjectName.text = $"Object: {purchase.name}";
            popup.Purchase.text = $"{cost - purchaseObject.Remaining.Value} / {cost}";
            popup.Fill.fillAmount = (float)(cost - purchaseObject.Remaining.Value) / cost;
            
            return this;
        }

        public void Redraw() { }
        public void Subscribe() { }
    } 
}