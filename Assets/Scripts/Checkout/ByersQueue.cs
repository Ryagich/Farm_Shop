using System;
using System.Collections.Generic;
using System.Linq;
using Buyer;
using StateMachine;
using UnityEngine;
using VContainer;
using Object = UnityEngine.Object;

namespace Checkout
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ByersQueue : IDisposable
    {
        private readonly BuyerSettings buyerSettings;
        private readonly Transform startQueueTransform;
        public List<StateMachineContext> Buyers = new();
        public int BuyersCount;

        public readonly List<(TargetPoint, bool)> queuePoints = new();
        
        public ByersQueue
            (
                BuyerSettings buyerSettings,
                [Key("queuePoint")] Transform startQueueTransform
            )
        {
            this.buyerSettings = buyerSettings;
            this.startQueueTransform = startQueueTransform;
            
            for (var i = 0; i < buyerSettings.CriticalCountOfQueue + 1; i++)
            {
                var place = new GameObject($"Queue Place {i}");
                var pt = place.transform;
                pt.SetParent(startQueueTransform);
                pt.localPosition = startQueueTransform.forward * buyerSettings.QueueDistance * i;
                queuePoints.Add((new TargetPoint(place.transform), true));
            }
        }

        public void Dispose()
        {
            foreach (var t in queuePoints)
            {
                if (t.Item1 != null && t.Item1.Target)
                {
                    Object.Destroy(t.Item1.Target.gameObject);
                    t.Item1.Target = null;
                }
            }
            queuePoints.Clear();
        }
        
        public (TargetPoint, bool) GetBuyerPosition(int index)
        {
            if (index >= queuePoints.Count)
            {
                return queuePoints[^1];
            }
            // while (index > queuePoints.Count)
            // {
            //     var place = new GameObject($"Queue Place {queuePoints.Count}");
            //     var pt = place.transform;
            //     pt.SetParent(startQueueTransform);
            //     pt.localPosition = startQueueTransform.forward * buyerSettings.QueueDistance * queuePoints.Count;
            //     queuePoints.Add((new TargetPoint(place.transform), true));
            // }
            return queuePoints[index]; //queuePoint.position + queuePoint.forward * buyerSettings.QueueDistance * index;
        }
        
        public bool TryGetFreePlace(out (TargetPoint, bool) placeInQueue)
        {
            placeInQueue = queuePoints[^1];
            var freePlace = queuePoints.FirstOrDefault(f => f.Item2);
            if (freePlace != default)
            {
                placeInQueue = freePlace;
                return true;
            }
            return false; //queuePoint.position + queuePoint.forward * buyerSettings.QueueDistance * Buyers.Count;
        }
    }
}