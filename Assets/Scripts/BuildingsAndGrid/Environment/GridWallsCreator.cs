using System.Collections.Generic;
using BuildingsAndGrid.Buildings;
using GameModes;
using MessagePipe;
using Messages;
using UnityEngine;
using VContainer.Unity;

namespace BuildingsAndGrid.Environment
{
    //TODO: Большой класс. Дублирования. Нужно бить на классы и методы.
    // ReSharper disable once ClassNeverInstantiated.Global
    public class GridWallsCreator : IStartable
    {
        private readonly GridSettings gridSettings;
        private readonly GridEnvironmentConfig gridEnvConfig;
        private readonly TilesController tilesController;

        private readonly IPublisher<CreatedNewBuildingOnGridRequest> createdNewBuildingOnGridPublisher;
        private readonly IPublisher<DeleteBuildingOnGridRequest> deleteBuildingOnGridPublisher;
        private readonly ISubscriber<GameModeChangedMessage> gameModeChangedMessageSubscriber;

        private List<Building> buildings = new();
        private GameObject environmentParent;
        private GameObject wallView;
        private GameObject externalWall;
        private bool lastModeIsRedactor;

        // private GameObject wallsObject;

        public GridWallsCreator
            (
                GridSettings gridSettings,
                GridEnvironmentConfig gridEnvConfig,
                TilesController tilesController,
                IPublisher<CreatedNewBuildingOnGridRequest> createdNewBuildingOnGridPublisher,
                ISubscriber<CreatedNewObjectOnGridMessage> createdNewObjectOnGridSubscriber,
                ISubscriber<GridExtendMessage> gridExtendSubscriber,
                ISubscriber<GameModeChangedMessage> gameModeChangedMessageSubscriber
            )
        {
            this.gridSettings = gridSettings;
            this.gridEnvConfig = gridEnvConfig;
            this.tilesController = tilesController;

            this.createdNewBuildingOnGridPublisher = createdNewBuildingOnGridPublisher;
            this.gameModeChangedMessageSubscriber = gameModeChangedMessageSubscriber;
            deleteBuildingOnGridPublisher = GlobalMessagePipe.GetPublisher<DeleteBuildingOnGridRequest>();

            createdNewObjectOnGridSubscriber.Subscribe(OnNewObjectCreatedOnGrid);
            gridExtendSubscriber.Subscribe(OnGridExtended);
        }
        
        public void Start()
        {
            environmentParent = new GameObject("Wall Environment Parent");
            CreateEnvironment();
            CreateCheckout();
            DrawWalls();
            gameModeChangedMessageSubscriber.Subscribe(OnGameModeChanged);
        }
        
        private void OnGameModeChanged(GameModeChangedMessage msg)
        {
            if (msg.GameMode == GameMode.Redactor)
            {
                UnDraw(wallView);
                lastModeIsRedactor = true;
            }
            else if (lastModeIsRedactor)
            {
                //Какой-то непонятный/неприятный баг.Долго не могу пофиксить.
                //Костыль. При смене режимов, переустанавливаю двери.
                ClearBuildings();
                CreateEnvironment();
                DrawWall();
                lastModeIsRedactor = false;
            }
        }
        
