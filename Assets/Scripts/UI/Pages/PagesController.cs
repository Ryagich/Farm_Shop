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

        private BasePage currentPage;
        
        public PagesController 
            (
                GameModesController gamesController,
                CursorController cursorController,
                MainPage mainPage,
                MainPageWithUI mainPageWithUI,
                ShopPage shopPage,
                ISubscriber<ChangeCursorStateMessage> changeCursorStateSubscriber,
                ISubscriber<GameModeChangedMessage> gameModeChangeSubscriber
            )
        {
            this.gamesController = gamesController;
            this.cursorController = cursorController;
            this.mainPage = mainPage;
            this.mainPageWithUI = mainPageWithUI;
            this.shopPage = shopPage;

            currentPage = mainPageWithUI;
            currentPage.Draw();
            
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
        }

        private void Update(GameModes.GameModes gameMode)
        {
            Debug.Log($"Update Page {gameMode}");
            HideCurrentPage();
            switch (gameMode)
            {
                case GameModes.GameModes.Game:
                    if (cursorController.IsVisibleInPlayMode)
                        currentPage = mainPageWithUI;
                    else
                        currentPage = mainPage;
                    break;
                case GameModes.GameModes.Redactor:
                    break; 
                case GameModes.GameModes.Shop:
                    currentPage = shopPage;
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
        Inventory
    }
}