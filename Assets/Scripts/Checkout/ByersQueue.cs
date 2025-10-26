using System.Collections.Generic;
using Buyer;
using StateMachine;
using UnityEngine;
using VContainer;

namespace Checkout
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ByersQueue
    {
        public List<StateMachineContext> Buyers = new();
        
        private readonly BuyerSettings buyerSettings;
        private readonly Transform queuePoint;
        
        public ByersQueue
            (
                BuyerSettings buyerSettings,
                [Key("QueuePoint")] Transform queuePoint
            )
        {
            this.buyerSettings = buyerSettings;
            this.queuePoint = queuePoint;
        }

        public Vector3 GetBuyerPosition(int index)
        {
            return queuePoint.position + queuePoint.forward * buyerSettings.QueueDistance * index;
        }
        
        public Vector3 GetBuyerPosition()
        {
            return queuePoint.position + queuePoint.forward * buyerSettings.QueueDistance * Buyers.Count;
        }
    }
}