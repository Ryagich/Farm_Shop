using System.Collections.Generic;
using System.Linq;
using BuildingsAndGrid.Buildings;
using GameModes;
using MessagePipe;
using Messages;
using Objects;
using Storage;
using UnityEngine;
using VContainer.Unity;
using YG;

namespace BuildingsAndGrid.Environment
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class DefaultBuildingsCreator : IStartable
    {
        private readonly GridEnvironmentConfig gridEnvConfig;
        private readonly GridSettings gridSettings;
        private readonly TilesController tilesController;
        private readonly BuildingPlacer buildingPlacer;
        private readonly Storage.Storage storage;
        private readonly IPublisher<CreatedNewBuildingOnGridRequest> createdNewBuildingOnGridPublisher;

        private readonly IPublisher<DeleteBuildingOnGridRequest> deleteBuildingOnGridPublisher;
        
        private readonly ISubscriber<GameModeChangedMessage> gameModeChangedMessageSubscriber;
        
        private GameObject environmentParent;
        private bool lastModeIsRedactor;
        private readonly List<Building> wallBuildings = new();

        public DefaultBuildingsCreator
            (
                GridEnvironmentConfig gridEnvConfig,
                GridSettings gridSettings,
                TilesController tilesController,
                BuildingPlacer buildingPlacer,
                Storage.Storage storage,
                IPublisher<CreatedNewBuildingOnGridRequest> createdNewBuildingOnGridPublisher,
                ISubscriber<GridExtendMessage> gridExtendSubscriber,
                ISubscriber<CreatedNewObjectOnGridMessage> createdNewObjectOnGridSubscriber,
                ISubscriber<GameModeChangedMessage> gameModeChangedMessageSubscriber,
                ISubscriber<DeleteBuildingOnGridMessage> deleteBuildingOnGridMessageSubscriber
            )
        {
            this.gridEnvConfig = gridEnvConfig;
            this.gridSettings = gridSettings;
            this.tilesController = tilesController;
            this.buildingPlacer = buildingPlacer;
            this.storage = storage;
            this.createdNewBuildingOnGridPublisher = createdNewBuildingOnGridPublisher;
            this.gameModeChangedMessageSubscriber = gameModeChangedMessageSubscriber;
            deleteBuildingOnGridPublisher = GlobalMessagePipe.GetPublisher<DeleteBuildingOnGridRequest>();

            createdNewObjectOnGridSubscriber.Subscribe(OnNewObjectCreatedOnGrid);
            gridExtendSubscriber.Subscribe(OnGridExtended);
            deleteBuildingOnGridMessageSubscriber.Subscribe(OnObjectDelete);
        }
        
        public async void Start()
        { 
            await StorageAwaiter.WaitReadyAsync();
            
            environmentParent = new GameObject("Wall Environment Parent");
            
            if (YG2.saves.BuildingSaves is null || YG2.saves.BuildingSaves.Count is 0)
            {
                CreateDefaultBuildings();
                CreateEnvironment();
            }
            else
            {
                CreateSavedBuildings();
            }
            
            gameModeChangedMessageSubscriber.Subscribe(OnGameModeChanged);
        }
        
        private void OnGameModeChanged(GameModeChangedMessage msg)
        {
            // if (msg.GameMode == GameMode.Redactor)
            // {
            //     lastModeIsRedactor = true;
            // }
            // else if (lastModeIsRedactor)
            // {
            //     lastModeIsRedactor = false;
            // }
        }

        private void CreateSavedBuildings()
        {
            Debug.Log($"CreateSavedBuildings | buildings in saves {YG2.saves.BuildingSaves.Count}");

            foreach (var buildingSave in YG2.saves.BuildingSaves)
            {
                var buildingConfig = storage.GetBuildingConfigById(buildingSave.Id);
                var rotation = Quaternion.Euler(buildingSave.RotX, buildingSave.RotY, buildingSave.RotZ);
                var lc = buildingConfig.HighlightBuilding.Content.localPosition;
                var localPosition = rotation.Equals(Quaternion.Euler(.0f, .0f, .0f)) ||
                                    rotation.Equals(Quaternion.Euler(.0f, 180.0f, .0f))
                                        ? lc
                                        : new Vector3(lc.z, .0f, lc.x);
                var currentSize = rotation.Equals(Quaternion.Euler(.0f, .0f, .0f)) ||
                                  rotation.Equals(Quaternion.Euler(.0f, 180.0f, .0f))
                                      ? buildingConfig.Size
                                      : new Vector2Int(buildingConfig.Size.y, buildingConfig.Size.x);
                var tiles = tilesController.Tiles.GetTilesAround(buildingSave.Cell, currentSize);
                
                createdNewBuildingOnGridPublisher.Publish(new CreatedNewBuildingOnGridRequest
                                                              (
                                                               buildingConfig,
                                                               new Vector3(buildingSave.Cell.x * gridSettings.TileSize.x,
                                                                           0,
                                                                           buildingSave.Cell.y * gridSettings.TileSize.z),
                                                               localPosition,
                                                               rotation,
                                                               tiles,
                                                               new Vector2Int(buildingSave.Cell.x, buildingSave.Cell.y)
                                                              ));
            }
        }
        
        private void OnNewObjectCreatedOnGrid(CreatedNewObjectOnGridMessage msg)
        {
            if (YG2.saves.BuildingSaves.FirstOrDefault(s => s.Id.Equals(msg.Building.BuildingConfig.Id) &&
                                                            s.Cell.Equals(msg.Cell)) is null)
            {
                YG2.saves.BuildingSaves.Add(new BuildingSave(msg.Building.BuildingConfig.Id, msg.Cell, msg.Rotation));
                YG2.SaveProgress();
            }
            
            if (msg.Building.BuildingConfig.Type is Area.Wall)
            {
                msg.Transform.SetParent(environmentParent.transform);
                wallBuildings.Add(msg.Building);
            }
        }

        private void OnObjectDelete(DeleteBuildingOnGridMessage msg)
        {
            var save = YG2.saves.BuildingSaves.FirstOrDefault(s => s.Id.Equals(msg.ID) && s.Cell.Equals(msg.Cell));
            if (save is null)
            {
                Debug.Log($"Удалилась постройка, не записаная в сохранениях | wtf");
            }
            else
            {
                YG2.saves.BuildingSaves.Remove(save);
                YG2.SaveProgress();
            }
        }
        
        private void ClearWallBuildings()
        {
            foreach (var building in wallBuildings)
            {
                if (building)
                    deleteBuildingOnGridPublisher.Publish(new DeleteBuildingOnGridRequest(building, true));
            }
            wallBuildings.Clear();
        }
        
        private void OnGridExtended(GridExtendMessage msg)
        {
            ClearWallBuildings();
            CreateEnvironment();
        }
        
        private void CreateDefaultBuildings()
        {
            //Checkout
            buildingPlacer.TryPlaceBuildingByPattern(new TileAroundInfoWithPosition(Vector2Int.zero,
                                                      new List<TileAroundInfo>
                                                      {
                                                          new(Area.Shop, 4),
                                                      }),
                                                     new List<TileAroundInfoWithPosition>
                                                     {
                                                         new(new Vector2Int(-2, -2),
                                                             new List<TileAroundInfo>
                                                             {
                                                                 new(Area.Wall, 3),
                                                                 new(Area.Garden, 1),
                                                             })
                                                     },
                                                     gridEnvConfig.Checkout,
                                                     Quaternion.identity);
            //Shelf For Carrot
            buildingPlacer.TryPlaceBuildingByPattern(new TileAroundInfoWithPosition(Vector2Int.zero,
                                                      new List<TileAroundInfo>
                                                      {
                                                          new(Area.Shop, 4),
                                                      }),
                                                     new List<TileAroundInfoWithPosition>
                                                     {
                                                         new(new Vector2Int(gridEnvConfig.CarrotShelf.Size.x + 2,
                                                                                 - 3),
                                                             new List<TileAroundInfo>
                                                             {
                                                                 new(Area.Wall, 3),
                                                                 new(Area.None, 1),
                                                             })
                                                     },
                                                     gridEnvConfig.CarrotShelf,
                                                     Quaternion.identity);
            //Landing For Carrot
            buildingPlacer.TryPlaceBuildingByPattern(new TileAroundInfoWithPosition(Vector2Int.zero,
                                                      new List<TileAroundInfo>
                                                      {
                                                          new(Area.Garden, 4),
                                                      }),
                                                     new List<TileAroundInfoWithPosition>
                                                     {
                                                         new(new Vector2Int(-7, gridEnvConfig.CarrotLanding.Size.y + 2),
                                                             new List<TileAroundInfo>
                                                             {
                                                                 new(Area.Wall, 3),
                                                                 new(Area.Garden, 1),
                                                             })
                                                     },
                                                     gridEnvConfig.CarrotLanding,
                                                     Quaternion.identity);
            //Deleter
            buildingPlacer.TryPlaceBuildingByPattern(new TileAroundInfoWithPosition(Vector2Int.zero,
                                                      new List<TileAroundInfo>
                                                      {
                                                          new(Area.Production, 4),
                                                      }),
                                                     new List<TileAroundInfoWithPosition>
                                                     {
                                                         new(new Vector2Int(3, -7),
                                                             new List<TileAroundInfo>
                                                             {
                                                                 new(Area.Wall, 3),
                                                                 new(Area.Garden, 1),
                                                             })
                                                     },
                                                     gridEnvConfig.Deleter,
                                                     Quaternion.identity);
            
        }
        
        private void CreateEnvironment()
        {
            //BackDoor 1
            buildingPlacer.TryPlaceBuildingByPattern(new TileAroundInfoWithPosition(Vector2Int.zero,
                                                      new List<TileAroundInfo>
                                                      {
                                                          new(Area.Wall, 2),
                                                          new(Area.Production, 1),
                                                          new(Area.Garden, 1),
                                                      }),
                                                     new List<TileAroundInfoWithPosition>
                                                     {
                                                         new(new Vector2Int(gridEnvConfig.BackDoor.Size.x, 0),
                                                             new List<TileAroundInfo>
                                                             {
                                                                 new(Area.Wall, 3),
                                                                 new(Area.Garden, 1),
                                                             })
                                                     },
                                                     gridEnvConfig.BackDoor,
                                                     Quaternion.identity);
            //BackDoor 2
            buildingPlacer.TryPlaceBuildingByPattern(new TileAroundInfoWithPosition(Vector2Int.zero,
                                                      new List<TileAroundInfo>
                                                      {
                                                          new(Area.Wall, 2),
                                                          new(Area.Shop, 1),
                                                          new(Area.Garden, 1),
                                                      }),
                                                     new List<TileAroundInfoWithPosition>
                                                     {
                                                         new(new Vector2Int(-1, 0),
                                                             new List<TileAroundInfo>
                                                             {
                                                                 new(Area.Wall, 3),
                                                                 new(Area.Garden, 1),
                                                             })
                                                     },
                                                     gridEnvConfig.BackDoor,
                                                     Quaternion.identity);
            //BackDoor 3
            buildingPlacer.TryPlaceBuildingByPattern(new TileAroundInfoWithPosition(Vector2Int.zero,
                                                      new List<TileAroundInfo>
                                                      {
                                                          new(Area.Wall, 2),
                                                          new(Area.Shop, 1),
                                                          new(Area.Production, 1),
                                                      }),
                                                     new List<TileAroundInfoWithPosition>
                                                     {
                                                         new(new Vector2Int(0, -1),
                                                             new List<TileAroundInfo>
                                                             {
                                                                 new(Area.Wall, 3),
                                                                 new(Area.Garden, 1),
                                                             })
                                                     },
                                                     gridEnvConfig.BackDoor,
                                                     Quaternion.Euler(.0f, 90.0f, .0f));
            //Shop Door
            buildingPlacer.TryPlaceBuildingByPattern(new TileAroundInfoWithPosition(Vector2Int.zero,
                                                      new List<TileAroundInfo>
                                                      {
                                                          new(Area.Wall, 2),
                                                          new(Area.Shop, 1),
                                                          new(Area.None, 1),
                                                      }),
                                                     new List<TileAroundInfoWithPosition>
                                                     {
                                                         new(new Vector2Int(-1, 0),
                                                             new List<TileAroundInfo>
                                                             {
                                                                 new(Area.Wall, 3),
                                                                 new(Area.None, 1),
                                                             })
                                                     },
                                                     gridEnvConfig.ShopDoorConfig,
                                                     Quaternion.identity);
        }
    }
}