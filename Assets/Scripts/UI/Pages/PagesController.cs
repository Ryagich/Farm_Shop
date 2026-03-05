using GameModes;
using Input.Cursor;
using MessagePipe;
using Messages;
using UnityEngine;
using VContainer.Unity;

namespace UI.Pages
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class PagesController : IStartable
    {
        private readonly GameModesController gamesController;
        private readonly CursorController cursorController;
        private readonly MainPage mainPage;
        private readonly MainPageWithUI mainPageWithUI;
        private readonly ShopPage shopPage;
        private readonly InventoryPage inventoryPage;
        private readonly RedactorPage redactorPage;
        private readonly DialoguePage dialoguePage;

        private BasePage currentPage;
        
        public PagesController 
            (
                GameModesController gamesController,
                CursorController cursorController,
                MainPage mainPage,
                MainPageWithUI mainPageWithUI,
                ShopPage shopPage,
                InventoryPage inventoryPage, 
                RedactorPage redactorPage,
                DialoguePage dialoguePage,
                ISubscriber<ChangeCursorStateMessage> changeCursorStateSubscriber,
                ISubscriber<GameModeChangedMessage> gameModeChangeSubscriber
            )
        {
            this.gamesController = gamesController;
            this.cursorController = cursorController;
            this.mainPage = mainPage;
            this.mainPageWithUI = mainPageWithUI;
            this.shopPage = shopPage;
            this.inventoryPage = inventoryPage;
            this.redactorPage = redactorPage;
            this.dialoguePage = dialoguePage;

            gameModeChangeSubscriber.Subscribe(OnGameModeChanged);
            changeCursorStateSubscriber.Subscribe(OnCursorStateChanged);
        }

        private void OnCursorStateChanged(ChangeCursorStateMessage msg)
        {
            Update(gamesController.GameMode);
        }
        
        private void OnGameModeChanged(GameModeChangedMessage msg)
        {
            Update(msg.GameMode);
            if (msg.Area != Area.None && currentPage is AreaDrawer areaDrawer)
            {
                areaDrawer.SetArea(msg.Area);
            }
        }
        
        private void Update(GameMode gameMode)
        {
            Debug.Log($"Update Page {gameMode}");
            HideCurrentPage();
            switch (gameMode)
            {
                case GameMode.Game:
                    if (cursorController.IsVisibleInPlayMode)
                        currentPage = mainPageWithUI;
                    else
                        currentPage = mainPage;
                    break;
                case GameMode.Shop:
                    currentPage = shopPage;
                    break;
                case GameMode.Inventory:
                    currentPage = inventoryPage;
                    break; 
                case GameMode.Redactor:
                    currentPage = redactorPage;
                    break;
                case GameMode.Dialogue:
                    currentPage = dialoguePage;
                    break;
                default:
                    if (cursorController.IsVisibleInPlayMode)
                        currentPage = mainPageWithUI;
                    else
                        currentPage = mainPage;
                    break;
            }
            if (currentPage is not null)
                currentPage.Draw();
        }
        
        private void HideCurrentPage()
        {
            currentPage?.Hide();
            currentPage = null;
        }
        
        public void Start() { }
    }

    public enum PageType
    {
        MainGame,
        GameWithUI,
        Shop,
        Inventory,
        Redactor,
        Dialogue,
        
    }
}