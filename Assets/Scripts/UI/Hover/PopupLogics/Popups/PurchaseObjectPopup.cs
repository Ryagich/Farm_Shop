using Purchase;
using UI.Hover.PopupLogics.Holders;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace UI.Hover.PopupLogics.Popups
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class PurchaseObjectPopup : IObjectPopup
    {
        private readonly PopupHolders popupHolders;
        private readonly PurchaseObject purchaseObject;
        private readonly LifetimeScope purchase;
        private readonly int cost;
        private readonly Canvas canvas;

        public PurchaseObjectPopup
            (
                PopupHolders popupHolders,
                PurchaseObject purchaseObject,
                Canvas canvas,
                [Key("Purchase")] LifetimeScope purchase,
                [Key("Cost")] int cost
            )
        {
            this.popupHolders = popupHolders;
            this.purchaseObject = purchaseObject;
            this.purchase = purchase;
            this.cost = cost;
            this.canvas = canvas;
        }
            
        public RectTransform DrawPopup()
        {
            var popup = Object.Instantiate(popupHolders.PurchaseObjectHolder, canvas.transform);
            popup.PurchaseObjectName.text = $"Object: {purchase.name}";
            popup.Purchase.text = $"{cost - purchaseObject.Remaining.Value} / {cost}";
            popup.Fill.fillAmount = (float)(cost - purchaseObject.Remaining.Value) / cost;
            
            return popup.GetComponent<RectTransform>();
        }
    } 
}