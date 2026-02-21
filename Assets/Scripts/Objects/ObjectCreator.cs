using System.Collections.Generic;
using BuildingsAndGrid;
using BuildingsAndGrid.Buildings;
using Container.Game;
using MessagePipe;
using Messages;
using UniRx;
using UnityEngine;
using VContainer.Unity;

namespace Objects
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ObjectCreator : IStartable
    {
        private readonly GridSettings gridSettings;
        private readonly TilesController tilesController;
        private readonly GameLifetimeScope gameLifetimeScope;
        private readonly IPublisher<CreatedNewObjectMessage> createdNewObjectPublisher;
        private readonly IPublisher<CreatedNewObjectOnGridMessage> createdNewObjectOnGridPublisher;
        private readonly IPublisher<DeleteBuildingOnGridMessage> deleteBuildingOnGridMessagePublisher;
        private readonly CompositeDisposable disposables = new();

        public ObjectCreator
            (
                GameLifetimeScope gameLifetimeScope,
                IPublisher<CreatedNewObjectMessage> createdNewObjectPublisher,
                IPublisher<CreatedNewObjectOnGridMessage> createdNewObjectOnGridPublisher,
                IPublisher<DeleteBuildingOnGridMessage> deleteBuildingOnGridMessagePublisher,
                ISubscriber<CreatedNewBuildingOnGridRequest> createdNewObjectOnGridSubscriber,
                ISubscriber<CreatedNewObjectRequest> playerMadePurchaseSubscriber,
                ISubscriber<DeleteBuildingOnGridRequest> deleteBuildingOnGridRequest
            )
        {
            this.gameLifetimeScope = gameLifetimeScope;
            this.createdNewObjectPublisher = createdNewObjectPublisher;
            this.createdNewObjectOnGridPublisher = createdNewObjectOnGridPublisher;
            this.deleteBuildingOnGridMessagePublisher = deleteBuildingOnGridMessagePublisher;

            playerMadePurchaseSubscriber.Subscribe(CreateObject).AddTo(disposables);
            createdNewObjectOnGridSubscriber.Subscribe(CreateObjectOnGrid).AddTo(disposables);
            deleteBuildingOnGridRequest.Subscribe(DeleteBuilding).AddTo(disposables);
        }

        private void CreateObject(CreatedNewObjectRequest msg)
        {
            var objScope = gameLifetimeScope.CreateChildFromPrefab(msg.Scope);
            var objTransform = objScope.gameObject.transform;
            createdNewObjectPublisher.Publish(new CreatedNewObjectMessage(objTransform, msg.Position, msg.Rotation));
        }

        private void CreateObjectOnGrid(CreatedNewBuildingOnGridRequest msg)
        {
            var buildingScope = gameLifetimeScope.CreateChildFromPrefab(msg.BuildingConfig.Building);
            var building = buildingScope.GetComponent<Building>();
            var buildingTransform = building.gameObject.transform;
            building.SetTiles(msg.Tiles);
            building.Cell = msg.Cell;
            building.SetContentRotation(msg.Rotation);
            building.BuildingConfig = msg.BuildingConfig;
            building.LastCell = msg.LastCell;
            building.HaveLastPosition = msg.HaveLastPosition;
            
            if (msg.LocalPosition != default)
            {
                building.Content.localPosition = msg.LocalPosition;
            }
            foreach (var tile in msg.Tiles)
            {
                tile.SetBuilding(building);
            }
            createdNewObjectOnGridPublisher.Publish(new CreatedNewObjectOnGridMessage(building, 
                                                                                      buildingTransform, 
                                                                                      msg.Position, 
                                                                                      msg.Rotation,
                                                                                      msg.Cell,
                                                                                      msg.LastCell,
                                                                                      msg.NeedSave
                                                                                      ));
        }
        
        public Building CreateObjectOnGrid
            (
                BuildingConfig buildingConfig, 
                List<Tile> tiles,
                Vector2Int cell,
                Quaternion rotation,
                Vector3 localPosition,
                Vector3 position
            )
        {
            var buildingScope = gameLifetimeScope.CreateChildFromPrefab(buildingConfig.Building);
            var building = buildingScope.GetComponent<Building>();
            var buildingTransform = building.gameObject.transform;
            building.SetTiles(tiles);
            building.Cell = cell;
            building.SetContentRotation(rotation);
            building.BuildingConfig = buildingConfig;
            if (localPosition != default)
            {
                building.Content.localPosition = localPosition;
            }
            foreach (var tile in tiles)
            {
                tile.SetBuilding(building);
            }

            createdNewObjectOnGridPublisher.Publish(new CreatedNewObjectOnGridMessage(building, 
                                                         buildingTransform, 
                                                         position, 
                                                         rotation,
                                                         cell,
                                                         cell,
                                                         true
                                                        ));
            return building;
        }
        
        private void DeleteBuilding(DeleteBuildingOnGridRequest msg)
        {
            foreach (var tile in msg.Building.Tiles)
            {
                tile.SetBuilding(null);
            }
            if (msg.NeedRemoveFromSave)
                deleteBuildingOnGridMessagePublisher.Publish(new DeleteBuildingOnGridMessage(msg.Building.BuildingConfig.Id, msg.OldCell));
            Object.Destroy(msg.Building.gameObject);
        }
        
        public void Start() { }
    }
}