using System;
using GameModes;
using MessagePipe;
using Messages;
using Storage;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Object = UnityEngine.Object;

namespace BuildingsAndGrid.Buildings
{
// ReSharper disable once ClassNeverInstantiated.Global
    public class BuildingMover : ITickable
    {
        private readonly GridRaycaster gridRaycaster;
        private readonly TilesController tilesController;
        private readonly GridSettings gridSettings;
        private readonly Storage.Storage storage;
        private readonly IObjectResolver resolver;
        private readonly IPublisher<CreatedNewBuildingOnGridRequest> createdNewBuildingOnGridRequest;
        private readonly IPublisher<ChangeGameModeRequest> changeGameModePublisher;
        private HighlightBuilding highlightBuilding;
        private BuildingConfig buildingConfig;
        private BuildingInStorage buildingInStorage;
        private Vector2Int currentCell = new(int.MaxValue, int.MaxValue);

        private Vector2Int currentSize;

// ===== Превью плитки =====
        private readonly System.Collections.Generic.List<GameObject> previewTiles = new();

        public BuildingMover
            (
                GridRaycaster gridRaycaster,
                TilesController tilesController, GridSettings gridSettings, Storage.Storage storage,
                IObjectResolver resolver,
                IPublisher<CreatedNewBuildingOnGridRequest> createdNewBuildingOnGridRequest,
                IPublisher<ChangeGameModeRequest> changeGameModePublisher,
                ISubscriber<GameModeChangedMessage> gameModeChangedSubscriber,
                ISubscriber<ClickMessage> clickSubscriber,
                ISubscriber<RightClickMessage> rightClickSubscriber,
                ISubscriber<ChoseBuildingMessage> chooseBuildingSubscriber,
                ISubscriber<LeftRotateMessage> leftRotateSubscriber,
                ISubscriber<RightRotateMessage> rightRotateSubscriber
            )
        {
            this.gridRaycaster = gridRaycaster;
            this.tilesController = tilesController;
            this.gridSettings = gridSettings;
            this.storage = storage;
            this.resolver = resolver;
            this.createdNewBuildingOnGridRequest = createdNewBuildingOnGridRequest;
            this.changeGameModePublisher = changeGameModePublisher;
           
            gameModeChangedSubscriber.Subscribe(OnGameModeChanged);
            clickSubscriber.Subscribe(OnClick);
            rightClickSubscriber.Subscribe(OnRightClick);
            chooseBuildingSubscriber.Subscribe(OnChooseBuilding);
            leftRotateSubscriber.Subscribe(RotateLeft);
            rightRotateSubscriber.Subscribe(RotateRight);
        }

        private void RotateLeft(LeftRotateMessage msg)
        {
            if (highlightBuilding is not null)
            {
                highlightBuilding.RotateLeft();
            }
            currentSize = new Vector2Int(currentSize.y, currentSize.x);
        }

        private void RotateRight(RightRotateMessage msg)
        {
            if (highlightBuilding is not null)
            {
                highlightBuilding.RotateRight();
            }
            currentSize = new Vector2Int(currentSize.y, currentSize.x);
        }

        private void OnGameModeChanged(GameModeChangedMessage msg)
        {
            var isActive = msg.GameMode == GameMode.Redactor;
            if (!isActive)
            {
                ReturnHighlight();
            }
        }

        private void OnClick(ClickMessage msg)
        {
            if (highlightBuilding is null)
            {
                return;
            }
            
            var cell = currentCell;
            if (tilesController.CanPlace(cell, currentSize, buildingConfig.Type))
            {
                var tiles = tilesController.Tiles.GetTilesAround(cell, currentSize);
                var highlightBuildingTransform = highlightBuilding.transform;
                createdNewBuildingOnGridRequest.Publish(new CreatedNewBuildingOnGridRequest(buildingConfig,
                                                         highlightBuildingTransform.position,
                                                         highlightBuilding.Content.localPosition,
                                                         highlightBuilding.GetContentRotation(), tiles));
                highlightBuilding.HaveLastPosition = false;
                buildingInStorage.Count--;
                if (buildingInStorage.Count < 1)
                {
                    HideHighlight();
                    changeGameModePublisher.Publish(new ChangeGameModeRequest(GameMode.Inventory));
                }
            }
        }

