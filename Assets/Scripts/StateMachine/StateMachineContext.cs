using System.Collections.Generic;
using Buyer;
using Checkout;
using Doors;
using Inventory;
using MessagePipe;
using Messages;
using Shelf;
using UnityEngine;
using UnityEngine.AI;

namespace StateMachine
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class StateMachineContext
    {
        public BuyerSettings BuyerSettings;
        public List<BuyPosition> BuyPositions;
        public NavMeshAgent NavMeshAgent;
        public ShelvesController ShelvesController;
        public IInventory Inventory;
        public Transform Hand;
        public BuyerSpawnPoints BuyerSpawnPoints;
        public BuyerLifetimeScope BuyerLifetimeScope;
        public CheckoutsController CheckoutsController;
        public IPublisher<BuyerIsOverMessage> BuyerIsOverPublisher;
        public SurfaceController SurfaceController;
        public Animator Animator;
        public DoorsController DoorsController;

        public InfoAboutPositionAtShelfForBuyer UsedInfoAboutPositionAtShelfForBuyer;
        public BuyPosition TargetBuyPosition;
        public CheckoutController CheckoutController;

        public TargetPoint TargetPoint;
        public Vector3 TP;

        public float DistanceToTarget;
        public float DeltaTime;
        public float TimeBetweenIterations;
        public float T;
        public List<int> Costs = new();
        
        public int QueueIndex;
        
        public void SetShortDistanceToTarget()
            => DistanceToTarget = Random.Range(BuyerSettings.ShortRange.x, BuyerSettings.ShortRange.y);
        public void SetLongDistanceToTarget()
            => DistanceToTarget = Random.Range(BuyerSettings.LongRange.x, BuyerSettings.LongRange.y);

        public void ClearInfoAboutShelf()
        {
            if (UsedInfoAboutPositionAtShelfForBuyer is not null)
            {
                UsedInfoAboutPositionAtShelfForBuyer.IsFree.Value = true;
                UsedInfoAboutPositionAtShelfForBuyer = null;
            }
        }
        
        public bool CheckDistanceToTarget()
        {
            if (!NavMeshAgent.isOnNavMesh)
                return false;

            if (!SurfaceController.Surface)
                return false;
            if (TargetPoint == null || !TargetPoint.Target)
                return false;

            // Найдём ближайшие валидные точки на навмеш для агента и цели
            if (NavMesh.SamplePosition(NavMeshAgent.transform.position, out var agentHit, 2f, NavMesh.AllAreas) &&
                NavMesh.SamplePosition(TargetPoint.Target.position, out var targetHit, 2f, NavMesh.AllAreas))
            {
                // Если удалось найти обе точки — считаем расстояние между ними по навмешу
                var path = new NavMeshPath();
                if (NavMesh.CalculatePath(agentHit.position, targetHit.position, NavMesh.AllAreas, path))
                {
                    float pathLength = 0f;

                    for (int i = 1; i < path.corners.Length; i++)
                        pathLength += Vector3.Distance(path.corners[i - 1], path.corners[i]);

                    return pathLength < DistanceToTarget;
                }
            }

            return false;
        }
    }
}