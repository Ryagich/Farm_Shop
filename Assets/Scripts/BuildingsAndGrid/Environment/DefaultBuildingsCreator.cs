using System.Collections.Generic;
using BuildingsAndGrid.Buildings;
using GameModes;
using MessagePipe;
using Messages;
using Objects;
using UnityEngine;
using VContainer.Unity;

namespace BuildingsAndGrid.Environment
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class DefaultBuildingsCreator : IStartable
    {
        private readonly GridEnvironmentConfig gridEnvConfig;
        private readonly BuildingPlacer buildingPlacer;
       
        private readonly IPublisher<DeleteBuildingOnGridRequest> deleteBuildingOnGridPublisher;
        
        private readonly ISubscriber<GameModeChangedMessage> gameModeChangedMessageSubscriber;
        
        private GameObject environmentParent;
        private bool lastModeIsRedactor;
        private List<Building> buildings = new();

        public DefaultBuildingsCreator
            (
                GridEnvironmentConfig gridEnvConfig,
                BuildingPlacer buildingPlacer,
                ISubscriber<GridExtendMessage> gridExtendSubscriber,
                ISubscriber<CreatedNewObjectOnGridMessage> createdNewObjectOnGridSubscriber,
                ISubscriber<GameModeChangedMessage> gameModeChangedMessageSubscriber
            )
        {
            this.gridEnvConfig = gridEnvConfig;
            this.buildingPlacer = buildingPlacer;
            this.gameModeChangedMessageSubscriber = gameModeChangedMessageSubscriber;
            deleteBuildingOnGridPublisher = GlobalMessagePipe.GetPublisher<DeleteBuildingOnGridRequest>();

            createdNewObjectOnGridSubscriber.Subscribe(OnNewObjectCreatedOnGrid);
            gridExtendSubscriber.Subscribe(OnGridExtended);
        }
        
        public void Start()
        { 
            environmentParent = new GameObject("Wall Environment Parent");
            CreateDefaultBuildings();
            CreateEnvironment();
            gameModeChangedMessageSubscriber.Subscribe(OnGameModeChanged);
        }
        
        private void OnGameModeChanged(GameModeChangedMessage msg)
        {
            if (msg.GameMode == GameMode.Redactor)
            {
                lastModeIsRedactor = true;
            }
            else if (lastModeIsRedactor)
            {
                //Какой-то непонятный/неприятный баг.Долго не могу пофиксить.
                //Костыль. При смене режимов, переустанавливаю двери.
                ClearBuildings();
                CreateEnvironment();
                lastModeIsRedactor = false;
            }
        }
        
        private void OnNewObjectCreatedOnGrid(CreatedNewObjectOnGridMessage msg)
        {
            if (msg.Building.BuildingConfig.Type is Area.Wall)
            {
                msg.Transform.SetParent(environmentParent.transform);
                buildings.Add(msg.Building);
            }
        }
        
        private void ClearBuildings()
        {
            foreach (var building in buildings)
            {
                if (building)
                    deleteBuildingOnGridPublisher.Publish(new DeleteBuildingOnGridRequest(building));
            }
            buildings.Clear();
        }
        
        private void OnGridExtended(GridExtendMessage msg)
        {
            ClearBuildings();
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