        private void OnRightClick(RightClickMessage msg)
        {
            if (highlightBuilding is null)
            {
                return;
            }
            ReturnHighlight();
            changeGameModePublisher.Publish(new ChangeGameModeRequest(GameMode.Inventory));
        }

        private void ReturnHighlight()
        {
            if (highlightBuilding && highlightBuilding.HaveLastPosition)
            {
                createdNewBuildingOnGridRequest.Publish(new CreatedNewBuildingOnGridRequest(buildingConfig,
                                                         highlightBuilding.LastPosition,
                                                         highlightBuilding.LastLocalPosition,
                                                         highlightBuilding.LastRotation,
                                                         highlightBuilding.LastTiles));
                buildingInStorage.Count--;
            }
            HideHighlight();
        }

        private void OnChooseBuilding(ChoseBuildingMessage msg)
        {
            HideHighlight();
            buildingInStorage = storage.Get(msg.BuildingConfig);
            buildingConfig = msg.BuildingConfig;
            currentSize = msg.BuildingConfig.Size;
            highlightBuilding = resolver.Instantiate(buildingConfig.HighlightBuilding);
            if (msg.HaveLastPosition)
            {
                highlightBuilding.LastPosition = msg.LastPosition;
                highlightBuilding.LastLocalPosition = msg.LastLocalPosition;
                highlightBuilding.LastRotation = msg.LastRotation;
                highlightBuilding.LastTiles = msg.LastTiles;
                highlightBuilding.HaveLastPosition = true;
            }
        }

        private void HideHighlight()
        {
            if (highlightBuilding != null)
                Object.Destroy(highlightBuilding.gameObject);
            highlightBuilding = null;
            HidePreviewTiles();
        }

// ==================== ПРЕВЬЮ ПЛИТКИ ==================
        private void HidePreviewTiles()
        {
            foreach (var t in previewTiles) t.SetActive(false);
        }

        private void UpdatePreviewTiles(Vector2Int cell)
        {
            HidePreviewTiles();
            var tiles = tilesController.Tiles;
            var y = gridSettings.yOffset;
            var count = 0;
            for (var dx = 0; dx < currentSize.x; dx++)
            for (var dy = 0; dy < currentSize.y; dy++)
            {
                var tx = cell.x + dx;
                var
                    ty = cell.y + dy;
// Проверяем существование тайла
                Tile tile;
                try
                {
                    tile = tiles.GetTile(tx, ty);
                }
                catch
                {
                    tile = null;
                }
                if (tile == null) continue;
// Создаём preview tile если нужно
                if (previewTiles.Count <= count)
                {
                    var obj = Object.Instantiate(gridSettings.HighlightTile);
                    obj.transform.localScale = gridSettings.TileSize;
                    previewTiles.Add(obj);
                }
                var p = previewTiles[count];
                p.SetActive(true);
// Позиция превью
                p.transform.position = new Vector3((tx + 0.5f) * gridSettings.TileSize.x, y, (ty + 0.5f)
                                                     * gridSettings.TileSize.z);
                //======= ЛОГИКА УСТАНОВКИ МАТЕРИАЛА =========
                var renderer = p.GetComponent<MeshRenderer>();
                if (renderer != null)
                {
                    var matchesType = tile.Type == buildingConfig.Type;
                    var free = tile.Building == null;
                    if (matchesType && free) renderer.material = gridSettings.GhostGreenMaterial;
                    else renderer.material = gridSettings.GhostRedMaterial;
                }
//============================================
                count++;
            }
//// Скрываем лишние батчи
            for (; count < previewTiles.Count; count++) previewTiles[count].SetActive(false);
        }

// ==================== ДВИЖЕНИЕ ====================
        public void Tick()
        {
            if (highlightBuilding == null) return;
            var cell = gridRaycaster.GetRaycastPositionOnGrid();
            cell =
                new
                    Vector2Int(Math.Clamp(cell.x, tilesController.Tiles.MinX, tilesController.Tiles.MaxX - currentSize.x),
                               Math.Clamp(cell.y, tilesController.Tiles.MinY,
                                          tilesController.Tiles.MaxY - currentSize.y));
            currentCell = cell;
            highlightBuilding.transform.position =
                new Vector3(cell.x * gridSettings.TileSize.x, gridSettings.yOffset, cell.y * gridSettings.TileSize.z);
            UpdatePreviewTiles(cell);
        }
    }
}