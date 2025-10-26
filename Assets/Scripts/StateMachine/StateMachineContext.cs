using System.Collections.Generic;
using Buyer;
using Checkout;
using Inventory;
using MessagePipe;
using Messages;
using Shelf;
using Unity.AI.Navigation;
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
        public Transform ShoppingEnter;
        public Transform Hand;
        public List<Transform> SpawnPointsForBuyers;
        public BuyerLifetimeScope BuyerLifetimeScope;
        public CheckoutsController CheckoutsController;
        public IPublisher<BuyerIsOverMessage> BuyerIsOverPublisher;
        public NavMeshSurface NavMeshSurface;
        public Animator Animator;

        public InfoAboutPositionAtShelfForBuyer UsedInfoAboutPositionAtShelfForBuyer;
        public CheckoutController CheckoutController;

        public Vector3 TargetPosition;
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
                UsedInfoAboutPositionAtShelfForBuyer.IsFree = true;
                UsedInfoAboutPositionAtShelfForBuyer = null;
            }
        }
        
        public bool CheckDistanceToTarget()
        {
            if (!NavMeshAgent.isOnNavMesh)
                return false;

            if (!NavMeshSurface)
                return false;

            // Найдём ближайшие валидные точки на навмеш для агента и цели
            if (NavMesh.SamplePosition(NavMeshAgent.transform.position, out var agentHit, 2f, NavMesh.AllAreas) &&
                NavMesh.SamplePosition(TargetPosition, out var targetHit, 2f, NavMesh.AllAreas))
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