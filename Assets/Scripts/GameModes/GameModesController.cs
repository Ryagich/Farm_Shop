using MessagePipe;
using Messages;
using VContainer.Unity;

namespace GameModes
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class GameModesController : IStartable
    {
        public GameMode GameMode { get; private set; } = GameMode.Game;
      
        private readonly IPublisher<GameModeChangedMessage> gameModeChangedPublisher;
        private readonly IPublisher<ChangeCursorStateMessage> changeCursorStatePublisher;

        public GameModesController
            (
                IPublisher<GameModeChangedMessage> gameModeChangedPublisher,
                IPublisher<ChangeCursorStateMessage> changeCursorStatePublisher,
                ISubscriber<OpenShopWithAreaRequest> openShopWithAreaRequestSubscriber,
                ISubscriber<ChangeGameModeRequest> openPageRequestSubscriber,
                ISubscriber<ChangeGameModeToDialogueRequest> changeGameModeToDialogueSubscriber
            )
        {
            this.gameModeChangedPublisher = gameModeChangedPublisher;
            this.changeCursorStatePublisher = changeCursorStatePublisher;

            openPageRequestSubscriber.Subscribe(OpenPage);
            openShopWithAreaRequestSubscriber.Subscribe(OpenShopWithArea);
            changeGameModeToDialogueSubscriber.Subscribe(OpenDialoguePage);
        }

        public void Start()
        {
            EnterMainGameMode();
            gameModeChangedPublisher.Publish(new GameModeChangedMessage(GameMode.Game));
        }

        private void EnterMainGameMode()
        {
            if (GameMode is GameMode.Game)
            {
                changeCursorStatePublisher.Publish(new ChangeCursorStateMessage());
                return;
            }
            GameMode = GameMode.Game;
            gameModeChangedPublisher.Publish(new GameModeChangedMessage(GameMode));
        }

        private void OpenPage(ChangeGameModeRequest msg)
        {
            if (GameMode == msg.Mode)
            {
                if (GameMode is GameMode.Game)
                {
                    EnterMainGameMode();
                }
                else
                {
                    EnterMainGameMode();
                    return;
                } 
            }
            GameMode = msg.Mode;
            gameModeChangedPublisher.Publish(new GameModeChangedMessage(GameMode));
        }
        
        private void OpenShopWithArea(OpenShopWithAreaRequest msg)
        {
            if (GameMode is GameMode.Shop)
            {
                EnterMainGameMode();
                return;
            }
            GameMode = GameMode.Shop;
            gameModeChangedPublisher.Publish(new GameModeChangedMessage(GameMode, msg.Area));
        }
        
        private void OpenDialoguePage(ChangeGameModeToDialogueRequest msg)
        {
            if (GameMode is GameMode.Dialogue)
            {
                EnterMainGameMode();
                return;
            }
            if (GameMode is not GameMode.Game)
            {
                return;
            }
            GameMode = GameMode.Dialogue;
            gameModeChangedPublisher.Publish(new GameModeChangedMessage(GameMode));
        }
    }

    public enum GameMode
    {
        Game,
        Shop,
        Inventory,
        Redactor,
        Dialogue,
    }
    
    public enum Area
    {
        None,
        Garden,
        Shop,
        Production,
        Wall,
    }
}