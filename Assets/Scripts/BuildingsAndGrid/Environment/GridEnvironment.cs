using MessagePipe;
using Messages;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace BuildingsAndGrid.Environment
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class GridEnvironment : IStartable
    {
        private readonly GridSettings gridSettings;
        private readonly TilesController tilesController;
        private readonly IObjectResolver resolver;

        private GameObject parent;

        public GridEnvironment
            (
                GridSettings gridSettings,
                TilesController tilesController,
                IObjectResolver resolver,
                ISubscriber<GridExtendMessage> gridExtendMessageSubscriber
            )
        {
            this.gridSettings = gridSettings;
            this.tilesController = tilesController;
            this.resolver = resolver;

            gridExtendMessageSubscriber.Subscribe(OnGridExtended);
        }

        public void Start()
        {
            CreateEnvironment();
        }

        private void OnGridExtended(GridExtendMessage msg)
        {
            CreateEnvironment();
        }
        
        private void CreateEnvironment()
        {
            if (parent)
                Object.Destroy(parent);
            parent = new GameObject("Environment Parent");

            var tiles = tilesController.Tiles;
            Vector3 tileSize = gridSettings.TileSize;

            int minX = tiles.MinX;
            int maxX = tiles.MaxX;
            int minY = tiles.MinY;
            int maxY = tiles.MaxY;

            // Размеры сетки
            float gridWidth = (maxX - minX) * tileSize.x;
            float gridHeight = (maxY - minY) * tileSize.z;

            float thicknessX = gridSettings.EnvironmentAddSizeSize.x * tileSize.x;
            float thicknessY = gridSettings.EnvironmentAddSizeSize.y * tileSize.z;

            float centerZ = (minY + maxY) * 0.5f * tileSize.z;
            float centerX = (minX + maxX) * 0.5f * tileSize.x;

            // --- RIGHT WALL ---
            CreateVerticalWall(
                               "Right Wall",
                               new Vector3(maxX * tileSize.x + thicknessX * 0.5f, 0, centerZ),
                               gridHeight,
                               thicknessX,
                               parent.transform
                              );

            // --- LEFT WALL ---
            CreateVerticalWall(
                               "Left Wall",
                               new Vector3(minX * tileSize.x - thicknessX * 0.5f, 0, centerZ),
                               gridHeight,
                               thicknessX,
                               parent.transform
                              );

            // --- TOP WALL (Север, по оси Y бонус сверху) ---
            CreateHorizontalWall(
                                 "Top Wall",
                                 new Vector3(centerX, 0, maxY * tileSize.z + thicknessY * 0.5f),
                                 gridWidth,
                                 thicknessY,
                                 parent.transform
                                );

            // --- BOTTOM WALL (Юг, снизу) ---
            CreateHorizontalWall(
                                 "Bottom Wall",
                                 new Vector3(centerX, 0, minY * tileSize.z - thicknessY * 0.5f),
                                 gridWidth,
                                 thicknessY,
                                 parent.transform
                                );
            // === CORNERS ===

            float cornerX = gridSettings.EnvironmentAddSizeSize.x * tileSize.x;
            float cornerZ = gridSettings.EnvironmentAddSizeSize.y * tileSize.z;

// Bottom-Left
            CreateCorner(
                         "Corner Bottom-Left",
                         new Vector3(
                                     minX * tileSize.x - cornerX * 0.5f,
                                     0,
                                     minY * tileSize.z - cornerZ * 0.5f
                                    ),
                         cornerX,
                         cornerZ,
                         parent.transform
                        );

// Bottom-Right
            CreateCorner(
                         "Corner Bottom-Right",
                         new Vector3(
                                     maxX * tileSize.x + cornerX * 0.5f,
                                     0,
                                     minY * tileSize.z - cornerZ * 0.5f
                                    ),
                         cornerX,
                         cornerZ,
                         parent.transform
                        );

// Top-Left
            CreateCorner(
                         "Corner Top-Left",
                         new Vector3(
                                     minX * tileSize.x - cornerX * 0.5f,
                                     0,
                                     maxY * tileSize.z + cornerZ * 0.5f
                                    ),
                         cornerX,
                         cornerZ,
                         parent.transform
                        );

// Top-Right
            CreateCorner(
                         "Corner Top-Right",
                         new Vector3(
                                     maxX * tileSize.x + cornerX * 0.5f,
                                     0,
                                     maxY * tileSize.z + cornerZ * 0.5f
                                    ),
                         cornerX,
                         cornerZ,
                         parent.transform
                        );
        }

        private void CreateVerticalWall(string name, Vector3 center, float height, float thickness, Transform parent)
        {
            GameObject wall = new GameObject(name);
            wall.transform.SetParent(parent);

            var mf = wall.AddComponent<MeshFilter>();
            var mr = wall.AddComponent<MeshRenderer>();
            mr.sharedMaterial = gridSettings.GreenMaterial;

            Mesh mesh = new Mesh();
            mf.mesh = mesh;

            float halfH = height * 0.5f;
            float halfT = thickness * 0.5f;

            Vector3[] vertices =
            {
                new(center.x - halfT, 0, center.z - halfH),
                new(center.x - halfT, 0, center.z + halfH),
                new(center.x + halfT, 0, center.z + halfH),
                new(center.x + halfT, 0, center.z - halfH),
            };

            int[] triangles = { 0, 1, 2, 0, 2, 3 };
            Vector2[] uvs = { new(0, 0), new(0, 1), new(1, 1), new(1, 0) };

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uvs;

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
        }

        private void CreateHorizontalWall(string name, Vector3 center, float width, float thickness, Transform parent)
        {
            GameObject wall = new GameObject(name);
            wall.transform.SetParent(parent);

            var mf = wall.AddComponent<MeshFilter>();
            var mr = wall.AddComponent<MeshRenderer>();
            mr.sharedMaterial = gridSettings.GreenMaterial;

            Mesh mesh = new Mesh();
            mf.mesh = mesh;

            float halfW = width * 0.5f;
            float halfT = thickness * 0.5f;

            Vector3[] vertices =
            {
                new(center.x - halfW, 0, center.z - halfT),
                new(center.x - halfW, 0, center.z + halfT),
                new(center.x + halfW, 0, center.z + halfT),
                new(center.x + halfW, 0, center.z - halfT),
            };

            int[] triangles = { 0, 1, 2, 0, 2, 3 };
            Vector2[] uvs = { new(0, 0), new(0, 1), new(1, 1), new(1, 0) };

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uvs;

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
        }

        private void CreateCorner(string name, Vector3 center, float sizeX, float sizeZ, Transform parent)
        {
            var corner = new GameObject(name);
            corner.transform.SetParent(parent);

            var mf = corner.AddComponent<MeshFilter>();
            var mr = corner.AddComponent<MeshRenderer>();
            mr.sharedMaterial = gridSettings.GreenMaterial;

            Mesh mesh = new Mesh();
            mf.mesh = mesh;

            float halfX = sizeX * 0.5f;
            float halfZ = sizeZ * 0.5f;

            Vector3[] vertices =
            {
                new(center.x - halfX, 0, center.z - halfZ),
                new(center.x - halfX, 0, center.z + halfZ),
                new(center.x + halfX, 0, center.z + halfZ),
                new(center.x + halfX, 0, center.z - halfZ),
            };

            int[] triangles = { 0, 1, 2, 0, 2, 3 };
            Vector2[] uvs = { new(0, 0), new(0, 1), new(1, 1), new(1, 0) };

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uvs;

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
        }
    }
}