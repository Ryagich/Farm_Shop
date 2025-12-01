using System;
using System.Collections.Generic;
using System.Linq;
using GameModes;
using MessagePipe;
using Messages;
using UnityEngine;

namespace BuildingsAndGrid
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class TilesController
    {
        public Tiles Tiles { get; private set; }

        private readonly GridSettings gridSettings;
        private readonly IPublisher<GridExtendMessage> gridExtendPublisher;

        public TilesController
            (
                GridSettings gridSettings,
                IPublisher<GridExtendMessage> gridExtendPublisher
            )
        {
            this.gridSettings = gridSettings;
            this.gridExtendPublisher = gridExtendPublisher;
            Tiles = CreateGrid();
        }
        
        private Tiles CreateGrid()
        {
            var tiles = GetEmptyTiles();
            var startPosition = gridSettings.Info[0].Position;

            foreach (var info in gridSettings.Info)
                for (var x = 0; x < info.Size.x; x++)
                for (var y = 0; y < info.Size.y; y++)
                {
                    var index = new Vector2Int(x + startPosition.x + info.Position.x,
                                               y + startPosition.y + info.Position.y);

                    var tile = new Tile(index, info.Type);

                    if (tiles.GetTile(tile.Index.x, tile.Index.y) != null)
                        throw new ArgumentOutOfRangeException($"На этом месте уже есть тайл. {index}");

                    tiles.SetTile(tile.Index.x, tile.Index.y, tile);
                }
            AddInnerWalls(tiles);
            AddWalls(tiles);

            return tiles;
        }
        
        private void AddInnerWalls(Tiles tiles)
        {
            Vector2Int[] dirs = new[]
                                {
                                    new Vector2Int(1, 0),
                                    new Vector2Int(-1, 0),
                                    new Vector2Int(0, 1),
                                    new Vector2Int(0, -1)
                                };

            for (int x = tiles.MinX; x < tiles.MaxX; x++)
            {
                for (int y = tiles.MinY; y < tiles.MaxY; y++)
                {
                    if (!tiles.TryGetTile(x, y, out var tile) || tile == null)
                        continue;

                    foreach (var dir in dirs)
                    {
                        int nx = x + dir.x;
                        int ny = y + dir.y;

                        if (!tiles.TryGetTile(nx, ny, out var neighbor))
                            continue;

                        if (neighbor == null)
                        {
                            // сосед пустой ➜ ставим стену
                            tiles.SetTile(nx, ny, new Tile(new Vector2Int(nx, ny), Area.Wall));
                            continue;
                        }

                        // типы разные и оба не стена ➜ ставим стену вместо соседа, если он пустой
                        if (tile.Type != neighbor.Type &&
                            tile.Type != Area.Wall &&
                            neighbor.Type != Area.Wall)
                        {
                            // ставим стену на стороне соседа, если он свободный
                            tiles.SetTile(nx, ny, new Tile(new Vector2Int(nx, ny), Area.Wall));
                        }
                    }
                }
            }
        }
        
        private void AddWalls(Tiles tiles)
        {
            int minX = int.MaxValue, minY = int.MaxValue;
            int maxX = int.MinValue, maxY = int.MinValue;

            for (var x = tiles.MinX; x < tiles.MaxX; x++)
            {
                for (var y = tiles.MinY; y < tiles.MaxY; y++)
                {
                    if (!tiles.TryGetTile(x, y, out var t) || t == null || t.Type == Area.Wall)
                        continue;

                    minX = Mathf.Min(minX, x);
                    minY = Mathf.Min(minY, y);
                    maxX = Mathf.Max(maxX, x);
                    maxY = Mathf.Max(maxY, y);
                }
            }

            // 2️⃣ Расширяем сетку на 1 клетку со всех сторон
            tiles.Resize(new Vector2Int(-1, 0));
            tiles.Resize(new Vector2Int(1, 0));
            tiles.Resize(new Vector2Int(0, -1));
            tiles.Resize(new Vector2Int(0, 1));

            // 3️⃣ Строим стеновую рамку по внешнему контуру
            for (int x = minX - 1; x <= maxX + 1; x++)
            {
                for (int y = minY - 1; y <= maxY + 1; y++)
                {
                    // если это граница — ставим стену
                    bool isBorder =
                        x == minX - 1 ||
                        x == maxX + 1 ||
                        y == minY - 1 ||
                        y == maxY + 1;

                    if (!isBorder)
                        continue;

                    if (tiles.TryGetTile(x, y, out var existing) && existing != null)
                        continue;

                    tiles.SetTile(x, y, new Tile(new Vector2Int(x, y), Area.Wall));
                }
            }
        }
        
        public bool CanPlace(Vector2Int position, Vector2Int size, Area type)
        {
            var currTiles = Tiles.GetTilesAround(position, size);
            if (currTiles.Count < size.x * size.y)
            {
                return false;
            }
            foreach (var tile in currTiles)
            {
                if (!tile.IsFree || tile.Type != type)
                {
                    return false;
                }
            }
            return true;
        }
        
        private Tiles GetEmptyTiles()
        {
            var maxX = gridSettings.Info.Max(info => info.Position.x + info.Size.x);
            var maxY = gridSettings.Info.Max(info => info.Position.y + info.Size.y);
            return new Tiles(maxX, maxY);
        }
        
        public void ExtendGrid(Vector2Int direction, List<Tile> baseTiles)
        {
            Tiles.Extend(direction, baseTiles);
            gridExtendPublisher.Publish(new GridExtendMessage());
        }
    }
}