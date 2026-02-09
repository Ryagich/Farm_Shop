using System.Collections.Generic;
using BuildingsAndGrid;
using BuildingsAndGrid.Buildings;
using UnityEngine;

namespace Messages
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public readonly struct ChoseBuildingMessage
    {
        public readonly BuildingConfig BuildingConfig;
        public readonly Vector3 LastPosition;
        public readonly Vector3 LastLocalPosition;
        public readonly Quaternion LastRotation;
        public readonly List<Tile> LastTiles;
        public readonly Vector2Int LastCell;
        public readonly bool HaveLastPosition;
        
        public ChoseBuildingMessage
            (
                BuildingConfig buildingConfig,
                Vector3 lastPosition,
                Vector3 lastLocalPosition,
                Quaternion lastRotation,
                List<Tile> lastTiles,
                Vector2Int lastCell,
                bool haveLastPosition = false
            )
        {
            BuildingConfig = buildingConfig;
            LastPosition = lastPosition;
            LastLocalPosition = lastLocalPosition;
            LastRotation = lastRotation;
            LastTiles = lastTiles;
            LastCell = lastCell;
            HaveLastPosition = haveLastPosition;
        }
    }
}