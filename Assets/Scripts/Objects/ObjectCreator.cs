using BuildingsAndGrid.Buildings;
using MessagePipe;
using Messages;
using UniRx;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Objects
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ObjectCreator : IStartable
    {
        private readonly IObjectResolver resolver;
        private readonly IPublisher<CreatedNewObjectMessage> createdNewObjectPublisher;
        private readonly IPublisher<CreatedNewObjectOnGridMessage> createdNewObjectOnGridPublisher;
        private readonly CompositeDisposable disposables = new();

        public ObjectCreator
            (
                IObjectResolver resolver,
                IPublisher<CreatedNewObjectMessage> createdNewObjectPublisher,
                IPublisher<CreatedNewObjectOnGridMessage> createdNewObjectOnGridPublisher,
                ISubscriber<CreatedNewBuildingOnGridRequest> createdNewObjectOnGridSubscriber,
                ISubscriber<CreatedNewObjectRequest> playerMadePurchaseSubscriber,
                ISubscriber<DeleteBuildingOnGridRequest> deleteBuildingOnGridRequest
            )
        {
            this.resolver = resolver;
            this.createdNewObjectPublisher = createdNewObjectPublisher;
            this.createdNewObjectOnGridPublisher = createdNewObjectOnGridPublisher;

            playerMadePurchaseSubscriber.Subscribe(CreateObject).AddTo(disposables);
            createdNewObjectOnGridSubscriber.Subscribe(CreateObjectOnGrid).AddTo(disposables);
            deleteBuildingOnGridRequest.Subscribe(DeleteBuilding).AddTo(disposables);
        }

        private void CreateObject(CreatedNewObjectRequest msg)
        {
            var objScope = resolver.Instantiate(msg.Scope);
            var objTransform = objScope.gameObject.transform;
            createdNewObjectPublisher.Publish(new CreatedNewObjectMessage(objTransform, msg.Position, msg.Rotation));
        }

        private void CreateObjectOnGrid(CreatedNewBuildingOnGridRequest msg)
        {
            var buildingScope = resolver.Instantiate(msg.BuildingConfig.Building);
            var building = buildingScope.GetComponent<Building>();
            var buildingTransform = building.gameObject.transform;
            building.SetTiles(msg.Tiles);
            building.SetContentRotation(msg.Rotation);
            building.BuildingConfig = msg.BuildingConfig;
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
                                                                                      msg.Rotation));
        }

        private void DeleteBuilding(DeleteBuildingOnGridRequest msg)
        {
            foreach (var tile in msg.Building.Tiles)
            {
                tile.SetBuilding(null);
            }
            Object.Destroy(msg.Building.gameObject);
        }
        
        public void Start() { }
    }
}