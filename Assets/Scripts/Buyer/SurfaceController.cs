using MessagePipe;
using Messages;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using VContainer.Unity;

namespace Buyer
{
    public class SurfaceController : IStartable
    {
        private readonly BuyerSettings buyerSettings;
        public NavMeshSurface Surface;

        private bool updateQueued = false;
        
        public SurfaceController
            (
                BuyerSettings buyerSettings,
                ISubscriber<GridExtendMessage> gridExtendMessageSubscriber,
                ISubscriber<CreatedNewObjectOnGridMessage> createdNewObjectOnGridMessageSubscriber
            )
        {
            this.buyerSettings = buyerSettings;
            
            gridExtendMessageSubscriber.Subscribe(OnGridExtended);
            createdNewObjectOnGridMessageSubscriber.Subscribe(OnNewBuilding);
        }
        
        private void OnNewBuilding(CreatedNewObjectOnGridMessage msg)
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
            // Ждём два кадра – обычно достаточно
            yield return null;
            yield return null;

            Debug.Log("SurfaceController: NavMesh обновляется после задержки");

            Surface.UpdateNavMesh(Surface.navMeshData);
            updateQueued = false;
        }

        public void Start()
        {
            Surface = Object.Instantiate(buyerSettings.NavMeshSurface);
            Surface.navMeshData = new NavMeshData();
            NavMesh.AddNavMeshData(Surface.navMeshData);
            Surface.UpdateNavMesh(Surface.navMeshData);
        }
    }
}