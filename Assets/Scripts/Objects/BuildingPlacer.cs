using System.Collections.Generic;
using BuildingsAndGrid;
using BuildingsAndGrid.Buildings;
using MessagePipe;
using Messages;
using UnityEngine;

namespace Objects
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class BuildingPlacer
    {
        private readonly GridSettings gridSettings;
        private readonly TilesController tilesController;
        private readonly IPublisher<CreatedNewBuildingOnGridRequest> createdNewBuildingOnGridPublisher;

        public BuildingPlacer
            (
                GridSettings gridSettings,
                TilesController tilesController,
                IPublisher<CreatedNewBuildingOnGridRequest> createdNewBuildingOnGridPublisher
            )
        {
            this.gridSettings = gridSettings;
            this.tilesController = tilesController;
            this.createdNewBuildingOnGridPublisher = createdNewBuildingOnGridPublisher;
        }
        
        public bool TryPlaceBuildingByPattern
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

            createdNewBuildingOnGridPublisher.Publish(new CreatedNewBuildingOnGridRequest
                                                          (
                                                           config,
                                                           new Vector3(px * gridSettings.TileSize.x,
                                                                       0,
                                                                       py * gridSettings.TileSize.z),
                                                           localPosition,
                                                           rotation,
                                                           tilesForBuilding,
                                                           new Vector2Int(px, py)
                                                          ));
            return true;
        }
    }
}