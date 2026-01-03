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

        private bool isLocalizationReady;
        
        public GameModesController
            (
                IPublisher<GameModeChangedMessage> gameModeChangedPublisher,
                IPublisher<ChangeCursorStateMessage> changeCursorStatePublisher,
                ISubscriber<OpenShopWithAreaRequest> OpenShopWithAreaRequestSubscriber,
                ISubscriber<ChangeGameModeRequest> OpenPageRequestSubscriber,
                ISubscriber<TranslationStateChangedMessage> TranslationStateChangedMessageSubscriber
            )
        {
            this.gameModeChangedPublisher = gameModeChangedPublisher;
            this.changeCursorStatePublisher = changeCursorStatePublisher;

            OpenShopWithAreaRequestSubscriber.Subscribe(OpenShopWithArea);
            OpenPageRequestSubscriber.Subscribe(OpenPage);
            TranslationStateChangedMessageSubscriber.Subscribe(OnLocalizationStateChanged);
        }

        //Пока приходит сообщение с одного места только в начале игры, поэтому по его приходу - перехожу к нормальному режиму UI
        //Когда языки можно будет менять с настроек - потребуется дополнительная обработка
        private void OnLocalizationStateChanged(TranslationStateChangedMessage msg)
        {
            isLocalizationReady = msg.IsReady;
            EnterMainGameMode();
        }

        public void Start()
        {
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
            if (!isLocalizationReady)
                return;
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