using System;
using System.Collections.Generic;
using System.Linq;
using GameModes;
using MessagePipe;
using Messages;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace BuildingsAndGrid
{
    // Рисует полы для нескольких типов Area: Shop, Garden, Production.
    // Для каждого типа разбивает тайлы на области (4-соседи), для каждой области генерирует один mesh.
    // ReSharper disable once ClassNeverInstantiated.Global
    public class VisualFloor : IStartable, IDisposable
    {
        private readonly GridSettings gridSettings;
        private readonly TilesController tilesController;
        private readonly Transform parentTransform;

        // Корни mesh-областей: по типам
        private readonly Dictionary<Area, List<GameObject>> regionRootsByArea = new();

        // Какие типы рисуем (можно расширить)
        private readonly Area[] floorAreas = { Area.Shop, Area.Garden, Area.Production, Area.Wall };

        // Конструктор (DI)
        private VisualFloor(
            GridSettings gridSettings,
            TilesController tilesController,
            [Key("GridRoot")] Transform parentTransform,
            ISubscriber<GridExtendMessage> gridExtendSub
        )
        {
            this.gridSettings = gridSettings;
            this.tilesController = tilesController ?? throw new ArgumentNullException(nameof(tilesController));
            this.parentTransform = parentTransform;

            // Подписка на расширение сетки
            gridExtendSub.Subscribe(_ => RebuildAllAreas());
            RebuildAllAreas();
        }

        // ========================== PUBLIC ==========================
        private void RebuildAllAreas()
        {
            ClearAllMeshes();

            foreach (var area in floorAreas)
                RebuildRegionsFor(area);
        }

        // ========================== BUILD PER AREA ==========================
        private void RebuildRegionsFor(Area area)
        {
            var tiles = tilesController.Tiles;
            if (tiles == null)
                return;

            // 1. Собираем тайлы нужного типа
            var areaTiles = new List<Vector2Int>();
            for (var x = tiles.MinX; x < tiles.MaxX; x++)
            {
                for (var y = tiles.MinY; y < tiles.MaxY; y++)
                {
                    var t = SafeGetTile(tiles, x, y);
                    if (t != null && t.Type == area)
                        areaTiles.Add(new Vector2Int(x, y));
                }
            }

            if (areaTiles.Count == 0)
                return;

            // 2. Разбиваем на регионы
            var regions = FloodFillRegions(areaTiles);

            // 3. Строим меши для каждого региона
            foreach (var region in regions)
                BuildMeshForRegion(area, region);
        }

        // ========================== HELPERS ==========================
        private Tile SafeGetTile(Tiles tiles, int x, int y)
        {
            try { return tiles.GetTile(x, y); }
            catch { return null; }
        }

        private static List<List<Vector2Int>> FloodFillRegions(List<Vector2Int> coords)
        {
            var set = new HashSet<Vector2Int>(coords);
            var regions = new List<List<Vector2Int>>();

            var dirs = new (int x, int y)[] { (1, 0), (-1, 0), (0, 1), (0, -1) };

            while (set.Count > 0)
            {
                var seed = set.First();
                var stack = new Stack<Vector2Int>();
                var region = new List<Vector2Int>();

                stack.Push(seed);
                set.Remove(seed);

                while (stack.Count > 0)
                {
                    var cur = stack.Pop();
                    region.Add(cur);

                    foreach (var d in dirs)
                    {
                        var nb = new Vector2Int(cur.x + d.x, cur.y + d.y);
                        if (set.Contains(nb))
                        {
                            set.Remove(nb);
                            stack.Push(nb);
                        }
                    }
                }

                regions.Add(region);
            }

            return regions;
        }

        // ========================== BUILD MESH ==========================
        private void BuildMeshForRegion(Area area, List<Vector2Int> regionTiles)
        {
            if (regionTiles == null || regionTiles.Count == 0)
                return;

            // bounding box
            int minX = regionTiles.Min(p => p.x);
            int minY = regionTiles.Min(p => p.y);
            int maxX = regionTiles.Max(p => p.x);
            int maxY = regionTiles.Max(p => p.y);

            int w = maxX - minX + 1;
            int h = maxY - minY + 1;

            // local mask
            var mask = new bool[w, h];
            foreach (var p in regionTiles)
                mask[p.x - minX, p.y - minY] = true;

            // find horizontal runs
            var runsPerRow = new List<List<(int x0, int x1)>>();
            for (int y = 0; y < h; y++)
            {
                var rowRuns = new List<(int, int)>();
                int x = 0;
                while (x < w)
                {
                    if (!mask[x, y]) { x++; continue; }

                    int start = x;
                    while (x < w && mask[x, y]) x++;

                    int end = x - 1;
                    rowRuns.Add((start, end));
                }

                runsPerRow.Add(rowRuns);
            }

            // merge vertical
            var rects = new List<RectInt>();
            var active = new List<(int x0, int x1, int y0)>();

            for (int y = 0; y < h; y++)
            {
                var newActive = new List<(int x0, int x1, int y0)>();
                var runs = runsPerRow[y];

                foreach (var a in active)
                {
                    bool extended = false;
                    for (int i = 0; i < runs.Count; i++)
                    {
                        var r = runs[i];
                        if (r.x0 == a.x0 && r.x1 == a.x1)
                        {
                            newActive.Add((a.x0, a.x1, a.y0));
                            runs.RemoveAt(i);
                            extended = true;
                            break;
                        }
                    }
                    if (!extended)
                    {
                        rects.Add(new RectInt(a.x0, a.y0, a.x1 - a.x0 + 1, y - a.y0));
                    }
                }

                foreach (var r in runs)
                    newActive.Add((r.x0, r.x1, y));

                active = newActive;
            }

            // finalize last row
            foreach (var a in active)
                rects.Add(new RectInt(a.x0, a.y0, a.x1 - a.x0 + 1, h - a.y0));

            if (rects.Count == 0)
                return;

            // =================== BUILD MESH ===================
            var verts = new List<Vector3>();
            var tris = new List<int>();
            var uvs = new List<Vector2>();
            var vertexIndex = new Dictionary<(float x, float z), int>();

            float tileX = gridSettings.TileSize.x;
            float tileZ = gridSettings.TileSize.z;
            float yOffset = gridSettings.yOffset - 0.02f;

            Vector3 LocalToWorld(int lx, int ly) =>
                new Vector3((minX + lx) * tileX, yOffset, (minY + ly) * tileZ);

            float plankSize = gridSettings.ShopPlankSize;

            int AddVertex(Vector3 pos)
            {
                var key = (pos.x, pos.z);
                if (vertexIndex.TryGetValue(key, out int idx))
                    return idx;

                idx = verts.Count;
                verts.Add(pos);
                uvs.Add(new Vector2(pos.x / plankSize, pos.z / plankSize));
                vertexIndex[key] = idx;
                return idx;
            }

            foreach (var r in rects)
            {
                Vector3 bl = LocalToWorld(r.x, r.y);
                Vector3 br = LocalToWorld(r.x + r.width, r.y);
                Vector3 tr = LocalToWorld(r.x + r.width, r.y + r.height);
                Vector3 tl = LocalToWorld(r.x, r.y + r.height);

                int i0 = AddVertex(bl);
                int i1 = AddVertex(br);
                int i2 = AddVertex(tr);
                int i3 = AddVertex(tl);

                tris.Add(i0); tris.Add(i2); tris.Add(i1);
                tris.Add(i0); tris.Add(i3); tris.Add(i2);
            }

            // Create region GameObject
            var go = new GameObject($"{area}FloorRegion_{minX}_{minY}");
            go.transform.SetParent(parentTransform, false);

            var mf = go.AddComponent<MeshFilter>();
            var mr = go.AddComponent<MeshRenderer>();

            mf.mesh = new Mesh { name = $"{area}FloorMesh_{minX}_{minY}" };
            mf.mesh.SetVertices(verts);
            mf.mesh.SetTriangles(tris, 0);
            mf.mesh.SetUVs(0, uvs);
            mf.mesh.RecalculateNormals();
            mf.mesh.RecalculateBounds();

            mr.sharedMaterial = GetMaterialFor(area);

            // store
            if (!regionRootsByArea.ContainsKey(area))
                regionRootsByArea[area] = new List<GameObject>();

            regionRootsByArea[area].Add(go);
        }

        // ========================== MATERIAL SELECTOR ==========================
        private Material GetMaterialFor(Area area)
        {
            return area switch
            {
                Area.Shop => gridSettings.ShopFloorMaterial,
                Area.Garden => gridSettings.GardenFloorMaterial,
                Area.Production => gridSettings.ProductionFloorMaterial,
                Area.Wall => gridSettings.WallFloorMaterial,
                _ => gridSettings.ShopFloorMaterial
            };
        }

        // ========================== CLEAR ==========================
        private void ClearAllMeshes()
        {
            foreach (var list in regionRootsByArea.Values)
                foreach (var go in list)
                    if (go) UnityEngine.Object.Destroy(go);

            regionRootsByArea.Clear();
        }

        public void Dispose() => ClearAllMeshes();
        public void Start() { }
    }
}
