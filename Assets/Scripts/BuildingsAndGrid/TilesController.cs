using System;
using System.Collections.Generic;
using System.Linq;
using GameModes;
using MessagePipe;
using Messages;
using UnityEngine;
using YG;

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
            Tiles = YG2.saves.Tiles == null || YG2.saves.Tiles.GetLength(0) is 0 ? CreateNewGrid() : CreateGrid();
        }
        
        private Tiles CreateGrid()
        {
            var saved = YG2.saves.Tiles;
            var savedOffset = YG2.saves.Offset;

            var width  = saved.GetLength(0);
            var height = saved.GetLength(1);

            Debug.Log($"Create Grid {width}|{height}, Offset={savedOffset}");

            // ВАЖНО: создаём Tiles сразу с offset из сейва
            var tiles = new Tiles(width, height, savedOffset);

            // идём по REAL координатам массива сохранения
            for (var realX = 0; realX < width; realX++)
            for (var realY = 0; realY < height; realY++)
            {
                var savedValue = saved[realX, realY];

                if (!TryParseArea(savedValue, out var type))
                    throw new ArgumentException($"Area '{savedValue}' not exist");

                // перевод в LOGICAL координаты
                var logicalX = realX - savedOffset.x;
                var logicalY = realY - savedOffset.y;

                var index = new Vector2Int(logicalX, logicalY);

                // безопасная проверка на занятость
                if (tiles.TryGetTile(logicalX, logicalY, out var existing) && existing != null)
                    throw new ArgumentOutOfRangeException($"На этом месте уже есть тайл. {index}");

                tiles.SetTile(logicalX, logicalY, new Tile(index, type));
            }

            return tiles;
        }

        
        private static bool TryParseArea(string value, out Area area)
        {
            return Enum.TryParse(value, out area);
        }
        
        private Tiles CreateNewGrid()
        {
            Debug.Log($"CreateNewGrid");
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
                    {
                        throw new ArgumentOutOfRangeException($"На этом месте уже есть тайл. {index}");
                    }
                    tiles.SetTile(tile.Index.x, tile.Index.y, tile);
                }
            AddInnerWalls(tiles);
            AddWalls(tiles);
            
            YG2.saves.Offset = tiles.Offset;
            YG2.saves.Tiles = new string[tiles.tiles.GetLength(0), tiles.tiles.GetLength(1)];

            for (var x = 0; x < tiles.tiles.GetLength(0); x++)
            for (var y = 0; y < tiles.tiles.GetLength(1); y++)
            {
                YG2.saves.Tiles[x, y] = tiles.tiles[x, y].Type.ToString();
            }

            YG2.SaveProgress();

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

            for (var x = tiles.MinX; x < tiles.MaxX; x++)
            {
                for (var y = tiles.MinY; y < tiles.MaxY; y++)
                {
                    if (!tiles.TryGetTile(x, y, out var tile) || tile == null)
                        continue;

                    foreach (var dir in dirs)
                    {
                        var nx = x + dir.x;
                        var ny = y + dir.y;

                        if (!tiles.TryGetTile(nx, ny, out var neighbor))
                            continue;

                        if (neighbor == null)
                        {
                            // сосед пустой -> стенa
                            tiles.SetTile(nx, ny, new Tile(new Vector2Int(nx, ny), Area.Wall));
                            continue;
                        }

                        // типы разные и не стена -> ставим стену вместо соседа, если он пустой
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

            // Расширяем сетку на 1 клетку со всех сторон
            tiles.Resize(new Vector2Int(-1, 0));
            tiles.Resize(new Vector2Int(1, 0));
            tiles.Resize(new Vector2Int(0, -1));
            tiles.Resize(new Vector2Int(0, 1));

            // Строим стеновую рамку по внешнему контуру
            for (var x = minX - 1; x <= maxX + 1; x++)
            {
                for (var y = minY - 1; y <= maxY + 1; y++)
                {
                    // если это граница — ставим стену
                    var isBorder =
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

            YG2.saves.Offset = Tiles.Offset;
            YG2.saves.Tiles = new string[Tiles.tiles.GetLength(0), Tiles.tiles.GetLength(1)];
            
            for (var x = 0; x < Tiles.tiles.GetLength(0); x++)
            for (var y = 0; y < Tiles.tiles.GetLength(1); y++)
            {
                YG2.saves.Tiles[x, y] = Tiles.tiles[x, y].Type.ToString();
            }
            
            YG2.SaveProgress();
        }
    }
}