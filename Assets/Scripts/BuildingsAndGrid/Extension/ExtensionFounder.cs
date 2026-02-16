using System.Collections.Generic;
using GameModes;
using Inventory.Finance;
using MessagePipe;
using Messages;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer.Unity;

namespace BuildingsAndGrid.Extension
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ExtensionFounder : ITickable
    {
        private readonly GridSettings gridSettings;
        private readonly Camera camera;
        private readonly TilesController tilesController;

        private readonly List<GameObject> tiles = new();
        private readonly GridExtensionSpawner gridExtensionSpawner;
        private readonly FinanceManager financeManager;

        private ExtensionPointer currentPointer;

        private bool isDraw;
        
        private ExtensionFounder
            (
                GridSettings gridSettings,
                Camera camera,
                TilesController tilesController,
                GridExtensionSpawner gridExtensionSpawner,
                FinanceManager financeManager,
                ISubscriber<GameModeChangedMessage> gameModeChangedSubscriber,
                ISubscriber<ClickMessage> clickSubscriber
            )
        {
            this.gridSettings = gridSettings;
            this.camera = camera;
            this.tilesController = tilesController;
            this.gridExtensionSpawner = gridExtensionSpawner;
            this.financeManager = financeManager;

            clickSubscriber.Subscribe(ExtentGrid);
            gameModeChangedSubscriber.Subscribe(OnGameModeChanged);
        }

        private void OnGameModeChanged(GameModeChangedMessage msg)
        {
            isDraw = msg.GameMode == GameMode.Inventory;
            if (msg.GameMode != GameMode.Inventory)
            {
                Hide();
            }
        }
        
        private void ExtentGrid(ClickMessage msg)
        {
            if (currentPointer != null && financeManager.TryChangeValue(-currentPointer.Price))
            {
                tilesController.ExtendGrid(currentPointer.Direction, currentPointer.Tiles);
                gridExtensionSpawner.ForceRefresh();
            }
        }

        public void Tick()
        {
            if (isDraw)
                if (TryGetRaycastExtension(out var pointer))
                {
                    if (pointer != currentPointer)
                    {
                        Hide();
                        currentPointer = pointer;
                        GetTiles(pointer);
                        for (var i = 0; i < pointer.Tiles.Count; i++)
                        {
                            var pos = pointer.Tiles[i].Index + Vector2.one * .5f 
                                    + pointer.Direction;
                            tiles[i].SetActive(true);
                            tiles[i].transform.position =
                                new Vector3(pos.x * gridSettings.TileSize.x,
                                            0.25f * gridSettings.TileSize.y,
                                            pos.y * gridSettings.TileSize.z);
                        }
                    }
                }
                else
                    Hide();
        }

        private void Hide()
        {
            currentPointer = null;
            foreach (var tile in tiles)
                tile.SetActive(false);
        }

        private void GetTiles(ExtensionPointer pointer)
        {
            while (tiles.Count < pointer.Tiles.Count)
            {
                var tile = Object.Instantiate(gridSettings.HighlightTile);
                tile.transform.localScale = gridSettings.TileSize;
                tile.GetComponent<MeshRenderer>().material = gridSettings.GhostMaterial;
                tiles.Add(tile);
            }
        }

        private bool TryGetRaycastExtension(out ExtensionPointer pointer)
        {
            pointer = null;
            if (Physics.Raycast(camera.ScreenPointToRay(Mouse.current.position.ReadValue()), 
                                out var hit, 100, gridSettings.ExtensionLayer))
            {
                if (hit.transform != null)
                {
                    pointer = hit.transform.GetComponent<ExtensionPointer>();
                    return pointer != null;
                }
            }
            return false;
        }
    }
}