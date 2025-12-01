using System;
using System.Collections.Generic;
using System.Linq;
using GameModes;
using UnityEngine;
using Utils;

namespace BuildingsAndGrid
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class Tiles
    {
        public Tile[,] tiles { get; private set; }
        private Vector2Int offset = Vector2Int.zero;

        public int MaxX => tiles.GetLength(0) - offset.x;
        public int MaxY => tiles.GetLength(1) - offset.y;
        public int MinX => -offset.x;
        public int MinY => -offset.y;

        public Tiles(int width, int height)
        {
            tiles = new Tile[width, height];
        }

        public Tile GetTile(int x, int y)
        {
            var realX = x + offset.x;
            var realY = y + offset.y;

            if (realX < 0 || realX >= tiles.GetLength(0)
                          || realY < 0 || realY >= tiles.GetLength(1))
            {
                throw new IndexOutOfRangeException("Tile position out of bounds!");
            }

            return tiles[realX, realY];
        }
        
        public bool TryGetTile(int x, int y, out Tile tile)
        {
            var realX = x + offset.x;
            var realY = y + offset.y;

            if (realX < 0 || realX >= tiles.GetLength(0)
             || realY < 0 || realY >= tiles.GetLength(1))
            {
                tile = null;
                return false;
            }
            tile = tiles[realX, realY];
            return true;
        }
        
        public void SetTile(int x, int y, Tile tile)
        {
            var realX = x + offset.x;
            var realY = y + offset.y;

            if (realX < 0 || realX >= tiles.GetLength(0)
                          || realY < 0 || realY >= tiles.GetLength(1))
                throw new IndexOutOfRangeException("Tile position out of bounds!");

            tiles[realX, realY] = tile;
        }

        public void Resize(Vector2Int direction)
        {
            var newOffset = offset;

            if (direction.x < 0)
                newOffset = offset.WithXInt(offset.x + 1);
            if (direction.y < 0)
                newOffset = offset.WithYInt(offset.y + 1);

            var newTiles = new Tile[tiles.GetLength(0) + Mathf.Abs(direction.x),
                tiles.GetLength(1) + Mathf.Abs(direction.y)];

            for (var x = 0; x < tiles.GetLength(0); x++)
            for (var y = 0; y < tiles.GetLength(1); y++)
            {
                newTiles[x - offset.x + newOffset.x, y - offset.y + newOffset.y] = tiles[x, y];
            }
            offset = newOffset;
            tiles = newTiles;
        }
        
        public List<Tile> GetTilesAround(Vector2Int position, Vector2Int size)
        {
            var result = new List<Tile>();
            for (var x = position.x; x < position.x + size.x; x++)
            {
                if (x < MinX || x >= MaxX)
                {
                    continue;
                }
                for (var y = position.y; y < position.y + size.y; y++)
                {
                    if (y < MinY || y >= MaxY)
                    {
                        continue;
                    }
                    if (GetTile(x, y) != null)
                    {
                        result.Add(GetTile(x, y));
                    }
                }
            }
            return result;
        }
        
        public void Extend(Vector2Int direction, List<Tile> baseTiles)
        {
            Resize(direction);

            foreach (var oldTile in baseTiles)
            {
                var newX = oldTile.Index.x + direction.x;
                var newY = oldTile.Index.y + direction.y;

                var newTile = new Tile(
                                       new Vector2Int(newX, newY),
                                       oldTile.Type
                                      );
                SetTile(newX, newY, newTile);

                if (oldTile.Type == Area.Wall)
                {
                    var x = oldTile.Index.x - direction.x;
                    var y = oldTile.Index.y - direction.y;
                    var ExampleTile = GetTile(x, y);
                    oldTile.SetType(ExampleTile.Type);
                    // SetTile(oldTile.Index.x, oldTile.Index.y, tile);
                }
            }
        }

        public bool TryGetTileByTilesCondition(TileAroundInfoWithPosition mainCondition,
                                          List<TileAroundInfoWithPosition> conditions, out Tile tile)
        {
            var found = false;
            tile = null;
            for (var x = 0; x < tiles.GetLength(0) && !found; x++)
            {
                for (var y = 0; y < tiles.GetLength(1); y++)
                {
                    if (TryGetTileByConditions(x + mainCondition.Offset.x, y + mainCondition.Offset.y, mainCondition.Info, out tile) 
                     && conditions.All(c => TryGetTileByConditions(x + c.Offset.x, y + c.Offset.y,
                                                                   c.Info, out _)))
                    {
                        found = true;
                        break;
                    }
                }
            }
            return found;
        }
        
        public bool TryGetTileByConditions(
            int x, int y,
            List<TileAroundInfo> infos,
            out Tile tile)
        {
            if (!TryGetTile(x, y, out tile))
                return false;

            // Сброс счётчиков
            for (var i = 0; i < infos.Count; i++)
            {
                var tmp = infos[i];
                tmp.Count = 0;
                infos[i] = tmp;
            }

            // Только крест
            Vector2Int[] dirs =
            {
                new(1, 0),
                new(-1, 0),
                new(0, 1),
                new(0, -1)
            };
            
            foreach (var dir in dirs)
            {
                Tile around;
                var nx = x + dir.x;
                var ny = y + dir.y;
                var exists = TryGetTile(nx, ny, out around);

                for (var i = 0; i < infos.Count; i++)
                {
                    var info = infos[i];

                    if (!exists || around == null)
                    {
                        if (info.Area == Area.None)
                            info.Count++;

                        infos[i] = info;
                        continue;
                    }

                    if (around.Type == info.Area)
                        info.Count++;

                    infos[i] = info;
                }
            }

            // Проверяем совпадения
            for (var i = 0; i < infos.Count; i++)
            {
                if (infos[i].Count != infos[i].Need)
                    return false;
            }

            return true;
        }
    }

    public struct TileAroundInfo
    {
        public readonly Area Area;
        public readonly int Need;
        public int Count;

        public TileAroundInfo(Area area, int need)
        {
            Area = area;
            Need = need;
            Count = 0;
        }
    }
    
    public struct TileAroundInfoWithPosition
    {
        public readonly Vector2Int Offset;
        public readonly List<TileAroundInfo> Info;

        public TileAroundInfoWithPosition(Vector2Int offset, List<TileAroundInfo> info = null)
        {
            Offset = offset;
            Info = info ?? new();
        }
    }
}