        private void OnGridExtended(GridExtendMessage msg)
        {
            ClearBuildings();
            CreateEnvironment();
            DrawWalls();
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
        
        private void OnNewObjectCreatedOnGrid(CreatedNewObjectOnGridMessage msg)
        {
            if (msg.Building.BuildingConfig.Type is Area.Wall)
            {
                msg.Transform.SetParent(environmentParent.transform);
                buildings.Add(msg.Building);
                DrawWalls();
            }
        }

        private void DrawWalls()
        {
            DrawWall(); 
            DrawExternalWall();
        }

        private void DrawWall()
        {
            UnDraw(wallView);
            wallView = DrawWalls("Wall View", gridSettings.WallFloorMaterial);
            wallView.AddComponent<MeshCollider>();
            wallView.layer = Mathf.RoundToInt(Mathf.Log(gridSettings.WallLayer.value, 2));
        }
        
        private void DrawExternalWall()
        {
            UnDraw(externalWall);
            externalWall = DrawExternalWalls();
            externalWall.AddComponent<MeshCollider>();
            externalWall.layer = Mathf.RoundToInt(Mathf.Log(gridSettings.WallForPlayerLayer.value, 2));
        }
        
        private void CreateEnvironment()
        {
            //BackDoor 1
            TryPlaceBuildingByPattern(new TileAroundInfoWithPosition(Vector2Int.zero,
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
            TryPlaceBuildingByPattern(new TileAroundInfoWithPosition(Vector2Int.zero,
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
            TryPlaceBuildingByPattern(new TileAroundInfoWithPosition(Vector2Int.zero,
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
            TryPlaceBuildingByPattern(new TileAroundInfoWithPosition(Vector2Int.zero,
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

        private void CreateCheckout()
        {
            TryPlaceBuildingByPattern(new TileAroundInfoWithPosition(Vector2Int.zero,
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
        }

        private bool TryPlaceBuildingByPattern
            (
                TileAroundInfoWithPosition mainCondition,
                List<TileAroundInfoWithPosition> conditions,
                BuildingConfig config,
                Quaternion rotation
            )
        {
            var found = tilesController.Tiles.TryGetTileByTilesCondition(mainCondition, conditions, out var tile);
            if (!found || tile == null)
                return false;

            var px = tile.Index.x + mainCondition.Offset.x;
            var py = tile.Index.y + mainCondition.Offset.y;
            var size = rotation.Equals(Quaternion.Euler(.0f, .0f, .0f)) ||
                       rotation.Equals(Quaternion.Euler(.0f, 180.0f, .0f))
                           ? config.Size
                           : new Vector2Int(config.Size.y, config.Size.x);
            var lc = config.HighlightBuilding.Content.localPosition;
            var localPosition = rotation.Equals(Quaternion.Euler(.0f, .0f, .0f)) ||
                                rotation.Equals(Quaternion.Euler(.0f, 180.0f, .0f))
                                    ? lc
                                    : new Vector3(lc.z, .0f, lc.x);
            var tilesForBuilding = tilesController.Tiles.GetTilesAround(new Vector2Int(px, py), size);

            createdNewBuildingOnGridPublisher.Publish(
                                                      new CreatedNewBuildingOnGridRequest(
                                                           config,
                                                           new Vector3(px * gridSettings.TileSize.x,
                                                                       0,
                                                                       py * gridSettings.TileSize.z),
                                                           localPosition,
                                                           rotation,
                                                           tilesForBuilding
                                                          ));
            return true;
        }

        private void UnDraw(GameObject wall)
        {
            if (wall != null)
                Object.Destroy(wall);
            wall = null;
        }

        private GameObject DrawExternalWalls()
        {
            var wallObject = new GameObject($"External Walls");
            wallObject.transform.SetParent(environmentParent.transform);

            var tiles = tilesController.Tiles;
            var tileSize = gridSettings.TileSize;

            var vertices = new List<Vector3>();
            var triangles = new List<int>();
            var uvs = new List<Vector2>();

            // направления для граней
            Vector2Int[] dirs =
            {
                new(1, 0),  // east
                new(-1, 0), // west
                new(0, 1),  // north
                new(0, -1)  // south
            };

            // смещения по осям для построения стен
            Vector3[] faceOffsets =
            {
                new(tileSize.x / 2, tileSize.y / 2, 0),  // east
                new(-tileSize.x / 2, tileSize.y / 2, 0), // west
                new(0, tileSize.y / 2, tileSize.z / 2),  // north
                new(0, tileSize.y / 2, -tileSize.z / 2)  // south
            };

            // нормали для граней (для правильного заказа треугольников)
            Vector3[] normals =
            {
                Vector3.right,
                Vector3.left,
                Vector3.forward,
                Vector3.back
            };

            // размеры граней
            Vector2[] faceSizes =
            {
                new(tileSize.z, tileSize.y), // east/west (Z × Y)
                new(tileSize.z, tileSize.y),
                new(tileSize.x, tileSize.y), // north/south (X × Y)
                new(tileSize.x, tileSize.y)
            };

            // Проход по всем плиткам
            for (var x = tiles.MinX; x < tiles.MaxX; x++)
            for (var y = tiles.MinY; y < tiles.MaxY; y++)
            {
                if (!tiles.TryGetTile(x, y, out var tile) || tile == null)
                    continue;
                if (tile.Type != Area.Wall)
                    continue;
                if (tile.Index.x > tiles.MinX && tile.Index.x < tiles.MaxX - 1 &&
                    tile.Index.y > tiles.MinY && tile.Index.y < tiles.MaxY - 1)
                    continue;
                
                // центр стены
                Vector3 tileCenter = new(
                                         (x + 0.5f) * tileSize.x,
                                         0,
                                         (y + 0.5f) * tileSize.z
                                        );

                // добавить стены только по внешним сторонам
                for (int i = 0; i < 4; i++)
                {
                    var dir = dirs[i];

                    // если сосед = стенка → пропускаем грань
                    if (tiles.TryGetTile(x + dir.x, y + dir.y, out var neighbor) &&
                        neighbor != null &&
                        neighbor.Type == Area.Wall &&
                        neighbor.Building == null)
                    {
                        continue; // внутренняя грань
                    }

                    // создаём грань
                    AddQuad(
                            vertices,
                            triangles,
                            uvs,
                            tileCenter + faceOffsets[i], // позиция грани
                            normals[i],
                            faceSizes[i]
                           );
                }
                // ===================== TOP FACE =====================
                {
                    // центр верхней грани
                    Vector3 topCenter = tileCenter + new Vector3(0, tileSize.y, 0);

                    // нормаль вверх
                    Vector3 topNormal = Vector3.up;

                    // размер крышки: X × Z
                    Vector2 topSize = new Vector2(tileSize.x, tileSize.z);

                    // добавить верхнюю грань
                    AddQuad(
                            vertices,
                            triangles,
                            uvs,
                            topCenter,
                            topNormal,
                            topSize
                           );
                }
            }
                
            // создаём Mesh
            var mesh = new Mesh();
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles, 0);
            mesh.SetUVs(0, uvs);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            
            // добавляем Mesh к одному объекту
            var mf = wallObject.AddComponent<MeshFilter>();
            var mr = wallObject.AddComponent<MeshRenderer>();
            mf.mesh = mesh;
            mr.material = gridSettings.GhostRedMaterial; // или WallMaterial
            return wallObject;
        }

        private GameObject DrawWalls(string wallName, Material wallMaterial)
        {
            var wallObject = new GameObject(wallName);
            wallObject.transform.SetParent(environmentParent.transform);

            var tiles = tilesController.Tiles;
            var tileSize = gridSettings.TileSize;

            var vertices = new List<Vector3>();
            var triangles = new List<int>();
            var uvs = new List<Vector2>();
            
            // направления для граней
            Vector2Int[] dirs =
            {
                new(1, 0),  // east
                new(-1, 0), // west
                new(0, 1),  // north
                new(0, -1)  // south
            };

            // смещения по осям для построения стен
            Vector3[] faceOffsets =
            {
                new(tileSize.x / 2, tileSize.y / 2, 0),  // east
                new(-tileSize.x / 2, tileSize.y / 2, 0), // west
                new(0, tileSize.y / 2, tileSize.z / 2),  // north
                new(0, tileSize.y / 2, -tileSize.z / 2)  // south
            };

            // нормали для граней (для правильного заказа треугольников)
            Vector3[] normals =
            {
                Vector3.right,
                Vector3.left,
                Vector3.forward,
                Vector3.back
            };

            // размеры граней
            Vector2[] faceSizes =
            {
                new(tileSize.z, tileSize.y), // east/west (Z × Y)
                new(tileSize.z, tileSize.y),
                new(tileSize.x, tileSize.y), // north/south (X × Y)
                new(tileSize.x, tileSize.y)
            };

            // Проход по всем плиткам
            for (int x = tiles.MinX; x < tiles.MaxX; x++)
            for (int y = tiles.MinY; y < tiles.MaxY; y++)
            {
                if (!tiles.TryGetTile(x, y, out var tile) || tile == null)
                    continue;

                if (tile.Type != Area.Wall || tile.Building != null)
                    continue;

                // центр стены
                Vector3 tileCenter = new(
                                         (x + 0.5f) * tileSize.x,
                                         0,
                                         (y + 0.5f) * tileSize.z
                                        );

                // добавить стены только по внешним сторонам
                for (int i = 0; i < 4; i++)
                {
                    var dir = dirs[i];

                    // если сосед = стенка → пропускаем грань
                    if (tiles.TryGetTile(x + dir.x, y + dir.y, out var neighbor) &&
                        neighbor != null &&
                        neighbor.Type == Area.Wall &&
                        neighbor.Building == null)
                    {
                        continue; // внутренняя грань
                    }

                    // создаём грань
                    AddQuad(
                            vertices,
                            triangles,
                            uvs,
                            tileCenter + faceOffsets[i], // позиция грани
                            normals[i],
                            faceSizes[i]
                           );
                }
                // ===================== TOP FACE =====================
                {
                    // центр верхней грани
                    Vector3 topCenter = tileCenter + new Vector3(0, tileSize.y, 0);

                    // нормаль вверх
                    Vector3 topNormal = Vector3.up;

                    // размер крышки: X × Z
                    Vector2 topSize = new Vector2(tileSize.x, tileSize.z);

                    // добавить верхнюю грань
                    AddQuad(
                            vertices,
                            triangles,
                            uvs,
                            topCenter,
                            topNormal,
                            topSize
                           );
                }
            }

            // создаём Mesh
            var mesh = new Mesh();
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles, 0);
            mesh.SetUVs(0, uvs);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            
            // добавляем Mesh к одному объекту
            var mf = wallObject.AddComponent<MeshFilter>();
            var mr = wallObject.AddComponent<MeshRenderer>();
            mf.mesh = mesh;
            mr.material = wallMaterial; // или WallMaterial
            return wallObject;
        }

        private void AddQuad(
            List<Vector3> vertices,
            List<int> triangles,
            List<Vector2> uvs,
            Vector3 center,
            Vector3 normal,
            Vector2 size)
        {
            normal.Normalize();

            Vector3 tangent;
            if (Mathf.Abs(Vector3.Dot(normal, Vector3.up)) > 0.99f)
                tangent = Vector3.right;
            else
                tangent = Vector3.up;

            Vector3 right = Vector3.Cross(normal, tangent).normalized;
            Vector3 up = Vector3.Cross(right, normal).normalized;

            Vector3 halfRight = right * (size.x / 2f);
            Vector3 halfUp = up * (size.y / 2f);

            int start = vertices.Count;

            // ---- ВЕРШИНЫ ----
            vertices.Add(center - halfRight - halfUp); //0
            vertices.Add(center - halfRight + halfUp); //1
            vertices.Add(center + halfRight + halfUp); //2
            vertices.Add(center + halfRight - halfUp); //3

            // ---- UV ----
            uvs.Add(new Vector2(0, 0)); //0
            uvs.Add(new Vector2(0, 1)); //1
            uvs.Add(new Vector2(1, 1)); //2
            uvs.Add(new Vector2(1, 0)); //3

            // ---- ТРЕУГОЛЬНИКИ ----
            triangles.Add(start + 0);
            triangles.Add(start + 1);
            triangles.Add(start + 2);

            triangles.Add(start + 0);
            triangles.Add(start + 2);
            triangles.Add(start + 3);
        }
    }
}