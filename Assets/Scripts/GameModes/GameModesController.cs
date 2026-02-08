using Localization;
using MessagePipe;
using Messages;
using Utils;
using VContainer.Unity;
using YG;

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
                ISubscriber<OpenShopWithAreaRequest> OpenShopWithAreaRequestSubscriber,
                ISubscriber<ChangeGameModeRequest> OpenPageRequestSubscriber
            )
        {
            this.gameModeChangedPublisher = gameModeChangedPublisher;
            this.changeCursorStatePublisher = changeCursorStatePublisher;

            OpenShopWithAreaRequestSubscriber.Subscribe(OpenShopWithArea);
            OpenPageRequestSubscriber.Subscribe(OpenPage);
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
    }

    public enum GameMode
    {
        Game,
        Shop,
        Inventory,
        Redactor
    }
    
    public enum Area
    {
        None,
        Garden,
        Shop,
        Production,
        Wall
    }
}