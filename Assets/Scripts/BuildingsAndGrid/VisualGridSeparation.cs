using System.Collections.Generic;
using GameModes;
using MessagePipe;
using Messages;
using UnityEngine;
using VContainer.Unity;

namespace BuildingsAndGrid
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class VisualGridSeparation : IStartable
    {
        private readonly GridSettings gridSettings;
        private readonly MeshFilter meshFilter;
        private readonly TilesController tilesController;

        private const float LineThickness = 0.03f;

        private VisualGridSeparation
            (
                GridSettings gridSettings,
                TilesController tilesController,
                MeshFilter meshFilter,
                ISubscriber<GameModeChangedMessage> GameModeChangedSubscriber,
                ISubscriber<GridExtendMessage> gridExtendSubscriber
            )
        {
            this.gridSettings = gridSettings;
            this.meshFilter = meshFilter;
            this.tilesController = tilesController;

            GameModeChangedSubscriber.Subscribe(OnGameModeChanged);
            gridExtendSubscriber.Subscribe(OnGridExtended);
            Draw();
        }

        private void OnGridExtended(GridExtendMessage msg) => Draw();
        
        private void OnGameModeChanged(GameModeChangedMessage msg)
            => OnGameModeChanged(msg.GameMode);

        private void OnGameModeChanged(GameMode gameMode)
        {
            if (gameMode is GameMode.Redactor or GameMode.Inventory)
                Draw();
            else
                UnDraw();
        }

        private void UnDraw()
        {
            meshFilter.mesh = null;
        }

        private void Draw()
        {
            var tiles = tilesController.Tiles;
            var tile = gridSettings.TileSize;
            var mesh = new Mesh { name = "GridMesh" };
            var vertices = new List<Vector3>();
            var triangles = new List<int>();
            var halfW = LineThickness * 0.5f;

            // === Горизонтальные линии ===
            for (var y = tiles.MinY; y <= tiles.MaxY; y++)
            {
                var worldZ = y * tile.z;

                var startX = tiles.MinX * tile.x;
                var endX = tiles.MaxX * tile.x;

                var p1 = new Vector3(startX, gridSettings.yOffset, worldZ);
                var p2 = new Vector3(endX,   gridSettings.yOffset, worldZ);

                AddThickLine(vertices, triangles, p1, p2, halfW);
            }

            // === Вертикальные линии ===
            for (var x = tiles.MinX; x <= tiles.MaxX; x++)
            {
                var worldX = x * tile.x;

                var startZ = tiles.MinY * tile.z;
                var endZ = tiles.MaxY * tile.z;

                var p1 = new Vector3(worldX, gridSettings.yOffset, startZ);
                var p2 = new Vector3(worldX, gridSettings.yOffset, endZ);

                AddThickLine(vertices, triangles, p1, p2, halfW);
            }

            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles, 0);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            meshFilter.mesh = mesh;
        }

        // ----------------------------------------
        //  Creates a thick line (quad) between p1-p2
        // ----------------------------------------
        private static void AddThickLine
        (
            List<Vector3> verts,
            List<int> tris,
            Vector3 p1,
            Vector3 p2,
            float halfW
        )
        {
            var dir = (p2 - p1).normalized;
            var perpendicular = new Vector3(-dir.z, 0, dir.x) * halfW;
            var index = verts.Count;

            verts.Add(p1 - perpendicular);
            verts.Add(p1 + perpendicular);
            verts.Add(p2 - perpendicular);
            verts.Add(p2 + perpendicular);

            tris.Add(index + 0);
            tris.Add(index + 1);
            tris.Add(index + 2);

            tris.Add(index + 2);
            tris.Add(index + 1);
            tris.Add(index + 3);
        }

        public void Start() { }
    }
}
