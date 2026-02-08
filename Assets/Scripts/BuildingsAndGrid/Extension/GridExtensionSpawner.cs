using System.Collections.Generic;
using System.Linq;
using GameModes;
using MessagePipe;
using Messages;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Object = UnityEngine.Object;

namespace BuildingsAndGrid.Extension
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class GridExtensionSpawner
    {
        private readonly GridSettings gridSettings;
        private readonly TilesController tilesController;
        private readonly GridLifetimeScope gridLifetimeScope;

        private List<ExtensionPointer> extensions = new();

        private GridExtensionSpawner
            (
                GridSettings gridSettings,
                TilesController tilesController,
                ISubscriber<GameModeChangedMessage> gameModeChangeSubscriber,
                GridLifetimeScope gridLifetimeScope
            )
        {
            this.gridSettings = gridSettings;
            this.tilesController = tilesController;
            this.gridLifetimeScope = gridLifetimeScope;

            gameModeChangeSubscriber.Subscribe(OnGameModeChanged);
            Debug.Log($"GridExtensionSpawner Constructor");
        }

        private void OnGameModeChanged(GameModeChangedMessage msg)
        {
            if (msg.GameMode == GameMode.Inventory)
                ShowExtensions();
            else
                HideExtensions();
        }

        public void ForceRefresh()
        {
            // HideExtensions();
            ShowExtensions();
        }

        private void ShowExtensions()
        {
            var tiles = tilesController.Tiles;

            HideExtensions();

            // ===== BOTTOM edge =====
            var bottomTiles = new List<Tile>();
            for (var x = tiles.MinX; x < tiles.MaxX; x++)
            for (var y = tiles.MinY; y < tiles.MaxY; y++)
            {
                var tile = SafeGet(tiles, x, y);
                if (tile == null) continue;
            
                bool hasBelow = y - 1 >= tiles.MinY && SafeGet(tiles, x, y - 1) != null;
                if (!hasBelow)
                    bottomTiles.Add(tile);
            }
            SpawnPointers(bottomTiles, Vector2Int.down);
            
            // ===== TOP edge =====
            var topTiles = new List<Tile>();
            for (var x = tiles.MinX; x < tiles.MaxX; x++)
            for (var y = tiles.MinY; y < tiles.MaxY; y++)
            {
                var tile = SafeGet(tiles, x, y);
                if (tile == null) continue;
            
                bool hasAbove = y + 1 < tiles.MaxY && SafeGet(tiles, x, y + 1) != null;
                if (!hasAbove)
                    topTiles.Add(tile);
            }
            SpawnPointers(topTiles, Vector2Int.up);
            
            // ===== LEFT edge =====
            var leftTiles = new List<Tile>();
            for (var x = tiles.MinX; x < tiles.MaxX; x++)
            for (var y = tiles.MinY; y < tiles.MaxY; y++)
            {
                var tile = SafeGet(tiles, x, y);
                if (tile == null) continue;
            
                bool hasLeft = x - 1 >= tiles.MinX && SafeGet(tiles, x - 1, y) != null;
                if (!hasLeft)
                    leftTiles.Add(tile);
            }
            SpawnPointers(leftTiles, Vector2Int.left);
            
            // ===== RIGHT edge =====
            var rightTiles = new List<Tile>();
            for (var x = tiles.MinX; x < tiles.MaxX; x++)
            for (var y = tiles.MinY; y < tiles.MaxY; y++)
            {
                var tile = SafeGet(tiles, x, y);
                if (tile == null) continue;
            
                bool hasRight = x + 1 < tiles.MaxX && SafeGet(tiles, x + 1, y) != null;
                if (!hasRight)
                    rightTiles.Add(tile);
            }
            SpawnPointers(rightTiles, Vector2Int.right);
        }

        private Tile SafeGet(Tiles tiles, int x, int y)
        {
            try { return tiles.GetTile(x, y); }
            catch { return null; }
        }

        private void SpawnPointers(List<Tile> tiles, Vector2Int direction)
        {
            if (tiles.Count == 0)
                return;

            // === сортируем по нужной оси ===
            if (direction == Vector2Int.left || direction == Vector2Int.right)
                tiles = tiles.OrderBy(t => t.Index.y).ThenBy(t => t.Index.x).ToList();
            else
                tiles = tiles.OrderBy(t => t.Index.x).ThenBy(t => t.Index.y).ToList();
            
            // === группируем по непрерывным последовательностям ===
            List<List<Tile>> groups = new();
            List<Tile> group = new() { tiles[0] };

            for (var i = 1; i < tiles.Count; i++)
            {
                var prev = tiles[i - 1];
                var curr = tiles[i];

                var isSequential =
                    (direction == Vector2Int.up || direction == Vector2Int.down)
                        ? curr.Index.x == prev.Index.x + 1
                        : curr.Index.y == prev.Index.y + 1;

                // тип тайла НЕ ВАЖЕН — группируем только по смежности
                if (isSequential)
                    group.Add(curr);
                else
                {
                    groups.Add(group);
                    group = new() { curr };
                }
            }
            groups.Add(group);
            
            // === создаём ExtensionPointer на группу ===
            foreach (var g in groups)
            {
                var extension = gridLifetimeScope.CreateChildFromPrefab(gridSettings.ExpansionPref);
                var extensionPointer = extension.GetComponent<ExtensionPointer>();
                // var extension = gridLifetimeScope.Instantiate(gridSettings.ExpansionPref);

                var pos = Vector2.zero;
                foreach (var t in g)
                    pos += t.Index + Vector2.one * .5f + direction * 2;
                pos /= g.Count; // центр группы

                extension.transform.position = new Vector3(
                    pos.x * gridSettings.TileSize.x,
                    0.25f * gridSettings.TileSize.y,
                    pos.y * gridSettings.TileSize.z
                );

                extensionPointer.SetValues(gridSettings, direction, g, tilesController.Tiles.tiles.Length);
                extensions.Add(extensionPointer);
            }
        }

        private void HideExtensions()
        {
            foreach (var ext in extensions)
                Object.Destroy(ext.gameObject);

            extensions.Clear();
        }
    }
}
