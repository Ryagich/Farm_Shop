using MessagePipe;
using Messages;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using VContainer.Unity;

namespace Buyer
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class SurfaceController : IStartable
    {
        private readonly BuyerSettings buyerSettings;
        public NavMeshSurface Surface;

        private bool updateQueued = false;
        
        public SurfaceController
            (
                BuyerSettings buyerSettings,
                ISubscriber<GridExtendMessage> gridExtendMessageSubscriber,
                ISubscriber<CreatedNewObjectOnGridMessage> createdNewObjectOnGridMessageSubscriber,
                ISubscriber<ObjectInHisPlaceMessage> objectInHisPlaceSubscriber
            )
        {
            this.buyerSettings = buyerSettings;

            gridExtendMessageSubscriber.Subscribe(OnGridExtended);
            createdNewObjectOnGridMessageSubscriber.Subscribe(OnNewBuilding);
            objectInHisPlaceSubscriber.Subscribe(OnBuildingEndedMoving);
        }

        public void Start()
        {
            Surface = Object.Instantiate(buyerSettings.NavMeshSurface);
            Surface.navMeshData = new NavMeshData();
            NavMesh.AddNavMeshData(Surface.navMeshData);
            Surface.UpdateNavMesh(Surface.navMeshData);
        }
        
        private void OnNewBuilding(CreatedNewObjectOnGridMessage msg)
        {
            QueueNavMeshUpdate();
        }
        
        private void OnBuildingEndedMoving(ObjectInHisPlaceMessage msg)
        {
            QueueNavMeshUpdate();
        }
        
        private void OnGridExtended(GridExtendMessage msg)
        {
            QueueNavMeshUpdate();
        }

        private void QueueNavMeshUpdate()
        {
            if (updateQueued)
                return;

            updateQueued = true;
            Surface.StartCoroutine(DelayedUpdate());
        }

        private System.Collections.IEnumerator DelayedUpdate()
        {
            yield return null;
            yield return null;

            Debug.Log("SurfaceController: NavMesh обновляется после задержки");

            Surface.UpdateNavMesh(Surface.navMeshData);
            updateQueued = false;
        }
    }
}