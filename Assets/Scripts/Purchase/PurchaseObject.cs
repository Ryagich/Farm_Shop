using System;
using Interactable;
using Inventory.Finance;
using UniRx;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Purchase
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class PurchaseObject
    {
        public event Action Completed;
        public event Action<Vector3> Interaction;
        
        private readonly FinanceManager financeManager;

        public readonly ReactiveProperty<int> Remaining = new();
        private readonly int costOneInteraction;

        public PurchaseObject
            (
                [Key("Cost")] int cost,
                FinanceConfig financeConfig,
                InteractableConfig interactableConfig,
                Interactable.Interactable interactable,
                FinanceManager financeManager
            )
        {
            this.financeManager = financeManager;
            Remaining.Value = cost;

            var maxInteractions = Mathf.FloorToInt(financeConfig.MaxPurchaseTime / interactableConfig.TimeBetweenInteractions);

            costOneInteraction = cost <= maxInteractions ? 1 : Mathf.CeilToInt((float)cost / maxInteractions);
            
            interactable.Interacted += Interact;
        }

        private void Interact(LifetimeScope scope)
        {
            if (Remaining.Value <= 0)
                return;
            var pay = Mathf.Min(costOneInteraction, Remaining.Value);
            
            if (financeManager.Check(pay))
            {
                Remaining.Value -= pay;
                financeManager.TryChangeValue(-pay);
                Interaction?.Invoke(scope.Container.Resolve<Transform>("Hand").position);
                if (Remaining.Value <= 0)
                {
                    Completed?.Invoke();
                }
            }
        }
    }
}