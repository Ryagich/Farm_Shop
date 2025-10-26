using System.Collections.Generic;
using Inventory.Finance;
using Inventory.Item;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Checkout
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class MoneyTaker : IFixedTickable
    {
        private readonly ItemsConfig itemsConfig;
        private readonly FinanceConfig financeConfig;
        private readonly FinanceManager financeManager;
        private readonly Transform place;

        private readonly List<(GameObject, int)> moneyObjects = new();

        public MoneyTaker
            (
                ItemsConfig itemsConfig,
                FinanceConfig financeConfig,
                FinanceManager financeManager,
                [Key("PlaceForMoney")] Transform place
            )
        {
            this.itemsConfig = itemsConfig;
            this.financeConfig = financeConfig;
            this.financeManager = financeManager;
            this.place = place;
        }

        public void Purchase(Matrix4x4 matrix, int cost)
        {
            var money = Object.Instantiate(financeConfig.MoneyPrefab, matrix.GetPosition(), matrix.rotation);
            moneyObjects.Add((money,cost));
        }
        
        public void FixedTick()
        {
            if (moneyObjects.Count <= 0)
                return;
            for (var i = 0; i < moneyObjects.Count; i++)
            {
                var item = moneyObjects[i];
                var itemTrans = item.Item1.transform;
                var targetPosition = place.position;
                if (Vector3.Distance(itemTrans.position, targetPosition) <= .05f)
                {
                    moneyObjects.Remove(item);
                    financeManager.TryChangeValue(item.Item2);
                    Object.Destroy(item.Item1.gameObject);
                    continue;
                }
                
                itemTrans.position = Vector3.MoveTowards(itemTrans.position, targetPosition, 
                                                         itemsConfig.MoveSpeed * Time.deltaTime);
            }
        }
    }
}