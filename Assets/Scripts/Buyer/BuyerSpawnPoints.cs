using System.Collections.Generic;
using BuildingsAndGrid;
using MessagePipe;
using Messages;
using UnityEngine;
using VContainer.Unity;

namespace Buyer
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class BuyerSpawnPoints : IStartable
    {
        private readonly GridSettings gridSettings;
        private readonly BuyerSettings buyerSettings;
        private readonly TilesController tilesController;

        private GameObject SpawnPointsParent;
        public readonly List<TargetPoint> SpawnPoints = new();

        public BuyerSpawnPoints
            (
                GridSettings gridSettings,
                BuyerSettings buyerSettings,
                TilesController tilesController,
                ISubscriber<GridExtendMessage> gridExtendMessageSubscriber
            )
        {
            this.gridSettings = gridSettings;
            this.buyerSettings = buyerSettings;
            this.tilesController = tilesController;

            gridExtendMessageSubscriber.Subscribe(OnGridExtended);
        }

        public void Start()
        {
            SpawnPointsParent = new GameObject("Spawn Places For Buyers Parent");
            CreateSpawnPlaces();
        }

        private void OnGridExtended(GridExtendMessage msg)
        {
            CreateSpawnPlaces();
        }
        
        private void CreateSpawnPlaces()
        {
            Clear();
            for (var i = 0; i < buyerSettings.SpawnPlacesCount; i++)
            {
                var place = new GameObject("SpawnPlace");
                var z = tilesController.Tiles.MaxY * gridSettings.TileSize.z + buyerSettings.SpaceBetweenGrid.z;
                var x = i * buyerSettings.SpaceBetweenSpawnPlaces.x;
                place.transform.position = new Vector3(x, .0f, z);
                place.transform.SetParent(SpawnPointsParent.transform);
                SpawnPoints.Add(new TargetPoint(place.transform));
            }
        }

        private void Clear()
        {
            foreach (var place in SpawnPoints)
            {
                Object.Destroy(place.Target.gameObject);
            }
            SpawnPoints.Clear();
        }
    }
}