using System;
using System.Collections.Generic;
using Buyer;
using StateMachine;
using UniRx;
using UnityEngine;
using VContainer;
using Object = UnityEngine.Object;

namespace Checkout
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ByersQueue : IDisposable
    {
        public ReactiveCollection<StateMachineContext> Buyers = new();
        public int BuyersCount;

        public readonly List<(TargetPoint, bool)> queuePoints = new();
        
        public ByersQueue
            (
                BuyerSettings buyerSettings,
                [Key("queuePoint")] Transform startQueueTransform,
                [Key("QueueCount")] int queueCount
            )
        {
            for (var i = 0; i < queueCount; i++)
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
            return queuePoints[index];
        }
    }
}