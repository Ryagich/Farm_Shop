using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Inventory.Finance;
using Inventory.Item;
using MessagePipe;
using Messages;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Object = UnityEngine.Object;

namespace Purchase
{
    //TODO: Дублирование логики перемещения предметов уже в 3 классах - нужно вынести ее в 1 класс,
    //TODO: а остальные пусть пользуются резуальтатом ее труда 
    // ReSharper disable once ClassNeverInstantiated.Global
    public class PurchaseMoneyMover : IFixedTickable
    {
        private readonly ItemsConfig itemsConfig;
        private readonly FinanceConfig financeConfig;
        private readonly Transform place;
        private readonly List<GameObject> moneyObjects = new();
        private readonly GameObject instance;
        private readonly IPublisher<PlayerMadePurchaseMessage> playerMadePurchaseMessage;
        
        private readonly LifetimeScope purchase;
        private readonly Vector3 position;      
        private readonly Vector3 rotation;
        
        private bool isDone;  
        [SuppressMessage("ReSharper", "ParameterHidesMember")]
        public PurchaseMoneyMover
            (
                [Key("Place")] Transform place,
                [Key("Purchase")] LifetimeScope purchase,
                [Key("TargetPosition")] Vector3 position,
                [Key("TargetRotation")] Vector3 rotation,
                ItemsConfig itemsConfig,
                FinanceConfig financeConfig,
                PurchaseObject purchaseObject,
                GameObject instance,
                IPublisher<PlayerMadePurchaseMessage> playerMadePurchaseMessage
            )
        {
            this.place = place;
            this.purchase = purchase;
            this.position = position;
            this.rotation = rotation;
            this.itemsConfig = itemsConfig;
            this.financeConfig = financeConfig;
            this.instance = instance;
            this.playerMadePurchaseMessage = playerMadePurchaseMessage;
    
            purchaseObject.Interaction += OnBuy;
            purchaseObject.Completed += ChangeDoneState;
        }

        private void ChangeDoneState()
        {
            isDone = true;
        }

        private void OnBuy(Vector3 pos)
        {
            moneyObjects.Add(Object.Instantiate(financeConfig.MoneyPrefab, pos, Quaternion.identity));
        }
        
        public void FixedTick()
        {
            if (moneyObjects.Count == 0)
                return;
            for (var i = 0; i < moneyObjects.Count; i++)
            {
                var item = moneyObjects[i];
                var itemTrans = item.transform;
                var itemPosition = place.position;
                
                if (Vector3.Distance(itemTrans.position, itemPosition) < 0.01f)
                {
                    moneyObjects.Remove(item);
                    Object.Destroy(item.gameObject);
                    if (isDone && moneyObjects.Count is 0)
                    {
                        playerMadePurchaseMessage.Publish(new PlayerMadePurchaseMessage(purchase, position, rotation));
                        Object.Destroy(instance);
                    }
                    continue;
                }

                itemTrans.position =
                    Vector3.MoveTowards(itemTrans.position, itemPosition, itemsConfig.MoveSpeed * Time.deltaTime);
                itemTrans.rotation =
                    Quaternion.RotateTowards(itemTrans.rotation, place.rotation, itemsConfig.MoveSpeed * Time.deltaTime);
            }
        }
    }
}